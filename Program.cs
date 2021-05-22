using System;
using System.IO;

namespace qr_code
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator("Hallo", Mode.alphanumeric, 6);
            byte[,] qr_code = generator.generate();

            for(int y = 0;y < qr_code.GetUpperBound(0);y++){
                for(int x = 0;x < qr_code.GetUpperBound(1);x++){
                    switch(qr_code[y,x]){
                        case 1: Console.ForegroundColor = ConsoleColor.Black;break;
                        case 2: Console.ForegroundColor = ConsoleColor.Gray;break;
                        default: Console.ForegroundColor = ConsoleColor.Magenta;break;
                    }
                    Console.Write("██");
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
