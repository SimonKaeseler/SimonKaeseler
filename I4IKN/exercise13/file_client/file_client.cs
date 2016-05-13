using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;
using Linklaget;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;


		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {	    	
			Transport _transport = new Transport (BUFSIZE);
			string filename = args[0];
			
			try{						
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
				_transport.send(encoding.GetBytes(filename), filename.Length);							
			}
			catch(TimeoutException){
				Console.WriteLine("Receive timed out");
			}

			receiveFile (LIB.extractFileName(filename), _transport);
	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
			// TO DO Your own code
			byte[] bytesForSize = new byte[BUFSIZE];
			int fsize = transport.receive(ref bytesForSize);
			string stringToSend = "";

			for (int i = 0; i < fsize; i++)
			{
				stringToSend += (char)bytesForSize[i];
			}
			
			int fileSize = int.Parse (stringToSend);
			
			if (fileSize == 0) 
			{
				Console.WriteLine ("Filelength was 0, doesnt exist...");
				return;
			}

			Console.WriteLine ("Creating file...");
			var fileToRecieve = File.Create (fileName);

			var bytesToRecieve = new byte[BUFSIZE];
			int index = 0;

			while (index < fileSize) 
			{
				int size = transport.receive (ref bytesToRecieve);
				fileToRecieve.Write (bytesToRecieve, 0, size);
				Console.WriteLine ("{0} bytes written to file", size);
				index += size;
			}

			fileToRecieve.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starting...");
			new file_client(args);
				
		}
	}
}