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
		public Link (int bufsize)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(bufsize*2)];

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
		public void Send (byte[] buf, int size)
		{
	    	// TO DO Your own code
			//Convert byte array to strings, send each string, terminate with /n
			//OBS: A 
			List<byte> bytelist= new List<byte>();
			bytelist.Add ((byte)'A');

			for (int i = 0; i < size; i++) 
			{
				if (buf [i] == (byte)'A') 
				{
					bytelist.Add ((byte)'B');
					bytelist.Add ((byte)'C');
				}
				else if (buf [i] == (byte)'B')
				{
					bytelist.Add ((byte)'B');
					bytelist.Add ((byte)'D');
				}
				else
					bytelist.Add (buf [i]);
			}

			bytelist.Add ((byte)'A');

			var t = bytelist.ToArray ();
			serialPort.Write (t, 0, bytelist.Count);

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
//			for (int i = 2; i < charsToSend.Length; i++) 
//			{
//				package += charsToSend [i];
//			}
//
//			Console.WriteLine (package);
//		    if (package != null) serialPort.Write (package);
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
		public int Receive (ref byte[] buf)
		{
			Console.WriteLine ("Link.recieve");

			while (serialPort.ReadByte () != (byte)'A') {
			}

			int readBytes = 0;
			byte c;
			while ((c = (byte)serialPort.ReadByte ()) != (byte)'A') 
			{
				buffer [readBytes] = c;
				readBytes++;
			}

			int returnBufSize = 0;
			for (int i = 0; i < readBytes; i++, returnBufSize++) 
			{
				if (buffer [i] == 'B') 
				{
					i++;
					if (buffer [i] == 'C')
						buf [returnBufSize] = (byte)'A';
					else if (buffer [i] == 'D')
						buf [returnBufSize] = (byte)'B';
				} else 
				{
					buf [returnBufSize] = buffer [i];
				}
			}

			return returnBufSize;
		}
	}
}
