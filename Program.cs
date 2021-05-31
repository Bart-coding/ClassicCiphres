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
            Console.WriteLine(ciphres.RailFence_Decode1("WBIORTDYANJEDIZ", 7));
            Console.WriteLine(ciphres.Vigenere_encode("CRYPTOGRAPHY", "BREAK"));
            Console.WriteLine(ciphres.Vigenere_decode("CZESCICZOLEM", "WITAJ"));


        }
    }
}
