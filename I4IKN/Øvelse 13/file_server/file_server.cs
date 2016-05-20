using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{

		private const int BUFSIZE = 1000;
		Transport transport;
		UTF8Encoding encoder;

		private file_server ()
		{
			transport = new Transport(BUFSIZE);
				encoder = new UTF8Encoding();

			try
			{
				var fileRequest = ReadString(transport,BUFSIZE);
				Console.WriteLine("Requested file: {0}", fileRequest);

				sendFile(fileRequest,fileRequest.Length,transport);
			}
			catch (Exception e) 
			{

			}
		}

	
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			fileSize = LIB.check_File_Exists (fileName);
			transport.send(encoder.GetBytes (fileName), (int)fileSize);

			if (fileSize == 0)
				return;

			FileStream f = File.Open (fileName, FileMode.Open);

			byte[] b = new byte[BUFSIZE];

			for (int i = 0; i < fileSize; i += BUFSIZE) 
			{
				int size = f.Read (b, 0, BUFSIZE);
				Console.WriteLine ("Size: {0}", size);

				transport.send (b, size);
			}
		}

		public static void Main (string[] args)
		{
			Console.WriteLine ("Server is starting...");
			new file_server();
		}

		internal string ReadString(Transport t, int buffersize)
		{
			byte[] bytes = new byte[buffersize];
			int size = t.receive(ref bytes);
			string returnstring = "";

			for (int i = 0; i < size; i++)
			{
				returnstring += (char)bytes[i];
			}

			return returnstring;
		}
	}
}