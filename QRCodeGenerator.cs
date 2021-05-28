using System;
using System.IO;
using System.Text;

// Tutorial: https://www.thonky.com/qr-code-tutorial/

namespace qr_code
{
    class Generator
    {


        public string message;
        public Mode mode;
        public ErrorCorrection errorCorrection;
        public int version;
        public int mask_pattern;

        // qr-code-matrix: format is [y,x]
        private byte[,] qr_code;
        private int size;


        public Generator(string message, Mode mode, ErrorCorrection error_correction, int version, int mask_pattern)
        {
            this.message = message;
            this.mode = mode;
            this.version = version;
            this.mask_pattern = mask_pattern;

        }

        public Generator(ref FileStream stream, Mode mode, int version, int mask_pattern)
        {
            this.mode = mode;
            this.version = version;
            this.mask_pattern = mask_pattern;
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
            for(int y = 0;y < size;y++){
                for(int x = 0;x < size;x++){
                    qr_code[x,y] = 0xff;
                }
            }
            InsertPatterns();
            InstertFormatAndVersionInformation();
            return qr_code;
        }

        private void InstertFormatAndVersionInformation(){
            byte[] format_string = new byte[15];
            
            // two bit thats specifies the error correction level in use in the qr code.
            // L := 01, M := 00, Q := 11, H := 10
            switch(errorCorrection){
                case ErrorCorrection.L:
                    format_string[1] = 1;
                case ErrorCorrection.M:break;
                case ErrorCorrection.Q:
                    format_string[1] = 1;
                case ErrorCorrection.H:
                    format_string[0] = 1;
            }
            
            // three bit that specify the mask pattern in use
            // convert the mask number into a three-bit binary string
            format_string[4] = (byte) mask_pattern % 2;
            format_string[3] = (byte) mask_pattern / 2 % 2;
            format_string[2] = (byte) mask_pattern / 4 % 2;

            foreach(byte f in format_string){
                Console.Write(f);
            }
        }

        private void InsertPatterns()
        {
            // insert finder patterns
            // 0 = black, 1 = white

            int pattern1_size = finder_pattern.pattern1.GetUpperBound(0) + 1;   // width, and hight are equal
            for (int y = 0; y < pattern1_size; y++)
            {
                for (int x = 0; x < pattern1_size; x++)
                {
                    // left upper corner
                    qr_code[y, x] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, x] = 1;                      // add a white stripe to the lower end

                    // right upper corner
                    qr_code[y, size - x - 2] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, size - x - 2] = 1;           // add a white stripe to the lower end

                    // left lower corner
                    qr_code[size - y - 2, x] = finder_pattern.pattern1[y, x];
                    qr_code[size - pattern1_size - 2, x] = 1;           // add a white stripe to the upper end
                }
                qr_code[y, pattern1_size] = 1;                          // add a white stripe to the right side of the left, upper pattern
                qr_code[y, size - pattern1_size - 2] = 1;               // add a white stripe to the left side of the right, upper pattern
                qr_code[size - y - 2, pattern1_size] = 1;               // add a white stripe to the right side of the left, lower pattern
            }
            qr_code[pattern1_size, pattern1_size] = 1;
            qr_code[size - pattern1_size - 2, pattern1_size] = 1;
            qr_code[pattern1_size, size - pattern1_size - 2] = 1;

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
            qr_code[size - pattern1_size - 2, pattern1_size + 1] = 0;

            // add timing patterns
            int counter = pattern1_size+1;
            byte color = 0;
            while(qr_code[pattern1_size-1,counter] == 0xff){
                qr_code[pattern1_size-1,counter] = color;
                qr_code[counter,pattern1_size-1] = color;
                color = (byte) (1 - color);
                counter++;
            }
        }
    }
}