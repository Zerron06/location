using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace location
{
    public class Whois
    {
        static void Main(string[] args)
        {
            bool debug = false;
            string serverAddress = "whois.net.dcs.hull.ac.uk";
            int serverPort = 43;
            string protocol = "whois", username=null, location=null;
            try
            {

                for (int i=0; i<args.Length; ++i)
                {
                switch(args[i])
                {
                    case "-h": try { serverAddress = args[++i]; }
                            catch
                            {
                                Console.WriteLine("ERROR: No server given");
                            }
                           
                        break;
                    case "-p": try { serverPort = int.Parse(args[++i]); }
                            catch
                            {
                                Console.WriteLine("ERROR: No port given");
                            }
                        break;
                    case "-h9": protocol = args[i];
                        break;
                    case "-h0": protocol = args[i];
                        break;
                    case "-h1": protocol = args[i];
                        break;
                    case "-d": debug = true;
                        break;
                    default:
                        if (username == null) { username = args[i]; }

                        else if (location == null) { location = args[i]; }

                        else
                        {
                            Console.WriteLine("ERROR: Too many arguments " + args[i]);
                            return;
                        }
                        break;
                }
            }
            if (username == null) { Console.WriteLine("ERROR: No username"); }
            if (debug) { Console.WriteLine($"Debug:  {serverAddress}, {serverPort}, {protocol}, {username}, {location}"); }



                
                TcpClient client = new TcpClient();

                
                client.Connect(serverAddress, serverPort);
                StreamWriter sw = new StreamWriter(client.GetStream());
                StreamReader sr = new StreamReader(client.GetStream());
                client.SendTimeout = 1000;
                client.ReceiveTimeout = 1000;
                //string line1 = sr.ReadLine();
                //while (line1 != "") line1 = sr.ReadLine();
                string response;

                switch (protocol)
                {
                    case "-h1":
                        if(location == null && serverPort==80)
                        {
                            sw.WriteLine($"GET / name ={ username} HTTP/1.1\r\nHost: { serverAddress}\r\n");
                            sw.Flush();

                            string line = sr.ReadLine().Trim();
                            if (line == "HTTP/1.1 404 Not Found")
                            {
                                Console.WriteLine(line + "\r\nContent-Type: text/plain\r\n\r\n");//not found message
                                //response = line + "\r\nContent-Type: text/plain\r\n\r\n";
                            }
                            else
                            {
                                while ((line != "") == true)
                                {
                                    line = sr.ReadLine().Trim();
                                }

                                Console.WriteLine(username + " is <html>");
                                Console.WriteLine("<head><title>301 Moved Permanently</title></head>");
                                Console.WriteLine("<body>");
                                Console.WriteLine("<center><h1>301 Moved Permanently</h1></center>");
                                Console.WriteLine("<hr><center>nginx</center>");
                                Console.WriteLine("</body>");
                                Console.WriteLine("</html>");
                                try
                                {
                                    int c;
                                    while ((c = sr.Read()) > 0)
                                    {
                                        Console.WriteLine((char)c);
                                    }
                                }
                                catch { }
                                
                            }
                        }
                        else if (location == null && serverPort!=80)
                        {
                            sw.WriteLine($"GET /?name={username} HTTP/1.1\r\nHost: {serverAddress}\r\n");
                            sw.Flush();
                            string serverResponse = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if(serverResponse == "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(serverResponse);

                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                            }
                        }
                        else
                        {
                            string combineString = $"name={username}&location={location}";
                            sw.WriteLine($"POST / HTTP/1.1\r\nHost: {serverAddress}\r\nContent-Length: {combineString.Length}\r\n\r\n" + combineString);
                            sw.Flush();
                            string serverResponse = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new char[] { ' ' }, 3);
                            serverResponse = sections[2];
                            if (serverResponse == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                            }
                            else
                            {
                                Console.WriteLine(serverResponse);
                            }
                        }
                        break;
                    case "-h0":
                        if (location == null)
                        {
                            sw.WriteLine($"GET /?{username} HTTP/1.0\r\n");
                            sw.Flush();
                            string serverResponse = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if (serverResponse == "HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(serverResponse);

                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                            }
                        }

                        else
                        {

                            sw.WriteLine($"POST /{username} HTTP/1.0\r\nContent-Length: {location.Length}\r\n\r\n{location}");
                            sw.Flush();
                            string serverResponse = sr.ReadLine();
                            string[] sections = serverResponse.Split(new char[] { ' ' }, 3);
                            serverResponse = sections[2];

                            if(serverResponse == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);

                            }
                            else
                            {
                                Console.WriteLine(serverResponse);
                            }
                        }
                        break;
                    case "-h9":
                        if (location == null)
                        {
                            sw.WriteLine("GET /" + username);
                            sw.Flush();
                            string serverResponse = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if (serverResponse == "HTTP/0.9 404 NotFound\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(serverResponse);
                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                            }

                        }
                        else
                        {
                            sw.WriteLine("PUT /" + username + "\r\n\r\n" + location);
                            sw.Flush();

                            string serverResponse = sr.ReadLine();
                            string[] sections = serverResponse.Split(new char[] { ' ' }, 3);
                            serverResponse = sections[2];
                            if(serverResponse == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                            }
                            else
                            {
                                Console.WriteLine(serverResponse);
                            }
                        }
                        break;
                    case "whois":
                        if (location == null)
                        {
                            sw.WriteLine(username);
                            sw.Flush();
                            response = sr.ReadToEnd();
                            try
                            {

                                if (response != null)
                                {
                                    Console.WriteLine(username + " is " + response);
                                }
                                else if (response.Substring(0, 6) == "ERROR")
                                {
                                    Console.WriteLine(response);
                                }

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(args[0] + " is " + sr.ReadToEnd());
                            }


                        }
                        else 
                        {
                            sw.WriteLine(username + " " + location);
                            sw.Flush();
                            response = sr.ReadToEnd();

                            if (response == "OK\r\n")
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                            }
                            else if (response.Substring(0, 6) == "ERROR")
                            {
                                Console.WriteLine(response);
                            }
                            else
                            {
                                Console.WriteLine(username + " is " + response);
                            }

                        }
                        //else
                        //{
                        //    Console.WriteLine("ERROR");
                        //}
                        break;

                }
                client.Close();


                //   if (args.Length == 1)
                //   {
                //       sw.WriteLine(args[0]);
                //       sw.Flush();
                //       response = sr.ReadToEnd();
                //       try
                //       {
                //
                //           if (response != null)
                //           {
                //               Console.WriteLine(args[0] + " is " + response);
                //           }
                //           else if (response.Substring(0, 6) == "ERROR")
                //           {
                //             Console.WriteLine(response);
                //           }
                //
                //       }
                //       catch (Exception e)
                //       {
                //           Console.WriteLine(args[0] + " is " + sr.ReadToEnd());
                //       }
                //
                //
                //   }
                //   else if (args.Length == 2)
                //   {
                //       sw.WriteLine(args[0] + " " + args[1]);
                //       sw.Flush();
                //       response = sr.ReadToEnd();
                //       
                //           if (response == "OK\r\n")
                //           {
                //               Console.WriteLine(args[0] + " location changed to be " + args[1]);
                //           }
                //           else if (response.Substring(0, 6) == "ERROR")
                //           {
                //               Console.WriteLine(response);
                //           }
                //           else
                //           {
                //               Console.WriteLine(args[0] + " is " + response);
                //           }
                //       
                //   }
                //   else
                //   {
                //       Console.WriteLine("ERROR");
                //   }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
