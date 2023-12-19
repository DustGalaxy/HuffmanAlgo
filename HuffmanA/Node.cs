using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanA
{
    internal class Node
    {
        public readonly byte symdol;
        public readonly int freq;
        public readonly Node bit0;
        public readonly Node bit1;

        public Node(byte symdol, int freq)
        {
            this.symdol = symdol;
            this.freq = freq;
        }

        public Node(Node bit0, Node bit1, int freq)
        {
            this.freq = freq;
            this.bit0 = bit0;
            this.bit1 = bit1;
        }
    }
}
