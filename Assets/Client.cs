using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;
    private GameObject localPlayer;
    
    //static string message = "hello!";
    static Int32 port = 7777;

    // Prefer a using declaration to ensure the instance is Disposed later.
    static TcpClient client;

    // Translate the passed message into ASCII and store it as a Byte array.
    private Byte[] bytes = new Byte[256];
    private string data = "hello";

    // Get a client stream for reading and writing.
    NetworkStream stream;
    
    // Start is called before the first frame update
    async void Start()
    {
        await StartClient();
    }

    // private bool timeout;
    // private float time;
    private async void Update()
    {
        // time += Time.deltaTime;
        //
        // if (time > 0.5f)
        // {
        //     timeout = !timeout;
        // }
        
        if (Input.inputString != "" && Input.inputString != "\r")// && timeout)
        {
            // time = 0;
            // timeout = true;
            // if (data.Length > 10)
            {
                // data = "";
            }
            data += Input.inputString;
            // if (data != "")
        }
        
        if(Input.GetKeyDown(KeyCode.Return))
        {
            await SendMessage();
            await ReceiveMessage();
        }
    }

    private async Task SendMessage()
    {
        string bytesToSend = data.Length + "|";
        data = data.Insert(0, bytesToSend);
        bytes = new Byte[data.Length];
        bytes = System.Text.Encoding.ASCII.GetBytes(data);
        await stream.WriteAsync(bytes, 0, bytes.Length);
        print($"Sent: {data}");
    }

    private async Task ReceiveMessage()
    {
        bytes = new Byte[1];
        string len = "";
        int i;
        for (int j = 0; j < 4; j++)
        {
            i = await stream.ReadAsync(bytes, 0, 1);
            
            if (i == 0)
            {
                print($"DISCONNECTED");
                break;
            }

            if (i > 0)
            {
                string currentByte = System.Text.Encoding.ASCII.GetString(bytes, 0, 1);
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
        }
        
        //  int bytesRecd = await stream.ReadAsync(bytes, 0, bytes.Length);
        //
        // // Translate data bytes to a ASCII string.
        // data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRecd);
        // print($"Received: {data}");
        data = "";

        // int bytesRecd = await stream.ReadAsync(bytes, 0, bytes.Length);
        //         
        // // Translate data bytes to a ASCII string.
        // data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRecd);
        // print($"Received: {data}");
    }
    
    private async Task StartClient()
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.

            client = new TcpClient("127.0.0.1", port);
            
            stream = client.GetStream();
            await SendMessage();
            await ReceiveMessage();
            
            // bytes = System.Text.Encoding.ASCII.GetBytes(data);
            //
            // await stream.WriteAsync(bytes, 0, bytes.Length);
            // print($"Sent: {data}");

            //int i;
            //while((i = await stream.ReadAsync(bytes, 0, bytes.Length))!=0) 
            {
                // int bytesRecd = await stream.ReadAsync(bytes, 0, bytes.Length);
                //
                // // Translate data bytes to a ASCII string.
                // data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRecd);
                // print($"Received: {data}");
                
                localPlayer = Instantiate(spawnPrefab);
            }
            
            // Send the message to the connected TcpServer.
            //this await stream.WriteAsync(data, 0, data.Length);

            //this print($"Sent: {message}");

            //this // Receive the server response.

            //this // Buffer to store the response bytes.
            //this data = new Byte[256];

            //this // String to store the response ASCII representation.
            //this String responseData = String.Empty;

            //this // Read the first batch of the TcpServer response bytes.
            //this Int32 bytes = await stream.ReadAsync(data, 0, data.Length);
            //this responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //this print($"Received: {responseData}");

            //this localPlayer = Instantiate(spawnPrefab);

            // Explicit close is not necessary since TcpClient.Dispose() will be
            // called automatically.
            // stream.Close();
            // client.Close();
        }
        catch (ArgumentNullException e)
        {
            print($"ArgumentNullException: {e}");
        }
        catch (SocketException e)
        {
            print($"SocketException: {e}");
        }
    }

    private void OnApplicationQuit()
    {
        client.Close();
        Destroy(localPlayer);
    }
}
