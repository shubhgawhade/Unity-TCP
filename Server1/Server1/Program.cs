using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args) {

        int backlog = -1, port = 7777;

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverSocket.ReceiveTimeout = -1;

        // Start listening.
        try {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(backlog);
        }
        catch (Exception) {
            Console.WriteLine("Listening failed!");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Start listening...");

        while(true) {
            Socket clientSocket = serverSocket.Accept();
            new System.Threading.Thread(() => {
                try { Process(clientSocket); } catch (Exception ex) { Console.WriteLine("Client connection processing error: " + ex.Message); }
            }).Start();
        }

        //Console.WriteLine("Press any key for exit...");
        //Console.ReadKey();
    }
    
    static void Process(Socket client) {

        Console.WriteLine("Incoming connection from " + client.RemoteEndPoint);

        // const int maxMessageSize = 1024;
        // byte[] response;
        // int received;
        
        while (true) {

            // Send message to the client:
            //Console.Write("Server: ");
            //client.Send(Encoding.ASCII.GetBytes(Console.ReadLine()));
            //Console.WriteLine();

            Byte[] bytes = new Byte[1];
            string len = "";
            int i = 0;
            for (int j = 0; j < 4; j++)
            {
                i = client.Receive(bytes, bytes.Length, 0);
                // print(bytes[0]);
                                
                if (i == 0)
                {
                    Console.WriteLine("Client closed connection!");
                    return;
                }

                string currentByte = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                if (currentByte == "|")
                {
                    break;
                }
                                
                len += currentByte;
            }

            Console.WriteLine(len);
            
            bytes = new Byte[int.Parse(len)];
            i = client.Receive(bytes, bytes.Length, 0);
            
            if (i > 0)
            {
                // Translate data bytes to a ASCII string.
                len = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine($"Received: {len}");
                
                // Process the data sent by the client.
                len = len.ToUpper() + "1";// $" {DateTime.Now}";
                
                string bytesToSend = len.Length + "|";
                len = len.Insert(0, bytesToSend);
                bytes = new Byte[len.Length];
                bytes = System.Text.Encoding.ASCII.GetBytes(len);
                
                // Send back a response.
                client.Send(bytes, bytes.Length, 0);
                Console.WriteLine($"Sent: {len}");
                len = "";
            }

            // Receive message from the server:
            // response = new byte[maxMessageSize];
            // received = client.Receive(response);
            // if (received == 0) {
            //     Console.WriteLine("Client closed connection!");
            //     return;
            // }
            //
            // List<byte> respBytesList = new List<byte>(response);
            // respBytesList.RemoveRange(received, maxMessageSize - received); // truncate zero end
            // Console.WriteLine("Client (" + client.RemoteEndPoint + "+: " + Encoding.ASCII.GetString(respBytesList.ToArray()));
        }
    }
}