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
		private const bool running = true;
       
     
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
			TcpListener server = new TcpListener (PORT);
			TcpClient client = new TcpClient ();
			//gets set when clients gets stream
			NetworkStream tcpStream;

			server.Start ();

			while (running) 
			{
				try
				{
				
					//Get the client
					Console.WriteLine("Accepting client(s)...");
					client = server.AcceptTcpClient ();
					tcpStream = client.GetStream();

					//Read client request
					Console.WriteLine("Reading client request...");
					var clientRequest = LIB.readTextTCP(tcpStream);
					Console.WriteLine("Request from client: " + clientRequest);

					//Checking the size of the file
					var lengthOfFile = LIB.check_File_Exists (clientRequest).ToString();
					LIB.writeTextTCP (tcpStream, lengthOfFile);

					var sizeofFile = Convert.ToInt32(lengthOfFile);

					//Sending the file
					SendFile(clientRequest,sizeofFile,tcpStream);
				}
				catch(Exception execption) 
				{
					Console.WriteLine ("An error occured: {0}",execption);
				}

			}

			client.Close ();
			Console.WriteLine ("Client connection terminated...");
			server.Stop ();
			Console.WriteLine ("Serer stopped...");
			Console.ReadKey ();

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


			if (fileSize == 0) 
			{
				Console.WriteLine ("Bad request, filesize = 0");
				return;
			}


			var file = File.Open (fileName, FileMode.Open);

			byte[] data = new byte[BUFSIZE];

			//For loop, that sending 1000 bytes at at time, and prints progress
			for (int i = 0; i < fileSize; i += BUFSIZE) 
			{
				int size = file.Read (data, 0, BUFSIZE);
				Console.WriteLine (size);
				io.Write (data, 0, size);
			}

			file.Close ();
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

