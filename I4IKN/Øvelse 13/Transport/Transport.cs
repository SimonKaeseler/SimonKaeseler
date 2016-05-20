using System;
using Linklaget;


namespace Transportlaget
{

	public class Transport
	{
		#region PrivateParts
		private Link link;	
		private Checksum checksum;
		private byte[] buffer;
		private byte seqNo;
		private byte old_seqNo;	
		private int errorCount;	
		private const int DEFAULT_SEQNO = 2;
		#endregion

		public Transport (int BUFSIZE)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
		}
	
	


		public void send(byte[] buf, int size)
		{
			bool sending = true;
			errorCount = 0;
			while (sending) 
			{
				buffer [2] = seqNo;
				buffer [3] = (int)TransType.DATA;

				Array.Copy (buf, 0, buffer, (int)TransSize.ACKSIZE, size);

				checksum.calcChecksum (ref buffer, size + (int)TransSize.ACKSIZE);

				link.send (buffer, size + (int)TransSize.ACKSIZE);

				try
				{
					sending = !receiveAck();
				}
				catch(Exception e) 
				{
					errorCount++;
					if (errorCount >= 500) 
					{
						break;
					}else
					continue;
				}
			}
		}


		public int receive (ref byte[] buf)
		{
			while (true) 
			{
				try
				{
					int sizeOfData = link.receive(ref buffer);

					if(buffer[2] != old_seqNo)
					{
						if(checksum.checkChecksum(buffer,sizeOfData))
							{
								old_seqNo = buffer[2];
								sendAck(true);
								Array.Copy(buffer,(int)TransSize.ACKSIZE,buf,0,sizeOfData-(int)TransSize.ACKSIZE);
								return sizeOfData-(int)TransSize.ACKSIZE;
						}else sendAck(false);
					}else sendAck(false);
				} catch(Exception e) {
				}
			}
		}

		#region Acks
		private bool receiveAck()
		{
			byte[] buf = new byte[(int)TransSize.ACKSIZE];
			int size = link.receive(ref buf);
			if (size != (int)TransSize.ACKSIZE) return false;
			if(!checksum.checkChecksum(buf, (int)TransSize.ACKSIZE) ||
				buf[(int)TransCHKSUM.SEQNO] != seqNo ||
				buf[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				return false;

			seqNo = (byte)((buf[(int)TransCHKSUM.SEQNO] + 1) % 2);

			return true;
		}


		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}
		#endregion
	
	}
}