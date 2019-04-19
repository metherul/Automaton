using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Hephaestus
{
    /// <summary>
    /// A Output stream that returns the MD5 hash of the data written to it
    /// after the stream has been closed. The stream does not retain any
    /// information written to it, so it's a simple way to hash (for example)
    /// the contents of an archive without having to extract them to disk
    /// </summary>
    class HashingStream : Stream
    {
        public string Filename;
        public long Size;
        private MD5CryptoServiceProvider md5_hasher;
        private SHA256CryptoServiceProvider sha_hasher;
        public string MD5Hash;
        public string SHA256Hash;

        public HashingStream(String filename)
        {
            this.Filename = filename;
            this.Size = 0;
            this.md5_hasher = new MD5CryptoServiceProvider();
            this.sha_hasher = new SHA256CryptoServiceProvider();
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void Flush()
        {
            return;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Size += count;
            md5_hasher.TransformBlock(buffer, 0, count, null, 0);
            sha_hasher.TransformBlock(buffer, 0, count, null, 0);
        }

        protected override void Dispose(bool disposing)
        {
            md5_hasher.TransformFinalBlock(new byte[0], 0, 0);
            sha_hasher.TransformFinalBlock(new byte[0], 0, 0);
            MD5Hash = ToHex(md5_hasher.Hash);
            SHA256Hash = ToHex(sha_hasher.Hash);
        }

        public static string ToHex(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString("x2"));

            return result.ToString();
        }
    }
}
