

using System.Buffers.Text;

namespace HuffmanA
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello it`s archiver!");
            Console.Write("Do you want to compress file or decompress? c/d >> ");
            string line = Console.ReadLine();
            Huffman huffman = new Huffman();
            if (line == "c")
            {
                Console.Write("Type path to need file here >> ");
                string pathToData = Console.ReadLine();
                Console.Write("Type path where create a archive here >> ");
                string pathToArchive = Console.ReadLine();
                Console.Write("Processing...");
                huffman.CompressFile(pathToData, pathToArchive + ".huf");
                Console.WriteLine("Archive created!");
            }
            else if (line == "d")
            {

            }
            else
            {
                Console.WriteLine("Error");
            }




            

            huffman.DecompressFile("abra.txt.huf", "abra.txt.huf.txt");

        }
    }
}

