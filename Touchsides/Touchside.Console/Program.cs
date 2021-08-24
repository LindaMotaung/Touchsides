using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Touchside
{
    public class WordCount
    {
        public int Count { get; set; }
        public string Word { get; set; }
    }

    public class ScrabbleScoreCardClass : WordCount { }

    enum ScrabbleScoreCard
    {
        A = 9,
        B = 2,
        C = 2,
        D = 4,
        E = 12,
        F = 2,
        G = 3,
        H = 2,
        I = 9,
        J = 1,
        K = 1,
        L = 4,
        M = 2,
        N = 6,
        O = 8,
        P = 2,
        Q = 1,
        R = 6,
        S = 4,
        T = 6,
        U = 4,
        V = 2,
        W = 2,
        X = 1,
        Y = 2,
        Z = 1
    }

    class Program
    {
        private static bool[] _lookup;

        static Program()
        {
            _lookup = new bool[65536];
            for (char c = '0'; c <= '9'; c++) _lookup[c] = true;
            for (char c = 'A'; c <= 'Z'; c++) _lookup[c] = true;
            for (char c = 'a'; c <= 'z'; c++) _lookup[c] = true;
            _lookup['.'] = true;
            _lookup['_'] = true;
        }

        static void Main(string[] args)
        {
            try
            {
                var filename = @"2600-0.txt";
                var filePath = AppDomain.CurrentDomain.BaseDirectory + filename;
                var listOfString = new List<string>();
                var wordCount = 0;

                if (File.Exists(filePath))
                {
                    var fileContents = File.ReadAllLines(filename);

                    var allData = File.ReadAllText(filename);
                    string replacement = Regex.Replace(allData, @"\t|\n|\r", " ").Replace(',', ' ').Replace('.', ' '); //reference: https://stackoverflow.com/questions/4140723/how-to-remove-new-line-characters-from-a-string/23248403
                    var resultData = replacement.Split(' ');

                    var groupData = resultData.GroupBy(x => x);
                    var listWordCountPair = new List<WordCount>();
                    var listOf7Chars = new List<WordCount>();
                    var listOfScrabble = new List<ScrabbleScoreCardClass>();
                    var scrabbleCount = 0;
                    var scrabbleCountIndexer = 0;
                    foreach (var dataIndex in groupData)
                    {

                        var wordObj = dataIndex.Key.ToString();
                        if (wordObj.Contains("“Hur-a-a-a-ah!”"))
                        {
                        }
                        var charArr = wordObj.ToCharArray();

                        for (int i = 0; i < charArr.Length; i++)
                        {
                            ScrabbleScoreCardClass scoreCardCls = new ScrabbleScoreCardClass();

                            var item = charArr[i].ToString();
                            item = RemoveSpecialCharacters(item);
                            //item = RemoveSpecialCharactersNew(item);

                            if (item.Contains("-") || item.Contains(":") || item.Contains("*") || item.Contains("\""))
                            {
                                var sanitizeCharArr = RemoveFromArray(charArr, charArr[i]);
                                if (inBounds(i, sanitizeCharArr) == true)
                                    item = sanitizeCharArr[i].ToString();
                            }
                            else
                            {
                                item = RemoveSpecialCharacters(item);
                                //item = RemoveSpecialCharactersNew(item);
                            }

                            if (item != ":" && !string.IsNullOrEmpty(item) && item != "*" && item != "-" && item != "(" && item != ")" && item != "“" && item != "”" && item != "\"")
                            {
                                if (wordObj != "tête-à-tête")
                                {
                                    var intEquivalent = GetValueOf(typeof(ScrabbleScoreCard), item.ToUpper(), wordObj);
                                    scrabbleCount += intEquivalent;

                                    scrabbleCountIndexer++;

                                    if (scrabbleCountIndexer == charArr.Length)
                                    {
                                        scoreCardCls.Word = wordObj;
                                        scoreCardCls.Count = scrabbleCount;
                                        listOfScrabble.Add(scoreCardCls);
                                        scrabbleCountIndexer = 0;
                                    }
                                }
                            }
                        }


                        if (!string.IsNullOrEmpty(wordObj))
                        {
                            WordCount wdCount = new WordCount();
                            wdCount.Word = dataIndex.Key;
                            wdCount.Count = dataIndex.Count();

                            listWordCountPair.Add(wdCount);

                            if (wordObj.Length == 7)
                                listOf7Chars.Add(wdCount);
                        }

                        //This is just to print every word on the screen. Comment out if you just want the result
                        Console.WriteLine(wordObj);
                    }

                    if (listWordCountPair.Any())
                    {
                        var maxCount = listWordCountPair.Max(x => x.Count);
                        var mostFrequentWord = listWordCountPair.FirstOrDefault(x => x.Count == maxCount);
                        Console.WriteLine("Most frequent word: " + "'" + mostFrequentWord.Word + "'" + " occurred " + maxCount + " times.");
                    }

                    if (listOf7Chars.Any())
                    {
                        var max7CharsCount = listOf7Chars.Max(x => x.Count);
                        var mostFrequent7CharsWord = listOf7Chars.FirstOrDefault(x => x.Count == max7CharsCount);

                        Console.WriteLine("Most frequent 7-character word: " + "'" + mostFrequent7CharsWord.Word + "'" + " occurred " + max7CharsCount + " times.");
                    }

                    if (listOfScrabble.Any())
                    {
                        var maxScrabbleCount = listOfScrabble.Max(x => x.Count);
                        var maxScrabbleWords = listOfScrabble.FirstOrDefault(x => x.Count == maxScrabbleCount);

                        Console.WriteLine("Highest scoring word(s): " + "'" + maxScrabbleWords.Word + "'" + " occurred " + maxScrabbleCount + " times.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static int GetValueOf(Type enumType, string enumConst, string wordObj) //Reference: https://stackoverflow.com/questions/29938581/how-to-find-enum-value-using-dynamic-enum-name-enum-key-name/29938662
        {
            var returValue = 0;
            try
            {
                if (wordObj != "tête-à-tête")
                {
                    object value = Enum.Parse(enumType, enumConst);
                    returValue = Convert.ToInt32(value);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetValueMethod {ex.Message}" + enumConst);
            }
            return returValue;
        }

        private static char[] RemoveFromArray(char[] source, char value)
        {
            if (source == null)
                return null;

            char[] result = new char[source.Length];

            int resultIdx = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] != value)
                    result[resultIdx++] = source[i];
            }

            return result.Take(resultIdx).ToArray();
        }

        public static string RemoveSpecialCharactersNew(string str)
        {
            char[] buffer = new char[str.Length];
            int index = 0;
            foreach (char c in str)
            {
                if (_lookup[c])
                {
                    buffer[index] = c;
                    index++;
                }
                else
                {
                    buffer[index] = ' ';
                    index++;
                }
            }
            return new string(buffer, 0, index);
        }

        public static string RemoveSpecialCharacters(string input) //Reference: https://stackoverflow.com/questions/4418279/regex-remove-special-characters
        {
            Regex r = new Regex("(?:[^a-z0-9 ]|(?<=['\"])s)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
            return r.Replace(input, String.Empty);
        }

        private static bool inBounds(int index, char[] array)
        {
            return (index >= 0) && (index < array.Length);
        }
    }
}
