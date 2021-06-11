using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SzyfrySieci1
{
    class Ciphers
    {
        private char[,] VigenereSquare;
        public Ciphers()
        {
            VigenereSquare = GenerateVigenereSquare();
        }
        public string RailFence_Encode(string M, int k) //płotek składa się z k szczebli
        {
            if (String.IsNullOrEmpty(M) || k == 0) //przypadek dla pustego słowa wejściowego lub wysokości płotku = 0
                return null;
            if (k == 1) //przypadek dla wysokości płotku = 1
                return M;

            List<StringBuilder> fence = new List<StringBuilder>(); //płotek
            for (int i = 0; i < k; i++)
            {
                StringBuilder rail = new StringBuilder(); //tworzenie k szczebli
                fence.Add(rail);                          //dodawanie szczebli do płotka
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

            StringBuilder C = new StringBuilder();
            foreach (StringBuilder rail in fence)
            {
                C.Append(rail);    //łączenie szczebli płotka ze znakami w jednopoziomowy wyjściowy łańcuch znaków
            }

            return C.ToString();
        }

        public string RailFence_Decode(string C, int k)
        {
            if (C.Length <= k)
                return C;

            int numOfFullVLikeParts = (C.Length - 1) / (2 * k - 2); // zaszyfrowana poprzez rail fence wiadomość wygląda jak płotek składający się z części w kształcie litery V;
                                                                    //przyjąłem, że pełna taka część nie zawiera pierwszej swojej litery, ponieważ jest ona z reguły częścią wspólną z poprzednią taką częścią
            int numOfOtherLetters = (C.Length - 1) % (2 * k - 2);

            string[] rails = new string[k];

            int numOfLettersInRail = 1 + numOfFullVLikeParts; //w górnej szynie jest tyle liter, ile jest części V + 1-sza litera
            rails[0] = C.Substring(0, numOfLettersInRail);
            int firstIndexOfNextRail = 0;
            for (int i = 1; i < k - 1; i++)
            {
                firstIndexOfNextRail += numOfLettersInRail;
                numOfLettersInRail = 2 * numOfFullVLikeParts + (numOfOtherLetters >= i ? numOfOtherLetters >= i + 2 * (k - (i + 1)) ? 2 : 1 : 0);
                    //w "wewnętrznej" szynie jest tyle liter, ile wynosi dwukrotność części V powiększona o 1 lub 2 litery z ewent. niepełnej części V; 2*(k-(i+1)) to odległość litery z "wnętrza" (nie)pełnej częsci V do kolejnej z tego wnętrza na tej samej wysokości
                rails[i] = C.Substring(firstIndexOfNextRail, numOfLettersInRail);
            }
            firstIndexOfNextRail += numOfLettersInRail;
            numOfLettersInRail = numOfFullVLikeParts + (numOfOtherLetters >= k - 1 ? 1 : 0); //w dolnej szynie jest tyle liter, ile jest części V i ewentualnie dodatkowa jeśli pozostałych liter jest chociaż k-1
            rails[k - 1] = C.Substring(firstIndexOfNextRail, numOfLettersInRail);

            char[] M = new char[C.Length];
            int shift;
            int lastChangedIndex = 0;
            for (int i = 0; i < k; i++) //dla każdej szyny
            {
                if (i == 0 || i == k - 1)
                {
                    shift = 2 * k - 2;
                    for (int j = 0; j < rails[i].Length; j++) //każdą literę z szyny
                        M[i + shift * j] = rails[i][j];       //umieszczamy w ciągu wynikowym co shift równy długości części V
                    continue;
                }
                for (int j = 0; j < rails[i].Length; j++)
                {
                    if (j == 0)
                    {
                        M[i] = rails[i][0]; //wszystkie pierwsze litery "wewn." szyn znajdują się na początku wyjścia (pomiędzy indeksem 0 a indeksem k-1)
                        lastChangedIndex = i;
                    }
                    else
                    {
                        shift = ((j % 2) == 0 ? (2 * i) : (2 * (k - (i + 1)))); //litery z wewnętrznych szczebli płotka są rozmieszczone od siebie w dwojaki sposób
                                                                                //para liter w obrębie tej samej "części w kształcie litery V" jest oddalona od siebie inaczej niż para liter z różnych takich "części V" płotka
                                                                                //jeśli j jest parzyste, to należy umieścić literę w miejscu oddalonym od poprzedniego o dwukrotną "odległość" szyny od zera (ruch od poprzedniej części V do nowej)
                                                                                //jeśli nie, w miejscu oddalonym o dwukrotną odległość szyny od drugiego końca (ruch w obrębie tej samej części V)
                        M[lastChangedIndex + shift] = rails[i][j];  
                        lastChangedIndex += shift;
                    }
                }
            }

            return new string(M);
        }

        public string MatrixRearrangement2a_encode(string M, string key, int d) //d mogłoby być także wyznaczane z klucza
        {
            List<string> fullLines = new List<string>();
            string notFullLine = null; //ostatni wiersz o niepełnej długości

            int numOfFullLines = M.Length / d; //liczba pełnych wierszy macierzy

            for (int i = 0; i < numOfFullLines; i++)
            {
                fullLines.Add(M.Substring(i * d, d)); //wypełnianie pełnych wierszy macierzy kolejnymi podciągami wejścia długości d
            }

            int numOfOtherLetters = M.Length % d; //liczba liter niepełnego wiersza macierzy
            if (numOfOtherLetters > 0)
            {
                notFullLine = M.Substring(numOfFullLines * d, numOfOtherLetters); //wypełnienie niepełnego wiersza macierzy ostatnim podciągiem wejścia, długości numOfOtherLetters
            }

            StringBuilder C = new StringBuilder();

            string[] keyNumbersTemp = key.Split('-');
            List<int> keyIndices = new List<int>();
            foreach (string num in keyNumbersTemp)
            {
                keyIndices.Add(Convert.ToInt32(num) - 1); //konwersja i zapamiętanie kolejnych zdekrementowanych cyfr klucza
            }

            foreach (string line in fullLines) // dla każdego wiersza z grona pełnych wierszy
            {
                foreach (int indice in keyIndices) // dla każdego indeksu (każdej zdekrementowanej cyfry z klucza)
                {
                    C.Append(line[indice]); // dodajemy do wyniku literę z wiersza znajdującą się pod danym indeksem
                }
            }
            if (notFullLine != null) // dla niepełnego wierszarsza
            {
                foreach (int indice in keyIndices)
                {
                    if (indice >= notFullLine.Length) // jeśli indeks przekracza wymiary niepełnego wiersza
                        continue;

                    C.Append(notFullLine[indice]);
                }
            }

            return C.ToString();
        }

        public string MatrixRearrangement2a_decode(string C, string key, int d) //d mogłoby być także wyznaczane z klucza
        {
            List<string> fullLines = new List<string>(); // lista wierszy macierzy długości klucza
            string notFullLine = null; // ostatni wiersz macierzy o niepełnej długości

            int numOfFullLines = C.Length / d;

            for (int i = 0; i < numOfFullLines; i++) //analogicznie do metody encode pobieram kolejne bloki liter z wejścia do macierzy (szyfru C)
            {
                fullLines.Add(C.Substring(i * d, d));
            }

            int numOfOtherLetters = C.Length % d;
            if (numOfOtherLetters > 0)
            {
                notFullLine = C.Substring(numOfFullLines * d, numOfOtherLetters);
            }

            StringBuilder M = new StringBuilder();
            List<char> partOfMessage = new List<char>(new char[d]); //lista znaków długości d

            string[] keyNumbersTemp = key.Split('-');
            List<int> keyIndices = new List<int>();
            foreach (string num in keyNumbersTemp)
            {
                keyIndices.Add(Convert.ToInt32(num) - 1);
            }
            for (int i = 0; i < fullLines.Count; i++)
            {
                for (int j = 0; j < d; j++) //dla każdego j mniejszego od długości klucza
                {
                    partOfMessage[keyIndices[j]] = fullLines[i][j]; //odtwarzana część (wiersz macierzy) wiadomości na indeksie równym j-tej liczbie z klucza (zdekrementowanej) to j-ty znak i-tego wiersza macierzy szyfru C
                                                                    //dla przykładu dla wiersza macierzy szyfru C = "YCPR" -> 'Y' przy kluczu "3-4-1-5-2" trafia na 3-cie miejsce odtwarzanej części wiadomości M
                }

                M.Append(partOfMessage.ToArray());
                partOfMessage = new List<char>(new char[d]);
            }
            if (notFullLine != null)
            {
                keyIndices.RemoveAll(i => i >= numOfOtherLetters); //usuwam te liczby z klucza, które i tak się nie przydadzą dla niepełnego wiersza macierzy
                for (int j = 0; j < numOfOtherLetters; j++)
                {
                    partOfMessage[keyIndices[j]] = notFullLine[j];
                }

                M.Append(partOfMessage.ToArray());
                partOfMessage.Clear();
            }

            return M.ToString();
        }

        struct KeyLetter
        {
            public char value;
            public int initialIndice; //pierwotny indeks danej litery w kluczu, niezależny od jej wartości liczbowej (zależnej od kolejności w alfabecie)
        }

        public string MatrixRearrangement2b_encode(string M, string Key)
        {
            M = String.Join("", M.Split(' '));
            int messageLength = M.Length;
            int keyLength = Key.Length;
            int numOfFullLines = messageLength / keyLength; // liczba pełnych (tzn. długości klucza) wierszy macierzy
            char[][] matrix = new char[numOfFullLines][];
            
            for (int i = 0; i<numOfFullLines; i++)
                matrix[i] = M.Substring(i * keyLength, keyLength).ToCharArray(); //pełne wiersze macierzy mają długość klucza

            int numOfOtherLetters = messageLength % keyLength; //litery poza pełnymi wierszami
            char[] notFullLine;
            string otherLetters = null;
            if (numOfOtherLetters > 0)
            {
                notFullLine = new char[numOfOtherLetters];
                otherLetters = M.Substring(messageLength - numOfOtherLetters);
            }

            KeyLetter[] keyLetters = new KeyLetter[keyLength];

            for (int i = 0; i < keyLength; i++)
            {
                keyLetters[i] = new KeyLetter
                {
                    value = Key[i],
                    initialIndice = i
                };
            }

            keyLetters = keyLetters.OrderBy(keyLetter => keyLetter.value).ThenBy(keyLetter => keyLetter.initialIndice).ToArray();
            //w tablicy zawarte są litery z klucza posortowane pierw względem kolejności w alfabecie, a następnie względem kolejności pierwotnej w kluczu

            StringBuilder C = new StringBuilder();
            int handledColumn;
            for (int i = 0; i<keyLength; i++)
            {
                handledColumn = keyLetters[i].initialIndice; 
                for (int j = 0; j < numOfFullLines; j++)
                   C.Append(matrix[j][handledColumn]); //z każdego wiersza pobieram znaki aktualnie przetwarzanej kolumny analogicznej kolejnościowo do pierwotnego indeksu kolejnej z posortowanych liter w kluczu

                if (handledColumn < numOfOtherLetters) //jeśli dana kolumna obejmuje litery ostatniego, niepełnego wiersza macierzy
                    C.Append(otherLetters[handledColumn]);

            }

            return C.ToString();
        }

        public string MatrixRearrangement2b_decode(string C, string Key)
        {
            C = String.Join("", C.Split(' '));
            int CLength = C.Length;
            int keyLength = Key.Length;

            KeyLetter[] keyLetters = new KeyLetter[keyLength];

            for (int i = 0; i < keyLength; i++)
            {
                keyLetters[i] = new KeyLetter
                {
                    value = Key[i],
                    initialIndice = i
                };
            }

            keyLetters = keyLetters.OrderBy(keyLetter => keyLetter.value).ThenBy(keyLetter => keyLetter.initialIndice).ToArray();
            //sortowanie alfabetycznie, tożsame z tym w metodzie encode

            int numOfFullMatrixLines = CLength / keyLength;
            int numOfOtherMatrixLetters = CLength % keyLength;
            string otherLetters = null;
            int numOfMatrixLines = numOfFullMatrixLines;
            if (numOfOtherMatrixLetters > 0)
            {
                otherLetters = C.Substring(C.Length - numOfOtherMatrixLetters);
                numOfMatrixLines++;
            }

            List<string> MatrixColumns = new List<String>(new string[keyLength]);
            int handledMColumn;

            int nextBlockBeginning = 0;
            int lettersToCut;
            for (int i = 0; i < keyLength; i++) // każdej literze klucza odpowiada kolumna macierzy wiadomości M
            {
                handledMColumn = keyLetters[i].initialIndice; // kolejny blok szyfru C pochodzi z kolumny o indeksie równym pierwotnemu indeksowi i-tej litery posortowanej tablicy liter klucza

                if (handledMColumn < numOfOtherMatrixLetters) //jeżeli mamy do czynienia z kolumną, która ma literę wspólną z ostatnim, niepełnym wierszem macierzy
                    lettersToCut = numOfMatrixLines;
                else
                    lettersToCut = numOfFullMatrixLines;

                MatrixColumns[handledMColumn] = C.Substring(nextBlockBeginning, lettersToCut);
                nextBlockBeginning += lettersToCut;
            }

            StringBuilder M = new StringBuilder();

            for (int line = 0; line < numOfFullMatrixLines; line++) //pobieram kolejno wierszami kolejne znaki wiadomości wejściowej M
                foreach (string column in MatrixColumns)
                    M.Append(column[line]);

            if (numOfOtherMatrixLetters > 0)
                for (int column = 0; column < numOfOtherMatrixLetters; column++)
                    M.Append(MatrixColumns[column][numOfFullMatrixLines]);

            return M.ToString();
        }

        public string MatrixRearrangement2c_encode(string M, string Key)
        {
            //M = String.Join("", M.Split(' '));
            int MLength = M.Length;
            int keyLength = Key.Length;

            KeyLetter[] keyLetters = new KeyLetter[keyLength];

            for (int i = 0; i < keyLength; i++)
            {
                keyLetters[i] = new KeyLetter
                {
                    value = Key[i],
                    initialIndice = i
                };
            }

            keyLetters = keyLetters.OrderBy(keyLetter => keyLetter.value).ThenBy(keyLetter => keyLetter.initialIndice).ToArray();
            //w tablicy zawarte są litery z klucza posortowane pierw względem kolejności w alfabecie, a następnie względem kolejności pierwotnej w kluczu


            List<string> matrixLines = new List<string>(); //lista wierszy macierzy
            int incrementedKeyLetterIdx;

            for (int startIdxOfNextMessageBlock = 0, i = 0; startIdxOfNextMessageBlock < MLength; i++)
            {
                incrementedKeyLetterIdx = keyLetters[i].initialIndice + 1;
                if (startIdxOfNextMessageBlock + incrementedKeyLetterIdx > MLength) //jeśli długość wiadomości wejściowej M może zostać przekroczona
                    incrementedKeyLetterIdx -= (startIdxOfNextMessageBlock + incrementedKeyLetterIdx) - MLength; //to nie biorę tyle liter na ile wskazuje zinkrementowany indeks klucza, a tyle, ile mogę maksymalnie wziąć
                    
                matrixLines.Add(M.Substring(startIdxOfNextMessageBlock, incrementedKeyLetterIdx)); //wycinam z wejścia podciąg od danego indeksu o ustalonej uprzednio długości
                startIdxOfNextMessageBlock += incrementedKeyLetterIdx; //indeks następnego bloku/podciągu/wiersza to następna litera po ostatnio pobranej
                
            }

            StringBuilder C = new StringBuilder();
            foreach (KeyLetter keyLetter in keyLetters)
            {
                int keyLetterIdx = keyLetter.initialIndice;
                foreach (string matrixLine in matrixLines) //pobieram dla każdego indeksu litery z klucza po jednej literze z każdego wiersza (pobierając de facto kolumnę)
                {
                    if (keyLetterIdx < matrixLine.Length) //pobieram tylko wtedy, kiedy mogę (indeks litery z klucza może przekraczać wiersz)
                        C.Append(matrixLine[keyLetterIdx]);
                }
            }

            return C.ToString();
        }

        public string MatrixRearrangement2c_decode(string C, string Key)
        {
            //C = String.Join("", C.Split(' '));
            int CLength = C.Length;
            int keyLength = Key.Length;

            KeyLetter[] keyLetters = new KeyLetter[keyLength];

            for (int i = 0; i < keyLength; i++)
            {
                keyLetters[i] = new KeyLetter
                {
                    value = Key[i],
                    initialIndice = i
                };
            }

            keyLetters = keyLetters.OrderBy(keyLetter => keyLetter.value).ThenBy(keyLetter => keyLetter.initialIndice).ToArray();

            int fullMatrixLinesCounter;
            int numOfAllMatrixLines = 0;
            int matrixLettersCounter = 0, numOfOtherMatrixLetters = 0;
            for (fullMatrixLinesCounter = 0; fullMatrixLinesCounter < keyLetters.Length; fullMatrixLinesCounter++)
            {
                matrixLettersCounter += keyLetters[fullMatrixLinesCounter].initialIndice + 1;

                if (matrixLettersCounter >= CLength)
                {
                    if (matrixLettersCounter > CLength)
                    {
                        numOfOtherMatrixLetters = CLength - (matrixLettersCounter - (keyLetters[fullMatrixLinesCounter--].initialIndice + 1));
                        numOfAllMatrixLines++; //tymczasowo, później zostaje podniesiona o liczbę pełnych wierszy
                    }
                    break;
                }
            }

            matrixLettersCounter = CLength;
            fullMatrixLinesCounter++; //wcześniej wiersze macierzy były liczone od 0
            numOfAllMatrixLines += fullMatrixLinesCounter;

            List<int> matrixColumnsLength = new List<int>(); //długości kolumn macierzy i zarazem długości bloków szyfru
            int letterCounter = 0;
            int handledColumnLength = 0;
            int handledColumnIdx = 0;

            List<StringBuilder> matrixColumns = new List<StringBuilder>();
            int CCharIdx = 0;
            while (letterCounter != CLength)
            {
                matrixColumns.Add(new StringBuilder());
                for (int i = 0; i<fullMatrixLinesCounter; i++)
                {
                    if (keyLetters[i].initialIndice >= keyLetters[handledColumnIdx].initialIndice)
                    {
                        matrixColumns[handledColumnIdx].Append(C[CCharIdx++]);
                        handledColumnLength++;
                    }
                    else
                    {
                        matrixColumns[handledColumnIdx].Append('\0');
                    }
                        
                }

                if (numOfOtherMatrixLetters > keyLetters[handledColumnIdx].initialIndice)
                {
                    matrixColumns[handledColumnIdx].Append(C[CCharIdx++]);
                    handledColumnLength++;
                }
                else
                {
                    matrixColumns[handledColumnIdx].Append('\0');
                }
                        

                matrixColumnsLength.Add(handledColumnLength); // kolumny są nieposortowane; ułożone są w kolejności w jakiej były brane do szyfru
                letterCounter += handledColumnLength;
                handledColumnLength = 0;
                handledColumnIdx++;
            }


            StringBuilder M = new StringBuilder();

            for (int i = 0; i < numOfAllMatrixLines; i++)
            {
                for (int j = 0; j < keyLetters.Length; j++)
                {
                    handledColumnIdx = Array.FindIndex(keyLetters, keyLetter => keyLetter.initialIndice == j);
                    if (matrixColumns[handledColumnIdx][i] != '\0')
                        M.Append(matrixColumns[handledColumnIdx][i]);
                }
            }


            return M.ToString();
        }

        public string ExtendedCaesar_encode(string A, int k1, int k0, int n)
        {
            if (GCD(k0, n) != 1 || GCD(k1, n) != 1)
            {
                Console.WriteLine("K0 and K1 should be relatively prime with n");
                return null;
            }
            StringBuilder encodedMessage = new StringBuilder();

            for (int i = 0; i < A.Length; i++)
                encodedMessage.Append((char)(((k1 * ((int)A[i] - 65) + k0) % n) + 65));

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
                decodedMessage.Append((char)(((((int)C[i] - 65) + (n - k0)) * Math.Pow(k1, CalculatePhi(n) - 1) % n + 65)));

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

        public int CalculatePhi(int n)
        {
            int relativelyPrimes = 0;

            for (int i = 1; i < n; i++)
            {
                if (GCD(i, n) == 1)
                    relativelyPrimes++;
            }
            return relativelyPrimes;
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
                    if (K.Length != initialKeyLength)
                        K = stringBuilder.Append(K.Substring(0, messageLength - K.Length)).ToString();
                }
            }
            stringBuilder = new StringBuilder();


            for (int i = 0; i < messageLength; i++)
                stringBuilder.Append(VigenereSquare[(int)M[i] - 65, (int)K[i] - 65]); // dla każdej kolumny tablicy Vigenere'a odpowiadającej danej literze wiadomości pobieramy znak z wiersza o numerze znajd. się pod analogicznym indeksem klucza


            return stringBuilder.ToString(); //EK(M)
        }

        public string Vigenere_decode(string EK, string K) // EK to zaszyfrowana wiadomość
        {
            char[,] VigenereSquare = this.VigenereSquare;
            StringBuilder stringBuilder;
            EK = EK.ToUpper();
            K = K.ToUpper();

            int EKLength = EK.Length;
            int initialKeyLength = K.Length;
            if (EKLength < initialKeyLength) //gdy długość zaszyfrowanej wiadomości jest krótsza od długości klucza
                K = K.Substring(0, EKLength); //obcinamy klucz do długości wiadomości
            else if (EKLength > initialKeyLength) //gdy długość zaszyfrowanej wiadomości jest dłuższa od długości klucza
            {
                int lengthDifference = EKLength - initialKeyLength;
                stringBuilder = new StringBuilder(K); //pobieramy aktualną postać klucza
                if (lengthDifference <= initialKeyLength) //jeśli różnica między długością wiadomości a długością klucza nie przekracza długości klucza
                    K = stringBuilder.Append(K.Substring(0, lengthDifference)).ToString(); //dodajemy do klucza jego początkową część długości wcześniej obliczonej różnicy
                else //jeśli różnica między długością wiadomości a długością klucza jest co najmniej długości klucza
                {
                    string initialKeyValue = K;
                    for (int i = 0; i < lengthDifference / initialKeyLength; i++) //tyle razy ile pełny klucz "zmieści się" we wcześniej wyliczonej różnicy
                        stringBuilder.Append(initialKeyValue); //dodajemy do klucza jego pierwotną wartość

                    if (stringBuilder.Length < EKLength) //jeśli jeszcze niedopełniono do długości wiadomości
                        stringBuilder.Append(stringBuilder.ToString().Substring(0, EKLength - stringBuilder.Length)); //dopełniamy różnicą
                       
                    K = stringBuilder.ToString();
                }
            }
            stringBuilder = new StringBuilder();

            char cellToTest;
            for (int i = 0; i < EKLength; i++)
                for (int j = 0; j < 26; j++)
                {
                    cellToTest = VigenereSquare[j, (int)K[i] - 65];
                    if (cellToTest == EK[i]) // jeśli j-ta kolumna macierzy da w K[i]-tym (numerycznie) wierszu EK[i], to jest to jeden z poszukiwanych znaków wiadomości wejściowej
                    {
                        stringBuilder.Append((char)(j + 65));
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


    }
}