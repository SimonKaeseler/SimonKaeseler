using System;
using System.IO.Ports;
using System.Collections.Generic;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			serialPort.ReadTimeout = 400;
			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
//		public void send (byte[] buf, int size)
//		{
//	    	// TO DO Your own code
//			//Convert byte array to strings, send each string, terminate with /n
//			//OBS: A 
//
//			char[] charsToSend = new char[size+3];
//			int currentIndex = 0;
//
//			for (currentIndex = 0; currentIndex < size; currentIndex++) 
//			{
//				if (buf [currentIndex] == 'A') {
//					charsToSend [currentIndex] = 'B';
//					currentIndex++;
//					charsToSend [currentIndex] = 'C';
//				} else if (buf [currentIndex] == 'B') {
//					charsToSend [currentIndex] = 'B';
//					currentIndex++;
//					charsToSend [currentIndex] = 'D';
//				} else 
//				{
//					charsToSend [currentIndex] = (char)buf [currentIndex];
//				}
//			}
//
//			charsToSend[currentIndex] = 'A';
//			currentIndex ++;
//			charsToSend[currentIndex] = '\n';
//			string package = null;
//
//			foreach (char c in charsToSend) {package += c;};
//
//			Console.WriteLine (package);
//			serialPort.Write (package);
//		}
		public void send (byte[] buf, int size)
		{
			List<byte> toSend= new List<byte>();
			toSend.Add ((byte)'A');

			for (int i = 0; i < size; i++) 
			{
				if (buf [i] == (byte)'A') 
				{
					toSend.Add ((byte)'B');
					toSend.Add ((byte)'C');
				}
				else if (buf [i] == (byte)'B')
				{
					toSend.Add ((byte)'B');
					toSend.Add ((byte)'D');
				}
				else
					toSend.Add (buf [i]);
			}

			toSend.Add ((byte)'A');

			//for (int i = 0; i < size; i++) 
			//{
			//	Console.WriteLine (buf [i]);
			//}

			var t = toSend.ToArray ();
			serialPort.Write (t, 0, toSend.Count);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{

			while (serialPort.ReadByte () != (byte)'A') {
			}
			int toRead = serialPort.BytesToRead;
			byte[] bytes = new byte[toRead];

			try
			{
			serialPort.Read(bytes, 0, toRead);
		

				for (int i = 0; i < toRead; i++)
				{				
					if (bytes[i] == (byte)'B' && bytes[i+1] == (byte)'C')
					{
						buf[i] = (byte) 'A';
					}
					else if (bytes[i] == (byte) 'B' && bytes[i + 1] == (byte) 'D')
					{
						buf[i] = (byte) 'B';
					}
					else
					{
						buf [i] = bytes [i];
					}

				}
			}
			catch(TimeoutException) 
			{
				Console.WriteLine ("Read timed out - no bytes to read...");
			}
						
			return buf.Length;
		}
	}
}
