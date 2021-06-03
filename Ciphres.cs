using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Transactions;

namespace SzyfrySieci1
{
    class Ciphres //Skomentować porządnie
    {
        private char[,] VigenereSquare;
        public Ciphres()
        {
            VigenereSquare = GenerateVigenereSquare();
        }
        public string RailFence_Encode(string M, int k) //płotek składa się z k szczebli
        {
            if (String.IsNullOrEmpty(M) || k == 0) //przypadek dla pustego słowa wejściowego lub wysokości płotku = 0
                return null;
            if (k == 1) //przypadek dla wysokości płotku = 0
                return M;
            string C;
            List<StringBuilder> fence = new List<StringBuilder>(); //płotek
            for (int i = 0; i < k; i++)
            {
                StringBuilder rail = new StringBuilder(); //tworzenie k szczebli
                fence.Add(rail);    //dodawanie szczebli do płotka
            }

            int j = -1;
            bool down = true;   //domyślnie idzie się od góry płotka w dół (pierwotnym płotkiem jest najwyższy)
            foreach (char c in M) //dla każdego znaku w słowie wejściowym M
            {
                if (down) //jeżeli wyznaczono, by iść w dół płotka (dla szczebla j=0 jako najwyzszego i j=k-1 dla jako najniższego)
                {
                    if (j != k - 1) //jeżeli nie dotarto jeszcze do najniższego szczebla
                        j++;    //idź w dół płotka
                    else        //jeżeli dotarto to najniższego szczebla
                    {
                        j--;    //idź w górę płotka
                        down = false; //od tej pory dalej w górę płotka
                    }
                }
                else    //jeżeli wyznaczono, by iść w górę płotka
                {
                    if (j != 0) //jeżeli nie dotarto jeszcze do najwyższego szczebla
                        j--;    //idź w górę płotka
                    else
                    {
                        j++;    //idź w dół płotka
                        down = true; //od tej pory dalej w dół płotka
                    }
                }
                fence[j].Append(c); //dodanie litery do szczebla j płotka, do którego dotarto w danej iteracji
            }

            StringBuilder result = new StringBuilder();
            foreach (StringBuilder rail in fence)
            {
                result.Append(rail);    //łączenie szczebli płotka ze znakami w jednopoziomowy wyjściowy łańcuch znaków
            }
            C = result.ToString();
            return C;
        }

        public string RailFence_Decode(string C, int k)
        {
            if (C.Length <= k)
                return C;

            int numOfFullVLikeParts = (C.Length - 1) / (2 * k - 2); // zaszyfrowana poprzez rail fence wiadomość wygląda jak płotek składający się z części w kształcie litery V; przyjąłem, że pełna taka część nie zawiera pierwszej swojej litery, ponieważ jest ona z reguły częścią wspólną z poprzednią taką częścią
            int numOfOtherLetters = (C.Length - 1) % (2 * k - 2);

            string[] rails = new string[k];

            int numOfLettersInRail = 1 + numOfFullVLikeParts;
            rails[0] = C.Substring(0, numOfLettersInRail);
            Console.WriteLine(rails[0]);
            int firstIndexOfNextRail = 0;
            for (int i = 1; i < k - 1; i++)
            {
                firstIndexOfNextRail += numOfLettersInRail;
                numOfLettersInRail = 2 * numOfFullVLikeParts + (numOfOtherLetters >= i ? numOfOtherLetters >= i + 2*(k - (i + 1)) ? 2 : 1 : 0);
                rails[i] = C.Substring(firstIndexOfNextRail, numOfLettersInRail);
                Console.WriteLine(rails[i]);
            }
            firstIndexOfNextRail += numOfLettersInRail;
            numOfLettersInRail = numOfFullVLikeParts + (numOfOtherLetters >= k - 1 ? 1 : 0);
            rails[k-1] = C.Substring(firstIndexOfNextRail, numOfLettersInRail);
            Console.WriteLine(rails[k-1]);

            char[] M = new char[C.Length];
            int shift = 0;
            int lastChangedIndex = 0;
            for (int i = 0; i<k; i++)
            {
                if (i == 0 || i == k-1)
                {
                    shift = 2 * k - 2;
                    for (int j = 0; j < rails[i].Length; j++)
                        M[i + shift * j] = rails[i][j];
                    continue;
                }
                for (int j = 0; j < rails[i].Length; j++)
                {
                    if (j==0)
                    {
                        M[i] = rails[i][0];
                        lastChangedIndex = i;
                    }
                    else
                    {
                        shift = ((j % 2) == 0 ? (2 * i) : (2 * (k - (i + 1)))); //litery z wewnętrznych szczebli płotka są rozmieszczone od siebie w dwojaki sposób; para liter w obrębie tej samej "części w kształcie litery V" jest oddalona od siebie inaczej niż para liter z różnych takich części płotka
                        M[lastChangedIndex + shift] = rails[i][j];
                        lastChangedIndex+=shift;
                    }
                }
            }

            return new string(M);
        }

        public string Vigenere_encode(string M, string K)
        {
            char[,] VigenereSquare = this.VigenereSquare;
            StringBuilder stringBuilder;
            M = M.ToUpper();
            K = K.ToUpper();

            int messageLength = M.Length;
            int initialKeyLength = K.Length;
            if (messageLength < initialKeyLength)
                K = K.Substring(0, messageLength);
            else if (messageLength > initialKeyLength)
            {
                int lengthDifference = messageLength - initialKeyLength;
                stringBuilder = new StringBuilder(K);
                if (lengthDifference <= initialKeyLength)
                    K = stringBuilder.Append(K.Substring(0, lengthDifference)).ToString();
                else
                {
                    string initialKeyValue = K;
                    for (int i = 0; i < lengthDifference / initialKeyLength; i++)
                        K = stringBuilder.Append(initialKeyValue).ToString();
                    if (K.Length!=initialKeyLength)
                        K = stringBuilder.Append(K.Substring(0, messageLength-K.Length)).ToString();
                }  
            }
            stringBuilder = new StringBuilder();


            for (int i = 0; i < messageLength; i++)
                stringBuilder.Append(VigenereSquare[(int)M[i] - 65, (int)K[i] - 65]);


            return stringBuilder.ToString(); //EK(M)
        }

        public string Vigenere_decode(string EK, string K) // EK is encrypted message
        {
            char[,] VigenereSquare = this.VigenereSquare;
            StringBuilder stringBuilder;
            EK = EK.ToUpper();
            K = K.ToUpper();

            int EKLength = EK.Length;
            int initialKeyLength = K.Length;
            if (EKLength < initialKeyLength)
                K = K.Substring(0, EKLength);
            else if (EKLength > initialKeyLength)
            {
                int lengthDifference = EKLength - initialKeyLength;
                stringBuilder = new StringBuilder(K);
                if (lengthDifference <= initialKeyLength)
                    K = stringBuilder.Append(K.Substring(0, lengthDifference)).ToString();
                else
                {
                    string initialKeyValue = K;
                    for (int i = 0; i < lengthDifference / initialKeyLength; i++)
                        K = stringBuilder.Append(initialKeyValue).ToString();
                    if (K.Length != initialKeyLength)
                        K = stringBuilder.Append(K.Substring(0, EKLength - K.Length)).ToString();
                }
            }
            stringBuilder = new StringBuilder();

            char cellToTest;
            for (int i = 0; i < EKLength; i++)
                for (int j = 0; j < 26; j++)
                {
                    cellToTest = VigenereSquare[j, (int)K[i] - 65];
                    if (cellToTest == EK[i])
                    {
                        stringBuilder.Append((char) (j+65));
                        break;
                    }
                }

            return stringBuilder.ToString(); //EK(M)
        }

        public char[,] GenerateVigenereSquare() // może to być również tablica wartości całkowitych znaków ASCII
        {
            char[,] VigenereSquare = new char[26, 26];
            for (int i = 0; i < 26; i++)
            {
                for (int j = 0; j < 26; j++)
                {
                    VigenereSquare[i, j] = (char)((i + j) % 26 + 65);
                    //Console.Write(VigenereSquare[i, j] + " ");
                }
                //Console.WriteLine(" ");
            }
            
            return VigenereSquare;
        }

        public string ExtendedCaesar_encode (string A, int k1, int k0, int n)
        {
            if (GCD(k0, n) != 1 || GCD(k1, n) != 1)
            {
                Console.WriteLine("K0 and K1 should be relatively prime with n");
                return null;
            }
            StringBuilder encodedMessage = new StringBuilder();

            for (int i = 0; i < A.Length; i++)
                encodedMessage.Append((char) ( ((k1*((int)A[i]-65)+k0)%n ) + 65) );

            return encodedMessage.ToString();
        }
        public string ExtendedCaesar_decode(string C, int k1, int k0, int n)
        {
            if (GCD(k0, n) != 1 || GCD(k1, n) != 1)
            {
                Console.WriteLine("K0 and K1 should be relatively prime with n");
                return null;
            }
            StringBuilder decodedMessage = new StringBuilder();

            for (int i = 0; i < C.Length; i++)
                decodedMessage.Append( (char) (((((int)C[i] - 65) + (n-k0)) * Math.Pow(k1, CalculatePhi(n)-1) %n + 65)));

            return decodedMessage.ToString();
        }

        public int GCD(int a, int b)
        {
            int c;
            while (b != 0)
            {
                c = a % b;
                a = b;
                b = c;
            }
            return a;
        }

        public int CalculatePhi (int n)
        {
            int relativelyPrimes = 0;

            for (int i = 1; i<n; i++)
            {
                if (GCD(i, n) == 1)
                    relativelyPrimes++;
            }
            return relativelyPrimes;
        }
    }
}
