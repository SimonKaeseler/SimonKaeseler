using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			// TO DO Your own code
			Transport transport = new Transport (BUFSIZE);
			byte[] fileToSend = new byte[BUFSIZE];

			int filesize = transport.receive (ref fileToSend);

			string fileName = fileToSend.ToString ();

			while (true) 
			{
				sendFile (fileName, filesize, transport);
			}
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			long fileLength = LIB.check_File_Exists(fileName);


			if (fileLength == 0) 
			{
				Console.WriteLine ("Filelength was 0, does not excist");
				return;
			}

			FileStream file = File.Open (fileName, FileMode.Open);

			byte[] data = new byte[BUFSIZE];
			for (int i = 0; i < fileSize; i += BUFSIZE) 
			{
				int size = file.Read (data, 0, BUFSIZE);
				Console.WriteLine (size);
				transport.send (data, size);
			}
			file.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Fileserver starting...");
			new file_server();
		}
	}
}