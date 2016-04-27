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
			LIB.SendStringByTrans (transport, fileName);

			if (fileLength == 0)
				return;

			FileStream file = File.Open (fileName, FileMode.Open);

			byte[] data = new byte[BUFSIZE];
			for (int i = 0; i < fileLength; i += BUFSIZE) 
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
			new file_server();

			Link link = new Link (255);

			string teststring = "mongoBongo";
			byte[] b = System.Text.Encoding.UTF8.GetBytes(teststring.ToUpper());

			link.send (b, b.Length);

		}
	}
}