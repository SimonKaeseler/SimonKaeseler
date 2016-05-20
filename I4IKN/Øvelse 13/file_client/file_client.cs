using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
	
		const int BUFSIZE = 1000;
		Transport transport;
		UTF8Encoding encoder;

	    private file_client(String[] args)
	    {
			transport = new Transport (BUFSIZE);
			encoder = new UTF8Encoding();
			var filePath = encoder.GetBytes(args [0]);

			try
			{
				transport.send(filePath,filePath.Length);
			}
			catch(Exception e) 
			{
				Console.WriteLine ("Server not responding...");
			}
			string fP = "";
			for (int i = 0; i < filePath.Length; i++) 
			{
				fP += filePath [i];
			}
			receiveFile (fP, transport);
	    }


		private void receiveFile (String fileName, Transport transport)
		{
			long fileLength = long.Parse(ReadString(transport,BUFSIZE));

			if (fileLength == 0) 
			{
				Console.WriteLine ("File does not exist");
				return;
			}

			byte[] b = new byte[BUFSIZE];
			FileStream f = File.Create (fileName);

			int i = 0;
			while (i < fileLength) 
			{
				int size = transport.receive(ref b);
				//Console.WriteLine (size);
				f.Write (b, 0, size);
				i += size;
			}

			f.Close ();
		}

	
		public static void Main (string[] args)
		{
			Console.WriteLine("Client starting - requesting {0}", args[0].ToString());
			new file_client(args);
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