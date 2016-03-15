using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
    class Client
    {
        /// <summary>
        /// The PORT.
        /// </summary>
        const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE.
        /// </summary>
        const int BUFSIZE = 1000;

        private readonly TcpClient client;
        private NetworkStream serverStream;
        private string ip,path;

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private Client(string[] args)
        {
            ip = args[0];
            path = args[1];

            //Creating socket for client and connecting
            client = new TcpClient();
            Console.WriteLine("Connecting");
            client.Connect(ip, PORT);
            Console.WriteLine("Connected");

            //Fetching stream and send items to server
            serverStream = client.GetStream();
            LIB.writeTextTCP(serverStream,path);
            Console.WriteLine("items sent");
            //Recieves the file from server
            ReceiveFile(LIB.extractFileName(path),serverStream);
        }

        /// <summary>
        /// Receives the file.
        /// </summary>
        /// <param name='fileName'>
        /// File name.
        /// </param>
        /// <param name='io'>
        /// Network stream for reading from the server
        /// </param>
        private void ReceiveFile(String fileName, NetworkStream io)
        {
            Console.WriteLine("entering reciever");
            long filesize = LIB.getFileSizeTCP(io);
            if (filesize != 0)
            {
                
                Byte[] inStream = new byte[BUFSIZE];

                var file = File.Create(fileName);
                var cntr = 0;

                while (cntr < filesize)
                {
                    var streamSize = serverStream.Read(inStream, 0, BUFSIZE);
                    file.Write(inStream, 0, streamSize);
                    Console.WriteLine(streamSize);
                    cntr += streamSize;
                }

                io.Close();
                file.Close();
            }
            else
            {
                throw new ArgumentException("File does not excist");
            }
            
        }

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments.
        /// </param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Client starts...");
            new Client(args);
        }
    }
}
