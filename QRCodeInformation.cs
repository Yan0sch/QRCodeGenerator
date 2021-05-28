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
    public enum Mode
    {
        numeric,             // 0..9
        alphanumeric,        // 0..9, A..B, $%*+-./: and space
        byte_mode                  // ISO-8859-1
    }
    public enum ErrorCorrection
    {    // Error Correction Level in %
        L, M, Q, H
    }

    // Different versions of qr-codes expect different sizes of the character count indicator
    public enum Version1To9 : int
    {
        numeric = 10,
        alphanumeric = 9,
        byte_mode = 8
    }
    public enum Version10To26 : int
    {
        numeric = 12,
        alphanumeric = 11,
        byte_mode = 16
    }
    public enum Version27To40 : int
    {
        numeric = 14,
        alphanumeric = 13,
        byte_mode = 16
    }


    // Two finder patterns that each qr-code need.
    // Version 1 only needs pattern 1, the others need pattern 2.
    public class finder_pattern
    {
        public static byte[,] pattern1 = new byte[,] {
            {0,0,0,0,0,0,0},
            {0,1,1,1,1,1,0},
            {0,1,0,0,0,1,0},
            {0,1,0,0,0,1,0},
            {0,1,0,0,0,1,0},
            {0,1,1,1,1,1,0},
            {0,0,0,0,0,0,0}
        };

        public static byte[,] pattern2 = new byte[,] {
            {0,0,0,0,0},
            {0,1,1,1,0},
            {0,1,0,1,0},
            {0,1,1,1,0},
            {0,0,0,0,0}
        };
    }
}