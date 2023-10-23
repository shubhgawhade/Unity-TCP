using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class Server : MonoBehaviour
{
    private static int port = 7777;
    TcpListener tcpListener;
    private TcpClient tcpClient;

    public Dictionary<string, TcpClient> clients = new();
    // public TcpClient[] clients = new TcpClient[3];
    public int numOfClients = 0;
    
    private void Start()
    {
        StartListener();
        WaitForConns();
    }

    // Buffer for reading data
    Byte[] bytes = new Byte[256];
    string data;
    
    private void StartListener()
    {
        tcpListener = TcpListener.Create(port);
        tcpListener.Start();
    }

    async void WaitForConns()
    {
        while (true)
        {
            print("[Server] waiting for clients...");
                
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            
            string name = $"{Random.Range(0,1000)}";
            // switch (numOfClients)
            // {
            //     case 0:
            //
            //         name = "AA";
            //             
            //         break;
            //     
            //     case 1:
            //
            //         name = "BB";
            //
            //         break;
            //     
            //     case 2:
            //         
            //         name = "CC";
            //         
            //         break;
            // }
            clients.Add(name, tcpClient);
            numOfClients++;
            print($"{name} {numOfClients}");

            foreach (var client in clients)
            {
                WaitForMessages(client.Key, client.Value);
            }

        }
    }

    async void WaitForMessages(string clientName, TcpClient client)
    {
        while (numOfClients > 0)
        {
            try
            {
                // for (int j = 0; j < numOfClients;)
                // {
                //     switch (j)
                //     {
                //         case 0:
                //
                //             name = "AA";
                //
                //             break;
                //
                //         case 1:
                //
                //             name = "BB";
                //
                //             break;
                //
                //         case 2:
                //
                //             name = "CC";
                //
                //             break;
                //     }
                //
                //     if (clients.TryGetValue(name, out TcpClient client))
                //     {
                //         print(name);
                        NetworkStream stream = client.GetStream();
                        
                        // if (stream.DataAvailable)
                        {
                            // Get a stream object for reading and writing
                            // NetworkStream stream = clients.GetStream();

                            // Byte[] dataLength = new byte[1];
                            bytes = new Byte[1];
                            string len = "";
                            int i;
                            for (int j = 0; j < 4; j++)
                            {
                                i = await stream.ReadAsync(bytes, 0, bytes.Length);
                                // print(bytes[0]);
                                
                                if (i == 0)
                                {
                                    // numOfClients--;
                                    clients.Remove(clientName);
                                    print($"REMOVED : {clientName} \n {clientName} connection lost"); // + clients.TryGetValue(clientName, out TcpClient val));
                                    numOfClients = clients.Count;
                                    // print("[Server] client connection lost");
                                    break;
                                }

                                if (i > 0)
                                {
                                    string currentByte = System.Text.Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                                    if (currentByte == "|")
                                    {
                                        break;
                                    }
                                    
                                    len += currentByte;
                                }
                            }
                            
                            print(len);

                            bytes = new Byte[int.Parse(len)];
                            i = await stream.ReadAsync(bytes, 0, bytes.Length);
                            print(i);
                            
                            // Loop to receive all the data sent by the client.
                            // while ((i = await stream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                            if (i > 0)
                            {
                                // Translate data bytes to a ASCII string.
                                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                                print($"Received: {data}");
                
                                // Process the data sent by the client.
                                data = data.ToUpper() + "1";// $" {DateTime.Now}";
                                // data = data.Insert(0, data.Length + "|");
                
                                string bytesToSend = data.Length + "|";
                                data = data.Insert(0, bytesToSend);
                                bytes = new Byte[data.Length];
                                bytes = System.Text.Encoding.ASCII.GetBytes(data);
                
                                // Send back a response.
                                await stream.WriteAsync(bytes, 0, bytes.Length);
                                print($"Sent: {data}");
                                data = "";
                            }
                        }   
                    

                    // ++j;
                // }
            }
            catch (SocketException e)
            {
                print($"SocketException: {e}");
            }
            
            //print("[Server] Client has connected");
            //using (var networkStream = tcpClient.GetStream())
            //using (var reader = new StreamReader(networkStream))
            //using (var writer = new StreamWriter(networkStream) { AutoFlush = false })
            //{
            //    var buffer = new byte[4096];
            //    print("[Server] Reading from client");
            //    string request = await reader.ReadLineAsync();
            //    // string.Format(string.Format("[Server] Client wrote '{0}'", request));
            //    print($"[Server] Client wrote {request}");
            //
            //    await writer.WriteLineAsync($"[Server] to Client {request}");
            //    //for (int i = 0; i < 5; i++)
            //    //{
            //    //    await writer.WriteLineAsync("I am the server! HAHAHA!");
            //    //    Console.WriteLine("[Server] Response has been written");
            //    //    await Task.Delay(TimeSpan.FromSeconds(1));
            //    //}
            //}
        }
    }
    
    void StartServer()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 7777;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Enter the listening loop.
            while(true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                using TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("Received: {0}", data);

                    // Process the data sent by the client.
                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }
            }
        }
        catch(SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server.Stop();
        }

        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    private void OnApplicationQuit()
    {
        // tcpClient.Close();
        tcpListener.Stop();
    }
}