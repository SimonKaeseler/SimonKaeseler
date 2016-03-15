using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace tcp
{
    class Server
    {
        /// <summary>
        /// The PORT
        /// </summary>
        public const int PORT = 9000;
        /// <summary>
        /// The BUFSIZE
        /// </summary>
        public const int BUFSIZE = 1000;

        public int fileLength;
     
        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// Opretter en socket.
        /// Venter på en connect fra en klient.
        /// Modtager filnavn
        /// Finder filstørrelsen
        /// Kalder metoden sendFile
        /// Lukker socketen og programmet
        /// </summary>
        private Server()
        {
            var serverSocket = new TcpListener(IPAddress.Any,PORT);
            var client = new TcpClient();
            var reader = client.GetStream();
            serverSocket.Start();
            Console.WriteLine("Server is started...");

            try
            {
                //Returns a TCPClient and saves it in "client"
                client = serverSocket.AcceptTcpClient();
                Console.WriteLine("Connection established...");
                reader = client.GetStream();
                
                string data = LIB.readTextTCP(reader);

                Console.WriteLine("Received: {0}", data);
                SendFile(data, fileLength, reader);
            }
            catch (Exception)
            {
                throw new SocketException();
            }
            
            client.Close();
            serverSocket.Stop();
        }

        /// Sends the file.
        /// </summary>
        /// <param name='fileName'>
        /// The filename.
        /// </param>
        /// <param name='fileSize'>
        /// The filesize.
        /// </param>
        /// <param name='io'>
        /// Network stream for writing to the client.
        /// </param>
        private void SendFile(String fileName, int fileSize, NetworkStream io)
        {
            //The filecheck returns the size of the file, if it excists
            long filesize = LIB.check_File_Exists(fileName);
            if (filesize != 0)
            {
                var file = File.Open(fileName, FileMode.Open);
                byte[] bytes = new byte[BUFSIZE];

                //iterates through the fie while it reads 1000 bits at a time.
                for (int i = 0; i < fileSize; i += BUFSIZE)
                {
                    //Reads the file "bytes" with zero offset 
                    var size = file.Read(bytes, 0, BUFSIZE);
                    io.Write(bytes,0,size);
                }
                file.Close();
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
            Console.WriteLine("Server starts...");

           
            new Server();
        }
    }
}

