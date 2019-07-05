using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automaton.Common
{
    public class PatchingStream
    {
        private Stream source_a;
        private Stream source_b;
        private Stream dest;
        private long dest_size;

        public PatchingStream(Stream srca, Stream srcb, Stream dest, long dest_size)
        {
            this.source_a = srca;
            this.source_b = srcb;
            this.dest = dest;
            this.dest_size = dest_size;
        }

        private const int buffer_size = 1024 * 8;
        public void Patch()
        {
            byte[] buff_a = new byte[buffer_size];
            byte[] buff_b = new byte[buffer_size];
            byte[] dest_buff = new byte[buffer_size];

            long read = 0;
            while(true)
            {
                int read_a = read_all(source_a, buff_a);
                int read_b = read_all(source_b, buff_b);

                if (read_a == read_b)
                {
                    xor_copy(buff_a, buff_b, dest_buff, read_a);
                    dest.Write(dest_buff, 0, read_a);
                    read += read_a;
                }
                else if (read_a < read_b)
                {
                    xor_copy(buff_a, buff_b, dest_buff, read_a);
                    dest.Write(dest_buff, 0, read_a);
                    dest.Write(buff_b, read_a, read_b - read_a);

                    if (read >= dest_size) break;
                    source_b.CopyTo(dest);
                    break;
                }
                else if (read_a > read_b)
                {
                    xor_copy(buff_a, buff_b, dest_buff, read_a);
                    dest.Write(dest_buff, 0, read_b);
                    dest.Write(buff_a, read_b, read_a - read_b);

                    if (read >= dest_size) break;
                    source_a.CopyTo(dest);
                    break;
                }
            }
        }

        private void xor_copy(byte[] a, byte[] b, byte[] d, int count)
        {
            for (int x = 0; x < count; x ++)
            {
                d[x] = (byte)(a[x] ^ b[x]);
            }
        }

        private int read_all(Stream s, byte[] buff)
        {
            int remain = buff.Length;
            while (remain > 0) {
                int read = s.Read(buff, 0, remain);
                if (read == 0)
                    return buff.Length - remain;
                remain -= read;
            }
            return buff.Length;
        }
    }
}
