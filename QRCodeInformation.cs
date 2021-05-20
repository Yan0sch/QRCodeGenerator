using System;

namespace qr_code{
    
    /* 
    The encoding mode depends on what kind of data you want to store.
    There are two more encoding modes: Kanji Mode and ECI Mode
    */
    enum Mode
    {
        numeric = 0x001,            // 0..9
        alphanumeric = 0x0010,       // 0..9, A..B, $%*+-./: and space
        byte_mode = 0x0100           // ISO-8859-1
    }
    enum ErrorCorrection : int {    // Error Correction Level in %
        L = 7,
        M = 15,
        Q = 25,
        H = 30
    }

    enum Version1To9{
        numeric = 10,
        alphanumeric = 9,
        byte_mode = 8
    }
    enum Version10To26{
        numeric = 12,
        alphanumeric = 11,
        byte_mode = 16
    }
    enum Version27To40{
        numeric = 14,
        alphanumeric = 13,
        byte_mode = 16
    }
}