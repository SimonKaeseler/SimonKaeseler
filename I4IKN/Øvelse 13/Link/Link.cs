using System;
using System.IO.Ports;


namespace Linklaget
{
	public class Link
	{
		const byte DELIMITER = (byte)'A';
		private byte[] buffer;
		SerialPort serialPort;

		public Link (int BUFSIZE)
		{
			// Create a new SerialPort object with default settings.
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);

			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			serialPort.ReadTimeout = 500;
			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		public void send (byte[] buf, int size)
		{
			int currentIndex = 0;

			buffer [currentIndex] = (byte)'A';
			currentIndex++;
			for (int i = 0; i < size; i++) 
			{
				if (buf [i] == (byte)'A') {
					buffer [currentIndex] = (byte)'B';
					currentIndex++;
					buffer [currentIndex] = (byte)'C';
				} else if (buf [i] == (byte)'B') {
					buffer [currentIndex] = (byte)'B';
					currentIndex++;
					buffer [currentIndex] = (byte)'D';
				} else 
				{
					buffer [currentIndex] = buf [currentIndex];
				}
				currentIndex++;
			}

			buffer[currentIndex] = (byte)'A';
			currentIndex ++;

			Console.WriteLine ("Sending size {0}",currentIndex);
			serialPort.Write (buffer,0, currentIndex);
		}

		public int receive (ref byte[] buf)
		{
			while (serialPort.ReadByte () != (byte)'A') {
			}

			int readIndex = 0;
			byte b;
			while ((b = (byte)serialPort.ReadByte ()) != (byte)'A') 
			{
				buffer [readIndex] = b;
				readIndex++;
			}
			int r = 0;
			for (int i = 0; i < r; i++,r++) 
			{
				if (buffer [i] == 'B' && buffer [i + 1] == 'C') {
					buf [r] = (byte)'A';
				} else if (buffer [i] == 'B' && buffer [i + 1] == 'D') {
					buf [r] = (byte)'B';
				} else
					buf [r] = buffer [i];
			}
			return r;
		}
	}
}
