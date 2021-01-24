using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NYTSpellingBee
{
    class Program
    {
        static string[] Solution1(char required_letter, char[] all_letters, string[] dictionary)
        {
            var answers = new List<string>();
            foreach (var word in dictionary)
            {
                bool CheckWord(string word)
                {
                    if (word.Length >= 4 && word.Contains(required_letter))
                    {
                        // Does this word only contain the available letters?
                        foreach (var letter in word)
                        {
                            if (!all_letters.Contains(letter))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                if (CheckWord(word))
                {
                    answers.Add(word);
                }
            }
            return answers.ToArray();
        }

        static string[] Solution2(char required_letter, char[] all_letters, string[] dictionary)
        {
            var dictdict = dictionary.ToHashSet();

            bool GetNextWord(char[] chars)
            {
                int incPosition = chars.Length - 1;
                char lastLetter;
                int lastLetterIndex;

                // Step 1: find which position to increment
                while (true)
                {
                    lastLetter = chars[incPosition];
                    lastLetterIndex = Array.IndexOf(all_letters, lastLetter);

                    // it's not max, can increment current position without shifting
                    if (lastLetterIndex < all_letters.Length - 1) break;

                    // This is the highest order letter
                    if (incPosition == 0) break;

                    // Shift! Set current position to 0-value and try advance to the next position
                    chars[incPosition] = all_letters[0];
                    incPosition--;
                }

                // Step 2. Increment the value at the found position
                if (lastLetter != all_letters[^1])
                {
                    chars[incPosition] = all_letters[lastLetterIndex + 1];
                }
                else
                {
                    return false; // no more
                }

                return true;
            }

            var answers = new List<string>();
            for(int wordlen = 4; wordlen <= 10; wordlen++)
            {
                var buffer = new char[wordlen];
                for (int i = 0; i < wordlen; i++) buffer[i] = all_letters[0];

                do
                {
                    //Console.WriteLine(new string(buffer));
                    if (buffer.Contains(required_letter))
                    {
                        var word = new string(buffer);
                        if (dictdict.Contains(word))
                        {
                            answers.Add(word);
                        }
                    }
                } while (GetNextWord(buffer));
            }

            return answers.ToArray();
        }

        static void PrintAnswers(IEnumerable<string> answers)
        {
            var groups = answers.GroupBy(w => w[0]);
            foreach (var group in groups)
            {
                var sameStartingLetter = String.Join(',', group.OrderBy(s => s.Length));
                Console.WriteLine($" {sameStartingLetter}");
            }
        }

        static void Main()
        {
            var dictionary = File.ReadAllLines("words-short.txt");

            var required_letter = 'b';
            var all_letters = new char[] { 'b', 'o', 'n', 'l', 'p', 'a', 'e' };

            var stopwatch = Stopwatch.StartNew();

            var answers = Solution1(required_letter, all_letters, dictionary);

            stopwatch.Stop();

            Console.WriteLine($"Words found in the main dictionary:");

            PrintAnswers(answers);

            Console.WriteLine($"Elapsed time for solution 1: {stopwatch.Elapsed}");

            // Find additoinal possible solutions from a larger dictionary
            var dictionary2 = File.ReadAllLines("words2.txt");
            var answersBig = Solution1(required_letter, all_letters, dictionary2);

            var extras = answersBig.Where(elem => !answers.Contains(elem));
            
            Console.WriteLine($"Additional words found in the large dictionary:");
            PrintAnswers(extras);

            Console.WriteLine($"Now trying the slow, complex and possibly incorrect solution...");

            var stopwatchSlow = Stopwatch.StartNew();

            var answersSlow = Solution2(required_letter, all_letters, dictionary);

            stopwatchSlow.Stop();

            if (!answers.OrderBy(s => s).SequenceEqual(answersSlow.OrderBy(s => s)))
            {
                Console.WriteLine($"Error! Results are different");
            }

            Console.WriteLine($"Elapsed time for solution 2: {stopwatchSlow.Elapsed}");
        }
    }
}
