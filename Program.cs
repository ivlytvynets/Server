using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Sevrer
{
    class Program
    {
        static readonly int _port = 1038;
        static readonly string _serverIp = "127.0.0.1";
        static readonly string _logPath = "E:\\server\\server_log.txt";


        static void Send(string message, Socket handler)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            handler.Send(data);
        }

        static void Log(string data)
        {
            using (StreamWriter writer = File.AppendText(_logPath))
            {
                writer.WriteLine(data);
            }
        }

        static void Main(string[] args)
        {
            File.WriteAllText(_logPath, String.Empty);

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(_serverIp), _port);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string password = "pw";

            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                string serverInitInfo = "Server initialized at " + DateTime.Now.ToString();
                Console.WriteLine(serverInitInfo);
                Log(serverInitInfo);

                while (true)
                {
                    Socket handler = listenSocket.Accept();
                    StringBuilder builder = new StringBuilder();

                    int bytes = 0;
                    byte[] data = new byte[256];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    string clientMessage = builder.ToString().Substring(1);
                    string logMessage = "Client message at " + DateTime.Now.ToString() + ": " + clientMessage;
                    

                    Console.WriteLine(logMessage);
                    Log(logMessage);

                    if (clientMessage.Equals("who"))
                    {
                        string info = "Info sent: Ivan Lytvynets, K-26, v.13 \"Password Picker\"";
                        Send(info, handler);
                        Log(info);
                    }
                    else if (clientMessage.Equals("connect"))
                    {
                        Send(password.Length.ToString(), handler);
                        Log("Connection established successfully at " + DateTime.Now.ToString());
                    }
                    else
                    {
                        if (clientMessage.Equals(password))
                        {
                            Send("Password is correct!", handler);
                            Log("Password cracked successfully at " + DateTime.Now.ToString());
                            break;
                        }
                        else
                        {
                            Send("Password is incorrect!", handler);
                        }
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
