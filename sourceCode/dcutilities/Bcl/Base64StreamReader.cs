using System;
using System.IO;


namespace DigitallyCreated.Utilities.Bcl
{
	/// <summary>
	/// A <see cref="Stream"/> that reads Base64 encoded bytes from a <see cref="TextReader"/>. This class
	/// has no ability to write or to seek.
	/// </summary>
	public class Base64StreamReader : Stream
	{
		private static readonly byte[] _ZeroLengthByteArray = new byte[0];
		private readonly TextReader _TextReader;
		private bool _Closed;
		private byte[] _BufferedBytes;


		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns>
		/// Always returns true
		/// </returns>
		public override bool CanRead
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns>
		/// Always returns false
		/// </returns>
		public override bool CanSeek
		{
			get { return false; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <returns>
		/// Always returns false
		/// </returns>
		public override bool CanWrite
		{
			get { return false; }
		}

		/// <summary>
		/// Not supported by <see cref="Base64StreamReader"/>
		/// </summary>
		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Not supported by <see cref="Base64StreamReader"/>
		/// </summary>
		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}


		/// <summary>
		/// Constructor, creates a Base64StreamReader that reads base64 text from <paramref name="textReader"/>
		/// </summary>
		/// <param name="textReader">The <see cref="TextReader"/> to read the base64 text from</param>
		public Base64StreamReader(TextReader textReader)
		{
			_TextReader = textReader;
			_Closed = false;
			_BufferedBytes = _ZeroLengthByteArray;
		}


		/// <summary>
		/// Does nothing for <see cref="Base64StreamReader"/>
		/// </summary>
		public override void Flush()
		{
		}


		/// <summary>
		/// Not supported by <see cref="Base64StreamReader"/>
		/// </summary>
		/// <param name="offset">Not used</param>
		/// <param name="origin">Not used</param>
		/// <returns>Never returns</returns>
		/// <exception cref="NotSupportedException">Always throws</exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}


		/// <summary>
		/// Not supported by <see cref="Base64StreamReader"/>
		/// </summary>
		/// <param name="value">Not used</param>
		/// <exception cref="NotSupportedException">Always throws</exception>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}


		/// <summary>
		/// Reads a sequence of bytes from the current stream and advances the position within the stream by
		/// the number of bytes read.
		/// </summary>
		/// <returns>
		/// The total number of bytes read into the buffer. This can be less than the number of bytes requested
		/// if that many bytes are not currently available, or you requested a number of bytes not divisible by
		/// three, or zero (0) if the end of the stream has been reached.
		/// </returns>
		/// <param name="buffer">
		/// An array of bytes. When this method returns, the buffer contains the specified byte array with the
		/// values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/>-
		/// 1) replaced by the bytes read from the current source. 
		/// </param>
		/// <param name="offset">
		/// The zero-based byte offset in <paramref name="buffer"/> at which to begin storing the data read
		/// from the current stream. 
		/// </param>
		/// <param name="count">
		/// The maximum number of bytes to be read from the current stream.
		/// </param>
		/// <exception cref="ArgumentException">
		/// The sum of <paramref name="offset"/> and <paramref name="count"/> is larger than the buffer length
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="offset"/> or <paramref name="count"/> is negative
		/// </exception>
		/// <exception cref="IOException">An I/O error occurs.</exception>
		/// <exception cref="ObjectDisposedException">
		/// Methods were called after the stream was closed. 
		/// </exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
				throw new ArgumentNullException("buffer");
			if (count + offset > buffer.Length)
				throw new ArgumentException("offset + count cannot be bigger the buffer.Length");
			if (offset < 0)
				throw new ArgumentOutOfRangeException("offset", offset, "offset cannot be a negative number");
			if (count < 0)
				throw new ArgumentOutOfRangeException("count", count, "count cannot be a negative number");
			if (_Closed)
				throw new ObjectDisposedException("Base64StreamReader");

			if (count == 0)
				return 0;

			//Read less because we've got some bytes in the buffer from last time
			//If necessary, don't read anything (but don't try to read a negative amount!)
			int needToReadByteCount = Math.Max(count - _BufferedBytes.Length, 0);
	
			//We need to read a mininum of four characters, this may mean we read more bytes
			//than are wanted
			char[] charBuf;
			if (needToReadByteCount % 3 == 0)
				charBuf = new char[needToReadByteCount / 3 * 4];
			else
				charBuf = new char[(needToReadByteCount / 3 + 1) * 4];
				
			
			int readSize = _TextReader.Read(charBuf, 0, charBuf.Length);

			//We need to remove all whitespace characters from the array and then
			//read more data to make up for the lost characters
			int rereadSize = readSize;
			int removedChars = RemoveWhitespaceFromCharArray(charBuf, 0, readSize);
			readSize -= removedChars;
			while (rereadSize > 0 && removedChars > 0)
			{
				rereadSize = _TextReader.Read(charBuf, readSize, removedChars);
				removedChars = RemoveWhitespaceFromCharArray(charBuf, readSize, rereadSize);
				readSize += rereadSize;
				if (removedChars == 0)
					break;
				readSize -= removedChars;
			}

			//Only try to do a convert if we actually read data
			byte[] convertBuf;
			if (readSize > 0)
				convertBuf = Convert.FromBase64CharArray(charBuf, 0, readSize);
			else
				convertBuf = _ZeroLengthByteArray;

			//Copy whatever was left over from last time out of the buffer
			int amountOfBufferToUse = Math.Min(_BufferedBytes.Length, count);
			Array.Copy(_BufferedBytes, 0, buffer, offset, amountOfBufferToUse);
			
			//If we got all we needed from the buffer
			if (readSize == 0)
			{
				//Remove the data we used from the buffer
				int newBufferSize = _BufferedBytes.Length - amountOfBufferToUse;
				if (newBufferSize > 0)
				{
					byte[] newBuffer = new byte[newBufferSize];
					Array.Copy(_BufferedBytes, amountOfBufferToUse, newBuffer, 0, newBufferSize);
					_BufferedBytes = newBuffer;
				}
				else
					_BufferedBytes = _ZeroLengthByteArray;

				return amountOfBufferToUse;
			}
				

			//Copy whatever we need from the converted buffer
			int amountOfConvertBufferToUse = Math.Min(convertBuf.Length, count - amountOfBufferToUse /*<- the remaining amount needed*/);
			Array.Copy(convertBuf, 0, buffer, offset + amountOfBufferToUse, amountOfConvertBufferToUse);

			//Is there any left over? If so, buffer it
			int bufferSize = convertBuf.Length - amountOfConvertBufferToUse;
			if (bufferSize > 0)
			{
				_BufferedBytes = new byte[bufferSize];
				Array.Copy(convertBuf, amountOfConvertBufferToUse, _BufferedBytes, 0, bufferSize);
			}
			else
				_BufferedBytes = _ZeroLengthByteArray;
			
			return count;
		}


		/// <summary>
		/// Removes all whitespace (tab, space, carriage return and newline characters) from a
		/// char array and shifts all data following the whitespace left (into earlier indicies).
		/// This leaves the array in a state where all the remaining data is up the beginning
		/// of the array and no whitespace can be found within it.
		/// </summary>
		/// <param name="charBuf">The char array</param>
		/// <param name="offset">The array offset index to start looking from</param>
		/// <param name="count">The number of array elements to search through</param>
		/// <returns>The number of characters removed</returns>
		private int RemoveWhitespaceFromCharArray(char[] charBuf, int offset, int count)
		{
			if (offset + count > charBuf.Length)
				throw new ArgumentException("offset + count cannot be bigger the charBuf.Length");

			int removedCharacterCount = 0;
			for (int i = offset; i < offset + count; i++)
			{
				if (charBuf[i] == '\t' || charBuf[i] == ' ' || charBuf[i] == '\r' || charBuf[i] == '\n')
					removedCharacterCount++;
				else if (removedCharacterCount > 0)
					charBuf[i - removedCharacterCount] = charBuf[i];
			}

			return removedCharacterCount;
		}


		/// <summary>
		/// Closes the current stream (and the TextReader being read by this stream)
		/// </summary>
		public override void Close()
		{
			base.Close();
			_BufferedBytes = null;
			_TextReader.Close();
			_Closed = true;
		}


		/// <summary>
		/// Not supported by <see cref="Base64StreamReader"/>
		/// </summary>
		/// <param name="buffer">Not used</param>
		/// <param name="count">Not used</param>
		/// <param name="offset">Not used</param>
		/// <exception cref="NotSupportedException">Always throws</exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}