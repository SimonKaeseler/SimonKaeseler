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

      

        /// <summary>
        /// Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        /// <param name='args'>
        /// The command-line arguments. First ip-adress of the server. Second the filename
        /// </param>
        private Client(string[] args)
        {
			//The arguments that user calls the program  with is args[0] = IP and args[1] file (includingpath)
			var IP = args [0];
			var file = args [1];

			//Creating the client socket
			Console.WriteLine ("Trying to connect...");
			TcpClient client = new TcpClient ();
			client.Connect (IP, PORT);
			Console.WriteLine ("Connected to server!");

			//Getting the stream
			var serverStream = client.GetStream ();
			Console.WriteLine ("Stream caught...");

			//Requesting file from the serverstream
			Console.WriteLine("Requesting file from server...");
			LIB.writeTextTCP (serverStream, file);

			//Recieving
			Console.WriteLine ("Recieving...");
			var filename = LIB.extractFileName (file);
			ReceiveFile (file, serverStream);
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
			//Getting the size of the requested file
			var fileLength = LIB.getFileSizeTCP (io);

			if (fileLength == 0) 
			{
				Console.WriteLine ("Filesize was 0, file doesnt excist...");
				return;
			}

			//Using this bytesize to write the file byte by byte
			var recievedBytes = new byte[BUFSIZE];

			//Creating a new file to save the recieved file in
			var file = File.Create (fileName);
			Console.WriteLine ("Creating new file...");
			int i = 0;

			//Writing in the file
			Console.WriteLine ("Writing new file...");
			while (i < fileLength) 
			{
				int size = io.Read (recievedBytes, 0, BUFSIZE);
				Console.WriteLine ("{0} bytes writtin in file...",size);
				file.Write (recievedBytes, 0, size);
				i += size;
			}
			Console.WriteLine ("File is completed at {0} bytes", file.Length);

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
            Console.WriteLine("Client starts...");
            new Client(args);
        }
    }
}
