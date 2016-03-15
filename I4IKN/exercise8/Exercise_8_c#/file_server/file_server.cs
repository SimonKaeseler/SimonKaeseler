using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

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
		public byte[] bytes;
		public string data;

		private TcpListener serverSocket;
		private TcpClient client;
		private NetworkStream reader;
		public int RequestCount { get; private set; }

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
			bytes = new byte[BUFSIZE];
			serverSocket = new TcpListener(IPAddress.Any,PORT);
			serverSocket.Start();
			Console.WriteLine("Server is started...");

			try
			{
				client = serverSocket.AcceptTcpClient();
				Console.WriteLine("Connection established...");
			}
			catch (Exception)
			{

				throw new SocketException();
			}

			reader = client.GetStream();
			fileLength = reader.Read(bytes, 0, bytes.Length);
			sendFile(data,fileLength,reader);

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
		private void sendFile(String fileName, int fileSize, NetworkStream io)
		{
			while (fileSize != 0)
			{
				// Translate data bytes to a ASCII string.
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, fileSize);

				Console.WriteLine(String.Format("Received: {0}", data));

				// Process the data sent by the client.
				data = data.ToUpper();

				byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

				// Send back a response.
				reader.Write(msg, 0, msg.Length);
				Console.WriteLine(String.Format("Sent: {0}", data));

				fileSize = reader.Read(bytes, 0, bytes.Length);
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

