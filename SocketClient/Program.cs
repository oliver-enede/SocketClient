using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket client = null;
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ipaddr = null;

            try
            {
                Console.WriteLine("*** Welcome to Socket Client Starter ***");
                Console.WriteLine("Please Type a Valid Server IP Address and Press Enter: ");

                string strIPAddress = Console.ReadLine();

                Console.WriteLine("Please Supply a Valid Port Number 0-65535 and Press Enter: ");

                string strPortInput = Console.ReadLine();
                int nPortInput = 0;

                if (string.IsNullOrWhiteSpace(strIPAddress)) strIPAddress = "127.0.0.1";
                if (string.IsNullOrWhiteSpace(strPortInput)) strPortInput = "25000";

                if (!IPAddress.TryParse(strIPAddress, out ipaddr))
                {
                    Console.WriteLine("Invalid server IP supplied.");
                    return;
                }
                if (!int.TryParse(strPortInput.Trim(), out nPortInput))
                {
                    Console.WriteLine("Invalid port number supplied, return.");
                    return;
                }
                if (nPortInput <= 0 || nPortInput > 65535)
                {
                    Console.WriteLine("Port number must be between 0 and 65535.");
                    return;
                }

                Console.WriteLine($"IPAddress: {ipaddr}-Port: {nPortInput}");
                client.Connect(ipaddr, nPortInput);

                Console.WriteLine("Connected to the server, type text and press enter to send it to the server, type <EXIT> to close.");

                Thread receiveThread = new Thread(ReceiveData);
                receiveThread.Start(client);

                string inputCommand = string.Empty;

                while (true)
                {
                    inputCommand = Console.ReadLine();

                    if (inputCommand.Equals("<EXIT>", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    byte[] buffSend = Encoding.ASCII.GetBytes(inputCommand);

                    client.Send(buffSend);
                }

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception excp)
            {
                Console.WriteLine(excp.ToString());
            }
            finally
            {
                if (client != null)
                {
                    if (client.Connected)
                    {
                        client.Shutdown(SocketShutdown.Both);
                    }

                    client.Close();
                    client.Dispose();
                }
            }
            Console.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }

        static void ReceiveData(object obj)
        {
            Socket clientSocket = (Socket)obj;
            byte[] buffReceived = new byte[128];

            try
            {
                while (true)
                {
                    int nRecv = clientSocket.Receive(buffReceived);
                    if (nRecv == 0) break;
                    Console.WriteLine("Data received: {0}", Encoding.ASCII.GetString(buffReceived, 0, nRecv));
                }
            }
            catch (Exception)
            {
                // Handle any exceptions
            }
        }
    }
}