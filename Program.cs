using System;
using SzyfrySieci1;

namespace SzyfryPodstawowe
{
    class Program
    {
        static void Main(string[] args)
        {
            Ciphres ciphres = new Ciphres();
            Console.WriteLine(ciphres.RailFence_Encode("WITAJDZIENDOBRY", 7));
            Console.WriteLine(ciphres.Vigenere_encode("CRYPTOGRAPHY", "BREAK"));
            Console.WriteLine(ciphres.Vigenere_decode("CZESCICZOLEM", "WITAJ"));
            Console.WriteLine(ciphres.ExtendedCaesar_encode("CRYPTOGRAPHY", 7, 5, 26));
            Console.WriteLine(ciphres.ExtendedCaesar_decode("TURGIZVUFGCR", 7, 5, 26));
            Console.WriteLine(ciphres.ExtendedCaesar_encode("DZIENDOBRY", 2, 7, 23));
            Console.WriteLine(ciphres.ExtendedCaesar_decode("DZIENDOBRY", 2, 7, 23));

            Console.WriteLine(ciphres.RailFence_Decode("ABCDEFGHIJKLMNOPRS", 6));
            Console.WriteLine(ciphres.RailFence_Decode("CTARPORPYYGH", 3));
            Console.WriteLine(ciphres.RailFence_Encode("BEZPIECZENSTWOSIECI", 5));
            Console.WriteLine(ciphres.RailFence_Decode("BEEEZNICZCSSIPETOIW", 5));
            Console.WriteLine(ciphres.RailFence_Encode("BEZPIECZENSTWOSIECI2021", 7));
            Console.WriteLine(ciphres.RailFence_Decode("BWETOZSS1PNI2IEE0EZC2CI", 7));
            Console.WriteLine(ciphres.RailFence_Encode("ARKA", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_encode("CRYPTOGRAPHY", "3-4-1-5-2", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_encode("CRYPTOGRAP", "3-4-1-5-2", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_encode("CRYPTOGRAPHYOSA", "3-1-4-2", 4));
            Console.WriteLine(ciphres.MatrixRearrangement2a_encode("CRYPTOGRAPHY", "3-2-2-3-1", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_decode("YPCTRRAOPG", "3-4-1-5-2", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_decode("YPCTRRAOPGHY", "3-4-1-5-2", 5));
            Console.WriteLine(ciphres.MatrixRearrangement2a_decode("CRYPTOGRAPHYBEZPIECZENSTWOSIECI", "6-5-1-3-2-4", 6));
            Console.WriteLine(ciphres.MatrixRearrangement2a_encode("YTPORCAHPYRGZIPEEBESNTZCSEICOWI", "6-5-1-3-2-4", 6));

            Console.WriteLine(ciphres.MatrixRearrangement2b_encode("HERE IS A SECRET MESSAGE ENCIPHERED BY TRANSPOSITION", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_encode("CR YPTOGRAP HY", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_decode("HECRN CEYI ISEP SGDI RNTO AAES RMPN SSRO EEBT ETIA EEHS", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_decode("SIEMASIEMASZYFRYSIEMASIEMASZYFRYYYYY", "CONVENIFENCE"));


            Console.WriteLine(ciphres.MatrixRearrangement2c_encode("CRYPTOGRAPHYBEZPIECZENSTWOSIECI", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_encode("abcdefghijklmnopqrstuwxyz123", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_encode("abcdefghijklmnopqrstuwxyz123abcdefghijklmnopqrstuwxyz123", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_encode("abc defghijklmnopqrstuwxyz123abcdefghijklmnopqrstuwxyz 123", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_encode("abc defghijklmnopqrstuwxyz123abcdefghijklmnopqrstuwxyz 123", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_decode("afjolqu1x3dejhmkppusyw22beglrxachfkinnsqwtzz3cbgmrydiot1", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2b_decode("afjolqu1x3dejhmkppusyw22beglrxachfkinnsqwtzz3cbgmrydiot1", "CONVENIENCECONVENIENCECONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_encode("HERE IS A SECRET MESSAGE ENCIPHERED BY TRANSPOSITI", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_decode("HEESPNIRRSSEESEIYASCBTEMGEPNANDICTRTAHSOIEERO", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_decode("HEE   NOSEITSITIAEED GHAERENYPISAPR RRCMEBSS ESC T", "CONVENIENCE"));
            Console.WriteLine(ciphres.MatrixRearrangement2c_decode("abcd123", "CONVENIENCE"));

        }
    }
}
