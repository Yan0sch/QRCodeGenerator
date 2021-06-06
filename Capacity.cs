using System;
using System.Collections.Generic;

namespace qr_code {

    // this class contains the character capacities by Version, Mode and Error Correction.

    public class Capacities {


        // [version, error correction, mode]
        // error correction L := 0, M := 1, Q := 2, H := 3
        // mode numeric := 0, alphanumeric := 1, byte := 2, kanji (not implemented) := 3
        private static readonly int[,,] capacities = {
            {
                {41, 25, 17},
                {34, 20, 14},
                {27, 16, 11},
                {17, 10,  7}
            },
            {
                {77, 47, 32},
                {63, 38, 26},
                {48, 29, 20},
                {34, 20, 14}
            },
            {
                {127, 77, 53},
                {101, 61, 42},
                {77, 47, 32},
                {58, 35, 24}
            },
            {
                {187, 114, 78},
                {149, 90, 62},
                {111, 67, 46},
                {82, 50, 34}
            },
            {
                {255, 154, 106},
                {202, 122, 84},
                {144, 87, 60},
                {106, 64, 44}
            },
            {       
                {322, 195, 134},
                {255, 154, 106},
                {178, 108, 74},
                {149, 84, 58}
            }
        };

        public static int getCapacity(int version, ErrorCorrection errorCorrection, Mode mode){
            if(version >= 7) throw new NotImplementedException("Version 7 or higher is not implemented");
            return capacities[version - 1, (int) errorCorrection, (int) mode];
        }
    }

}