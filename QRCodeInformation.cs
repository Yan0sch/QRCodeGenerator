using System;

/**
 * This file contains all information the program needs to generate a qr-code,
 * such as patterns, error correction levels, etc.
 */


namespace qr_code
{

    /**
     * The encoding mode depends on what kind of data you want to store.
     * There are two more encoding modes: Kanji Mode and ECI Mode
     */
    public class Mode
    {
        public const byte numeric = 0b0001;             // 0..9
        public const byte alphanumeric = 0b0010;        // 0..9, A..B, $%*+-./: and space
        const byte byte_mode = 0b0100;                  // ISO-8859-1
    }
    public class ErrorCorrection
    {    // Error Correction Level in %
        public const byte L = 7;
        public const byte M = 15;
        public const byte Q = 25;
        public const byte H = 30;
    }

    // Different versions of qr-codes expect different sizes of the character count indicator
    public class Version1To9
    {
        public const byte numeric = 10;
        public const byte alphanumeric = 9;
        public const byte byte_mode = 8;
    }
    public class Version10To26
    {
        public const byte numeric = 12;
        public const byte alphanumeric = 11;
        public const byte byte_mode = 16;
    }
    public class Version27To40
    {
        public const byte numeric = 14;
        public const byte alphanumeric = 13;
        public const byte byte_mode = 16;
    }


    // Two finder patterns that each qr-code need.
    // Version 1 only needs pattern 1, the others need pattern 2.
    public class finder_pattern
    {
        public static byte[,] pattern1 = new byte[,] {
            {1,1,1,1,1,1,1},
            {1,2,2,2,2,2,1},
            {1,2,1,1,1,2,1},
            {1,2,1,1,1,2,1},
            {1,2,1,1,1,2,1},
            {1,2,2,2,2,2,1},
            {1,1,1,1,1,1,1}
        };

        public static byte[,] pattern2 = new byte[,] {
            {1,1,1,1,1},
            {1,2,2,2,1},
            {1,2,1,2,1},
            {1,2,2,2,1},
            {1,1,1,1,1}
        };
    }
}