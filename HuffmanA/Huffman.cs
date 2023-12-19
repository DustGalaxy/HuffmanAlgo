using System;
using System.Buffers;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace HuffmanA
{
    internal class Huffman
    {



        public void CompressFile(string dataFilename, string outFilename)
        {
            byte[] data = File.ReadAllBytes(dataFilename);


            //long arrayLength = (long)((4.0d / 3.0d) * data.Length);
            //if (arrayLength % 4 != 0)
            //    arrayLength += 4 - arrayLength % 4;

            //char[] str = new char[arrayLength]; 

            //Convert.ToBase64CharArray(data, 0, data.Length, str, 0);

            //ReadOnlySequence<char> chars = new ReadOnlySequence<char>(str);

            //byte[] bytes = EncodingExtensions.GetBytes(Encoding.UTF8, chars);

            byte[] arch = CompressByte(data);
            File.WriteAllBytes(outFilename, arch);
        }

        private byte[] CompressByte(byte[] data)
        {
            int[] freqs = CalculateFreq(data);
            Node root = CreateHuffmanTree(freqs);
            string[] codes = CreateHuffmanCode(root);
            byte[] bits = Compress(data, codes);
            byte[] head = CreateHeader(data.Length, freqs);
            return head.Concat(bits).ToArray();
        }

        private byte[] CreateHeader(int dataLength, int[] freqs)
        {
            List<byte> head = new List<byte>();
            head.Add((byte)(dataLength & 255));
            head.Add((byte)(dataLength >> 8 & 255));
            head.Add((byte)(dataLength >> 16 & 255));
            head.Add((byte)(dataLength >> 24 & 255));

            for (int i = 0; i < 256; ++i)
                head.Add((byte)freqs[i]);

            return head.ToArray();
        }

        private byte[] Compress(byte[] data, string[] codes)
        {
            List<byte> bits = new List<byte>();
            byte sum = 0;
            byte bit = 1;
            foreach (var sumbol in data)
                foreach (var c in codes[sumbol])
                {
                    if (c == '1')
                        sum |= bit;
                    if (bit < 128)
                        bit <<= 1;
                    else
                    {
                        bits.Add(sum);
                        sum = 0;
                        bit = 1;
                    }
                }
            if (bit > 1)
                bits.Add(sum);
            return bits.ToArray();
        }

        private string[] CreateHuffmanCode(Node root)
        {
            string[] codes = new string[256];
            Next(root,"");
            return codes;

            void Next(Node node, string code)
            {
                if (node.bit0 == null)
                    codes[node.symdol] = code;
                else
                {
                    Next(node.bit0, code + '0');
                    Next(node.bit1, code + '1');
                }
            }
        }

        private Node CreateHuffmanTree(int[] freqs)
        {
            PriorityQueue<Node, int> pq = new();
            for (int i = 0; i < 256; i++)
                if (freqs[i] > 0)
                    pq.Enqueue(new Node((byte)i, freqs[i]), freqs[i]);

            while (pq.Count > 1)
            {
                Node bit0 = pq.Dequeue();
                Node bit1 = pq.Dequeue();
                int freq = bit1.freq + bit0.freq;
                Node next = new Node(bit0, bit1, freq);
                pq.Enqueue(next, freq);
            }

            return pq.Dequeue();
        }

        private int[] CalculateFreq(byte[] data)
        {
            int[] freqs = new int[256];
            foreach (var item in data)
                freqs[item]++;
            NormalizeFreqs(freqs);
            return freqs;
        }

        private void NormalizeFreqs(int[] freqs)
        {
            int max = freqs.Max();
            if (max <= 255)return;
            for (int i = 0; i < 256; i++)
                if (freqs[i] > 0)
                    freqs[i] = 1 + freqs[i] * 255 / (max + 1);
        }

        public void DecompressFile(string archFilename, string outFilename)
        {
            byte[] arch = File.ReadAllBytes(archFilename);
            byte[] data = DecompressBytes(arch);
            File.WriteAllBytes(outFilename, data);
        }

        private byte[] DecompressBytes(byte[] arch)
        {

            ParseHeader(arch, out int dataLength, out int startIndex, out int[] freqs);
            Node root = CreateHuffmanTree(freqs);

            byte[] data = Decompess(arch, startIndex, dataLength, root);

            return data;
        }

        private byte[] Decompess(byte[] arch, int startIndex, int dataLength, Node root)
        {
            int size = 0;
            Node curr = root;
            List<byte> data = new List<byte>();
            for (int i = startIndex; i < arch.Length; i++)
                for (int bit = 1; bit <= 128; bit <<= 1)
                {
                    if ((arch[i] & bit) == 0)
                    {
                        curr = curr.bit0;
                    }
                    else
                    {
                        curr = curr.bit1;
                    }

                    if (curr.bit0 != null)
                        continue;

                    if (size++ < dataLength)
                        data.Add(curr.symdol);
                    curr = root;
                }

            return data.ToArray();
        }

        private void ParseHeader(byte[] arch, out int dataLength, out int startIndex, out int[] freqs)
        {
            dataLength = arch[0] | (arch[1] << 8) 
                                 | (arch[1] << 16) 
                                 | (arch[1] << 24);

            freqs = new int[256];
            for (int i = 0; i < 256;  i++)
                freqs[i] = arch[4 + i];
            startIndex = 4 + 256;
        }

    }
}
