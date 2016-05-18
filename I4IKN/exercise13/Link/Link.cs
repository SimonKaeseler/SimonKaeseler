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

			serialPort.ReadTimeout = 500;
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
			List<byte> package = new List<byte> ();
			package.Add ((byte)'A');

			for (int i = 0; i < size; i++) 
			{
				if (buf [i] == (byte)'A') {
					package.Add ((byte)'B');
					package.Add ((byte)'C');
				} else if (buf [i] == (byte)'B') {
					package.Add ((byte)'C');
					package.Add ((byte)'D');
				} else
					package.Add (buf [i]);
			}

			//ESC char
			package.Add ((byte)'A');
			string toPrint = "";
			foreach (byte b in package) 
			{
				toPrint += (char)b;
			}
			Console.WriteLine("Sending package {0} with checksum {1};{2}",toPrint,buf[0],buf[1]);
			serialPort.Write (package.ToArray (), 0, package.Count);
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
