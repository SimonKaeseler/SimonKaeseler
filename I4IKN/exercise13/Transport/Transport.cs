using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{


	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{

		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private bool receiveAck()
		{
            
			byte[] buf = new byte[(int)TransSize.ACKSIZE];
			int size = link.Receive(ref buf);
			if (size != (int)TransSize.ACKSIZE) return false;
			if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) ||
					buf[(int)TransCHKSUM.SEQNO] != seqNo ||
					buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
                Console.WriteLine("buf[(int)TransCHKSUM.SEQNO]: {0} buf[(int)TransCHKSUM.TYPE]: {1}", buf[(int)TransCHKSUM.SEQNO], buf[(int)TransCHKSUM.TYPE]);
				return false;
			
			seqNo = (byte)((buf[(int)TransCHKSUM.SEQNO] + 1) % 2);
			
			return true;
		}

		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
					(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
            Console.WriteLine("ackBuf [(int)TransCHKSUM.SEQNO]: {0} ackBuf [(int)TransCHKSUM.TYPE]: {1} ", ackBuf[(int)TransCHKSUM.SEQNO], ackBuf[(int)TransCHKSUM.TYPE]);
			link.Send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
			bool sending = true;
			errorCount = 0;
			while (sending)
			{
				buffer[2] = seqNo;
				buffer[3] = (int)TransType.DATA;
				
				Array.Copy(buf, 0, buffer, 4, size);
				checksum.calcChecksum (ref buffer, (int) (size + TransSize.ACKSIZE));

				link.Send (buffer, (int) (size+TransSize.ACKSIZE));

				try{
					sending = !receiveAck();

				}
				catch(TimeoutException) {
					errorCount++;
					if (errorCount >= 5) {
						Console.WriteLine ("Connection timed out ...");
						break;
					} else
						continue;
				}

			}


		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			Console.WriteLine ("Transport.recieve");
			while (true) 
			{
				//byte[] buffToRecieve = new byte[buf.Length];

				int sizeOfData = link.Receive (ref buf);

				var check = checksum.checkChecksum (buffer, sizeOfData);
                Console.WriteLine("SeqNo: {0} OldSeqNo: {1}, Checksum {2}",seqNo,old_seqNo,check);
				if (check && seqNo != old_seqNo) 
				{
                    old_seqNo = buffer[2];
					sendAck (true);

					Array.Copy (buffer, 4, buf, 0, sizeOfData-(int)TransSize.ACKSIZE);
                    
					return (int) (sizeOfData-TransSize.ACKSIZE);
				}

				sendAck (false);
			}
		}

		public bool Ack()
		{
			return receiveAck ();
		}
	}
}