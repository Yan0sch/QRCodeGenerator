using System;
using System.IO;

namespace qr_code
{
    class Program
    {
        static void Main(string[] args)
        {
            Generator generator = new Generator("HELLO WORLD", Mode.alphanumeric, ErrorCorrection.Q, 4, 7);
            generator.generate();
            byte[,] qr_code = generator.qr_code;

            for(int y = 0;y < qr_code.GetUpperBound(0);y++){
                for(int x = 0;x < qr_code.GetUpperBound(1);x++){
                    switch(qr_code[y,x]){
                        case 0: Console.ForegroundColor = ConsoleColor.Gray;break;
                        case 1: Console.ForegroundColor = ConsoleColor.Black;break;
                        case 0xff: Console.ForegroundColor = ConsoleColor.Magenta;break;
                        default:
                            throw new ArgumentException("The value of the qr-code matrix has to be 0, 1 or 255!", nameof(qr_code));
                    }
                    Console.Write("██");
                }
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
