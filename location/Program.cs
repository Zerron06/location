using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;

namespace location
{
    public class Whois
    {

        public static string serverResponse;
        public static string debugResponse;
        static public void Main(string[] args)
        {
            bool debug = false;
            string serverAddress = "whois.net.dcs.hull.ac.uk";
            int serverPort = 43;
            string protocol = "whois", username=null, location=null;
            int timeout = 1000;
            try
            {
                if(args.Length == 0)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new ClientForm());
                }
                else
                {


                    for (int i = 0; i < args.Length; ++i)
                    {
                        switch (args[i])
                        {
                            case "-h":
                                try { serverAddress = args[++i]; }
                                catch
                                {
                                    Console.WriteLine("ERROR: No server given");
                                }

                                break;
                            case "-p":
                                try { serverPort = int.Parse(args[++i]); }
                                catch
                                {
                                    Console.WriteLine("ERROR: No port given");
                                }
                                break;
                            case "-h9":
                                protocol = args[i];
                                break;
                            case "-h0":
                                protocol = args[i];
                                break;
                            case "-h1":
                                protocol = args[i];
                                break;
                            case "-d":
                                debug = true;
                                break;
                            case "-t":
                                try
                                {
                                    timeout = int.Parse(args[++i]);
                                }
                                catch { }
                                break;
                            default:
                                if (username == null) { username = args[i]; }

                                else if (location == null) { location = args[i]; }

                                // else
                                // {
                                //     Console.WriteLine("ERROR: Too many arguments " + args[i]);
                                // }
                                break;
                        }
                    }
                }

                if (username == null) { Console.WriteLine("ERROR: No username"); serverResponse = "No user name"; }
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
                            sw.WriteLine($"GET / name ={username} HTTP/1.1\r\nHost: { serverAddress}\r\n");
                            sw.Flush();

                            string line = sr.ReadLine().Trim();
                            if (line == "HTTP/1.1 404 Not Found")
                            {
                                Console.WriteLine(line + "\r\nContent-Type: text/plain\r\n\r\n");//not found message
                                serverResponse = line + "\r\nContent-Type: text/plain\r\n\r\n";
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
                                serverResponse = username + " is ";
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
                            string responses = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if(responses == "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;

                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                                serverResponse = username + " is " + location;
                            }
                        }
                        else
                        {
                            string combineString = $"name={username}&location={location}";
                            int contentLength = combineString.Length + serverAddress.Length + 41;
                            sw.WriteLine($"POST / HTTP/1.1\r\nHost: {serverAddress}\r\nContent-Length: {combineString.Length}\r\n\r\n" + combineString);
                            sw.Flush();
                            string responses = sr.ReadToEnd();
                            string[] sections = serverResponse.Split(new char[] { ' ' }, 3);
                            responses = sections[2];
                            if (responses == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                                serverResponse = username + " location changed to be" + location;
                            }
                            else
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;
                            }
                        }
                        break;
                    case "-h0":
                        if (location == null)
                        {
                            sw.WriteLine($"GET /?{username} HTTP/1.0\r\n");
                            sw.Flush();
                            string responses = sr.ReadToEnd();
                            string[] sections = responses.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if (responses == "HTTP/1.0 404 Not Found\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;

                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                                serverResponse = username + " is " + location;
                            }
                        }

                        else
                        {

                            sw.WriteLine($"POST /{username} HTTP/1.0\r\nContent-Length: {location.Length}\r\n\r\n{location}");
                            sw.Flush();
                            string responses = sr.ReadLine();
                            string[] sections = responses.Split(new char[] { ' ' }, 3);
                            responses = sections[2];

                            if(responses == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                                serverResponse = username + " location changed to be " + location;

                            }
                            else
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;
                            }
                        }
                        break;
                    case "-h9":
                        if (location == null)
                        {
                            sw.WriteLine("GET /" + username);
                            sw.Flush();
                            string responses = sr.ReadToEnd();
                            string[] sections = responses.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            location = sections[3];

                            if (serverResponse == "HTTP/0.9 404 NotFound\r\nContent-Type: text/plain\r\n\r\n")
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;
                            }
                            else
                            {
                                Console.WriteLine(username + " is " + location);
                                serverResponse = username + " is " + location;
                            }

                        }
                        else
                        {
                            sw.WriteLine("PUT /" + username + "\r\n\r\n" + location);
                            sw.Flush();

                            string responses = sr.ReadLine();
                            string[] sections = responses.Split(new char[] { ' ' }, 3);
                            responses = sections[2];
                            if(serverResponse == sections[2])
                            {
                                Console.WriteLine(username + " location changed to be " + location);
                                serverResponse = username + " location changed to be " + location;
                            }
                            else
                            {
                                Console.WriteLine(responses);
                                serverResponse = responses;
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
                                    serverResponse = username + " is " + response;
                                }
                                else if (response.Substring(0, 6) == "ERROR")
                                {
                                    Console.WriteLine(response);
                                    serverResponse = response;
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
                                serverResponse = username + " location changed to be " + location;
                            }
                            else if (response.Substring(0, 6) == "ERROR")
                            {
                                Console.WriteLine(response);
                                serverResponse = response;
                            }
                            else
                            {
                                Console.WriteLine(username + " is " + response);
                                serverResponse = username + " is " + response;
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
