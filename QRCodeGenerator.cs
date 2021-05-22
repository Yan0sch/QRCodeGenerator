using System;
using System.IO;
using System.Text;

// Tutorial: https://www.thonky.com/qr-code-tutorial/

namespace qr_code
{
    class Generator
    {


        public string message;
        public byte mode;
        public int version;

        // qr-code-matrix: format is [y,x]
        private byte[,] qr_code;
        private int size;


        public Generator(string message, byte mode, int version)
        {
            this.message = message;
            this.mode = mode;
            this.version = version;
        }

        public Generator(ref FileStream stream, byte mode, int version)
        {
            this.mode = mode;
            this.version = version;
            StreamReader reader = new StreamReader(stream);

            string line;
            StringBuilder sb = new StringBuilder();
            while ((line = reader.ReadLine()) != null)
                sb.Append(line);
            this.message = sb.ToString();
            reader.Close();
        }

        public byte[,] generate(){
            size = 17 + (version * 4) + 1;          // version 1 := 21, version 2 := 25, ...
            qr_code = new byte[size, size];
            insert_pattern();
            return qr_code;
        }

        private void insert_patterns()
        {
            // insert finder patterns
            // 1 = black, 2 = white

            int pattern1_size = finder_pattern.pattern1.GetUpperBound(0) + 1;   // width, and hight are equal
            for (int y = 0; y < pattern1_size; y++)
            {
                for (int x = 0; x < pattern1_size; x++)
                {
                    // left upper corner
                    qr_code[y, x] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, x] = 2;                      // add a white stripe to the lower end

                    // right upper corner
                    qr_code[y, size - x - 2] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, size - x - 2] = 2;           // add a white stripe to the lower end

                    // left lower corner
                    qr_code[size - y - 2, x] = finder_pattern.pattern1[y, x];
                    qr_code[size - pattern1_size - 2, x] = 2;           // add a white stripe to the upper end
                }
                qr_code[y, pattern1_size] = 2;                          // add a white stripe to the right side of the left, upper pattern
                qr_code[y, size - pattern1_size - 2] = 2;               // add a white stripe to the left side of the right, upper pattern
                qr_code[size - y - 2, pattern1_size] = 2;               // add a white stripe to the right side of the left, lower pattern
            }
            qr_code[pattern1_size, pattern1_size] = 2;
            qr_code[size - pattern1_size - 2, pattern1_size] = 2;
            qr_code[pattern1_size, size - pattern1_size - 2] = 2;

            // if the version is 2 or bigger, the qr-code expect another finder pattern (pattern2)
            // 9 for the first one, 16 for the others
            if(version >= 2){
                // TODO implement patterns for version 7 and higher
                if(version >= 7) throw new ArgumentOutOfRangeException(nameof(version), "Version 7 or higher is not implemented!");

                int pattern2_size = finder_pattern.pattern2.GetUpperBound(0) + 1;
                (int, int) pattern2_pos = (size - 10,size - 10);          // format: (y,x)
                for(int y = 0;y < pattern2_size;y++){
                    for(int x = 0;x < pattern2_size;x++){
                        qr_code[pattern2_pos.Item1 + y,pattern2_pos.Item2 + x] = finder_pattern.pattern2[y,x];
                    }
                }
            }

            // add the dark module
            qr_code[size - pattern1_size - 2, pattern1_size + 1] = 1;

            // add timing patterns
            int counter = pattern1_size+1;
            byte color = 1;
            while(qr_code[pattern1_size-1,counter] == 0){
                qr_code[pattern1_size-1,counter] = color;
                qr_code[counter,pattern1_size-1] = color;
                color = (byte) (3 - color);
                counter++;
            }
        }
    }
}