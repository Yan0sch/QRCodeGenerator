using System;
using System.IO;

namespace qr_code
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator("Hallo", Mode.alphanumeric, 1);
        }
    }
}
