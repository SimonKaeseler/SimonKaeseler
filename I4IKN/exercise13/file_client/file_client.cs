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

			//byte[] buffer = new byte[]{ (byte)'A', (byte)'B', (byte)'C' };
			//_transport.send(buffer, buffer.Length);
			byte[] filename = Encoding.UTF8.GetBytes (args[0]);
			_transport.send (filename, filename.Length);

			receiveFile (args[0], _transport);
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
			if (fileName.Length == 0) 
			{
				Console.WriteLine ("Filelength was 0, doesnt excist...");
				return;
			}

			Console.WriteLine ("Creating file...");
			var fileToRecieve = File.Create (fileName);

			byte[] bytesToRecieve = new byte[BUFSIZE];
			int size = 1;

			while (size > 0) 
			{
				size = transport.receive (ref bytesToRecieve);
				fileToRecieve.Write (bytesToRecieve, 0, size);
				Console.WriteLine ("{0} bytes written to file", size);
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
			new file_client(args);

				
		}
	}
}