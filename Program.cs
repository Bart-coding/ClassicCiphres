using System;
using SzyfrySieci1;

namespace SzyfryPodstawowe
{
    class Program
    {
        static void Main(string[] args)
        {
            Ciphres ciphres = new Ciphres();

            string text = "CRYPTOGRAPHY";
            bool encoding = true;

            if (encoding)
            {
                Console.WriteLine("Rail fence encode: " + ciphres.RailFence_Encode(text, 3));

                Console.WriteLine("2a encode: " + ciphres.MatrixRearrangement2a_encode(text, "3-4-1-5-2", 5));

                Console.WriteLine("2b encode: " + ciphres.MatrixRearrangement2b_encode(text, "CONVENIENCE"));

                Console.WriteLine("2c encode: " + ciphres.MatrixRearrangement2c_encode(text, "CONVENIENCE"));

                Console.WriteLine("Vigenere encode: " + ciphres.Vigenere_encode(text, "BREAK"));

                Console.WriteLine("Caesar encode: " + ciphres.ExtendedCaesar_encode(text, 7, 5, 26));
            }
            else
            {
                Console.WriteLine("Rail fence decode: " + ciphres.RailFence_Decode(text, 7));

                Console.WriteLine("2a decode: " + ciphres.MatrixRearrangement2a_decode(text, "3-4-1-5-2", 5));

                Console.WriteLine("2b decode: " + ciphres.MatrixRearrangement2b_decode(text, "CONVENIENCE"));

                Console.WriteLine("2c decode: " + ciphres.MatrixRearrangement2c_decode(text, "CONVENIENCE"));

                Console.WriteLine("Vigenere decode: " + ciphres.Vigenere_decode(text, "BREAK"));

                Console.WriteLine("Caesar decode: " + ciphres.ExtendedCaesar_decode(text, 7, 5, 26));
            }
            

        }
    }
}
