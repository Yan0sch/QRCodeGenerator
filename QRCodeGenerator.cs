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
        public byte[,] qr_code { get; private set; }
        private int size;


        public Generator(string message, Mode mode, ErrorCorrection errorCorrection, int version, int mask_pattern)
        {
            this.message = message;
            this.mode = mode;
            this.errorCorrection = errorCorrection;
            this.version = version;
            this.mask_pattern = mask_pattern;

            size = 17 + (version * 4) + 1;          // version 1 := 21, version 2 := 25, ...
            qr_code = new byte[size, size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    qr_code[x, y] = 0xff;
                }
            }

        }

        public Generator(string message)
        {
            // TODO implement an constructor, that choose automatically the necessary mode and version
        }

        public void generate()
        {
            InsertPatterns();
            InsertFormatAndVersionInformation();
            InsertData();
        }

        private void InsertData()
        {
            // first we add the mode indicator and the character count indicator (cci)
            // numeric := 0001, alphanumeric := 0010, byte := 0100
            // information about the length of the character count indicator is in QRInformation.cs
            int capacity = Capacities.getCapacity(version, errorCorrection, mode);
            if (capacity < message.Length) throw new FormatException("The capacity of the qr-code is to low!");
            
            byte[] indicator = getIndicator();
            foreach (byte i in indicator)
            {
                Console.Write(i);
            }
            Console.Write("\n");
        }

        private byte[] getIndicator()
        {
            byte[] indicator = new byte[0];
            if (version <= 9)
            {
                switch (mode)
                {
                    case Mode.numeric: indicator = new byte[]       { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; break;
                    case Mode.alphanumeric: indicator = new byte[]  { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; break;
                    case Mode.byte_mode: indicator = new byte[]     { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; break;
                }
            }
            if (version > 7) throw new NotImplementedException("Version 7 or higher is not implemented");

            int m_len = message.Length;
            int idx = indicator.Length;
            while (m_len != 0)
            {
                indicator[--idx] = (byte)(m_len % 2);
                m_len /= 2;
            }
            return indicator;
        }

        private void InsertFormatAndVersionInformation()
        {
            byte[] format_string = new byte[15];

            // two bit thats specifies the error correction level in use in the qr code.
            // L := 01, M := 00, Q := 11, H := 10
            switch (errorCorrection)
            {
                case ErrorCorrection.L:
                    format_string[1] = 1;
                    break;
                case ErrorCorrection.M: break;
                case ErrorCorrection.Q:
                    format_string[0] = 1;
                    format_string[1] = 1;
                    break;
                case ErrorCorrection.H:
                    format_string[0] = 1;
                    break;
            }

            // three bit that specify the mask pattern in use
            // convert the mask number into a three-bit binary string
            format_string[4] = (byte)(mask_pattern % 2);
            format_string[3] = (byte)(mask_pattern / 2 % 2);
            format_string[2] = (byte)(mask_pattern / 4 % 2);


            // calculate the error correction bit for the format string
            byte[] errorCorrectionBits = new byte[format_string.Length];
            Array.Copy(format_string, errorCorrectionBits, format_string.Length);
            byte[] generatorPolynomial = { 1, 0, 1, 0, 0, 1, 1, 0, 1, 1, 1 };
            int firstBit = 0;
            removeZeros(errorCorrectionBits, out firstBit);

            while (errorCorrectionBits.Length - firstBit > generatorPolynomial.Length)
            {
                byte[] paddedPolynomial = new byte[errorCorrectionBits.Length - firstBit];
                padByteArray(generatorPolynomial, paddedPolynomial);
                xorByteArray(paddedPolynomial, errorCorrectionBits, firstBit);
                removeZeros(errorCorrectionBits, out firstBit);

            }

            // put format string and error correction bits together
            for (int i = 0; i < format_string.Length; i++)
            {
                format_string[i] |= errorCorrectionBits[i];
            }

            // xor with a mask pattern
            byte[] mask = { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 };
            xorByteArray(mask, format_string);

            // write the format string to the qr-code
            int offset = 0;
            for (int i = 0; i < format_string.Length / 2; i++)
            {
                if (qr_code[8, i + offset] != 0xff) offset++;
                qr_code[8, i + offset] = format_string[i];
                qr_code[size - i - 2, 8] = format_string[i];
            }
            offset = 0;
            for (int i = 0; i < format_string.Length / 2 + 1; i++)
            {
                if (qr_code[i + offset, 8] != 0xff) offset++;
                qr_code[i + offset, 8] = format_string[format_string.Length - i - 1];
                qr_code[8, size - i - 2] = format_string[format_string.Length - i - 1];
            }
        }

        private void removeZeros(byte[] arr, out int firstBit)
        {
            firstBit = 0;
            while (arr[firstBit] == 0) firstBit++;
        }

        // This function pad a given array to the left and create a new array with the given length
        private void padByteArray(byte[] arr, byte[] destinationArray)
        {
            if (arr.Length > destinationArray.Length) throw new ArgumentException("Destination array must be larger.", nameof(arr) + ", " + nameof(destinationArray));
            for (int i = 0; i < arr.Length; i++)
            {
                destinationArray[i] = arr[i];
            }

        }

        // this function xors two byte arrays, the second array is the destination
        private void xorByteArray(byte[] firstArray, byte[] secondArray, int startBit = 0)
        {
            if (firstArray.Length != secondArray.Length - startBit) throw new ArgumentException("Array must be the same size!", nameof(firstArray) + nameof(secondArray));
            for (int i = 0; i < firstArray.Length; i++)
            {
                secondArray[i + startBit] = (byte)(firstArray[i] ^ secondArray[i + startBit]);
            }
        }

        private void InsertPatterns()
        {
            // insert finder patterns
            // 0 = black, 1 = white

            int pattern1_size = finder_pattern.pattern1.GetUpperBound(0) + 1;   // width, and height are equal
            for (int y = 0; y < pattern1_size; y++)
            {
                for (int x = 0; x < pattern1_size; x++)
                {
                    // left upper corner
                    qr_code[y, x] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, x] = 0;                      // add a white stripe to the lower end

                    // right upper corner
                    qr_code[y, size - x - 2] = finder_pattern.pattern1[y, x];
                    qr_code[pattern1_size, size - x - 2] = 0;           // add a white stripe to the lower end

                    // left lower corner
                    qr_code[size - y - 2, x] = finder_pattern.pattern1[y, x];
                    qr_code[size - pattern1_size - 2, x] = 0;           // add a white stripe to the upper end
                }
                qr_code[y, pattern1_size] = 0;                          // add a white stripe to the right side of the left upper pattern
                qr_code[y, size - pattern1_size - 2] = 0;               // add a white stripe to the left side of the right upper pattern
                qr_code[size - y - 2, pattern1_size] = 0;               // add a white stripe to the right side of the left lower pattern
            }
            qr_code[pattern1_size, pattern1_size] = 0;
            qr_code[size - pattern1_size - 2, pattern1_size] = 0;
            qr_code[pattern1_size, size - pattern1_size - 2] = 0;

            // if the version is 2 or bigger, the qr-code expect another finder pattern (pattern2)
            // 9 for the first one, 16 for the others
            if (version >= 2)
            {
                // TODO implement patterns for version 7 and higher
                if (version >= 7) throw new NotImplementedException("Version 7 or higher is not implemented!");

                int pattern2_size = finder_pattern.pattern2.GetUpperBound(0) + 1;
                (int, int) pattern2_pos = (size - 10, size - 10);          // format: (y,x)
                for (int y = 0; y < pattern2_size; y++)
                {
                    for (int x = 0; x < pattern2_size; x++)
                    {
                        qr_code[pattern2_pos.Item1 + y, pattern2_pos.Item2 + x] = finder_pattern.pattern2[y, x];
                    }
                }
            }

            // add the dark module
            qr_code[size - pattern1_size - 2, pattern1_size + 1] = 1;

            // add timing patterns
            int counter = pattern1_size + 1;
            byte color = 1;
            while (qr_code[pattern1_size - 1, counter] == 0xff)
            {
                qr_code[pattern1_size - 1, counter] = color;
                qr_code[counter, pattern1_size - 1] = color;
                color = (byte)(1 - color);
                counter++;
            }
        }
    }
}