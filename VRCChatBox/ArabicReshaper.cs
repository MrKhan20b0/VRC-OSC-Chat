using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;






namespace VRCOSC
{
    internal class ArabicReshaper
    {
        private static readonly Dictionary<string, string> ligatures
        = new Dictionary<string, string>
            {   
                // Lam followed by Alef          
                { "\u0644\u0627", "\uFEFB" },
            };

        // question mark, 
        private static readonly string punctuation = "\u061F";


        private static readonly Dictionary<char, List<char>> letters
        = new Dictionary<char, List<char>>
            {   

                // Lam followed by Alef          { isolated, initial, middle, terminal }
                { '\uFEFB', new List<char> { '\uFEFB', '\0', '\0', '\uFEFC'} },
                

                ///////////////////////////////////////////////////////////////////////////
                // letter   new List<char> { isolated, initial, middle, terminal }
                { '\u0621', new List<char> { '\uFE80', '\0', '\0', '\0'} },

                { '\u0622', new List<char> { '\u0622', '\0', '\0', '\uFE82'} },
                { '\u0623', new List<char> { '\u0623', '\0', '\0', '\uFE84'} },
                { '\u0624', new List<char> { '\u0624', '\0', '\0', '\uFE86'} },
                { '\u0625', new List<char> { '\u0625', '\0', '\0', '\uFE88'} },
                { '\u0626', new List<char> { '\u0626', '\uFE8B', '\uFE8C', '\uFE8A'} },
                { '\u0627', new List<char> { '\u0627', '\0', '\0', '\uFE8E'} },
                { '\u0628', new List<char> { '\u0628', '\uFE91', '\uFE92', '\uFE90'} },
                { '\u0629', new List<char> { '\u0629', '\0', '\0', '\uFE94'} },
                { '\u062A', new List<char> { '\u062A', '\uFE97', '\uFE98', '\uFE96'} },
                { '\u062B', new List<char> { '\u062B', '\uFE9B', '\uFE9C', '\uFE9A'} },
                { '\u062C', new List<char> { '\u062C', '\uFE9F', '\uFEA0', '\uFE9E'} },
                { '\u062D', new List<char> { '\uFEA1', '\uFEA3', '\uFEA4', '\uFEA2'} },
                { '\u062E', new List<char> { '\u062E', '\uFEA7', '\uFEA8', '\uFEA6'} },
                { '\u062F', new List<char> { '\u062F', '\0', '\0', '\uFEAA'} },
                { '\u0630', new List<char> { '\u0630', '\0', '\0', '\uFEAC'} },
                { '\u0631', new List<char> { '\u0631', '\0', '\0', '\uFEAE'} },
                { '\u0632', new List<char> { '\u0632', '\0', '\0', '\uFEB0'} },
                { '\u0633', new List<char> { '\u0633', '\uFEB3', '\uFEB4', '\uFEB2'} },
                { '\u0634', new List<char> { '\u0634', '\uFEB7', '\uFEB8', '\uFEB6'} },
                { '\u0635', new List<char> { '\u0635', '\uFEBB', '\uFEBC', '\uFEBA'} },
                { '\u0636', new List<char> { '\u0636', '\uFEBF', '\uFEC0', '\uFEBE'} },
                { '\u0637', new List<char> { '\u0637', '\uFEC3', '\uFEC4', '\uFEC2'} },
                { '\u0638', new List<char> { '\u0638', '\uFEC7', '\uFEC8', '\uFEC6'} },
                { '\u0639', new List<char> { '\u0639', '\uFECB', '\uFECC', '\uFECA'} },
                { '\u063A', new List<char> { '\u063A', '\uFECF', '\uFED0', '\uFECE'} },
                { '\u0640',  new List<char> { '\u0640', '\u0640', '\u0640', '\u0640' } },
                { '\u0641', new List<char> { '\u0641', '\uFED3', '\uFED4', '\uFED2'} },
                { '\u0642', new List<char> { '\u0642', '\uFED7', '\uFED8', '\uFED6'} },
                { '\u0643', new List<char> { '\u0643', '\uFEDB', '\uFEDC', '\uFEDA'} },
                { '\u0644', new List<char> { '\u0644', '\uFEDF', '\uFEE0', '\uFEDE'} },
                { '\u0645', new List<char> { '\u0645', '\uFEE3', '\uFEE4', '\uFEE2'} },
                { '\u0646', new List<char> { '\u0646', '\uFEE7', '\uFEE8', '\uFEE6'} },
                { '\u0647', new List<char> { '\u0647', '\uFEEB', '\uFEEC', '\uFEEA'} },
                { '\u0648', new List<char> { '\u0648', '\0', '\0', '\uFEEE'} },
                { '\u0649', new List<char> { '\u0649', '\uFBE8', '\uFBE9', '\uFEF0'} },
                { '\u064A', new List<char> { '\u064A', '\uFEF3', '\uFEF4', '\uFEF2'} },
                { '\u0671', new List<char> { '\u0671', '\0', '\0', '\uFB51'} },
                { '\u0677', new List<char> { '\u0677', '\0', '\0', '\0'} },
                { '\u0679', new List<char> { '\u0679', '\uFB68', '\uFB69', '\uFB67'} },
                { '\u067A', new List<char> { '\u067A', '\uFB60', '\uFB61', '\uFB5F'} },
                { '\u067B', new List<char> { '\u067B', '\uFB54', '\uFB55', '\uFB53'} },
                { '\u067E', new List<char> { '\u067E', '\uFB58', '\uFB59', '\uFB57'} },
                { '\u067F', new List<char> { '\u067F', '\uFB64', '\uFB65', '\uFB63'} },
                { '\u0680', new List<char> { '\u0680', '\uFB5C', '\uFB5D', '\uFB5B'} },
                { '\u0683', new List<char> { '\u0683', '\uFB78', '\uFB79', '\uFB77'} },
                { '\u0684', new List<char> { '\u0684', '\uFB74', '\uFB75', '\uFB73'} },
                { '\u0686', new List<char> { '\u0686', '\uFB7C', '\uFB7D', '\uFB7B'} },
                { '\u0687', new List<char> { '\u0687', '\uFB80', '\uFB81', '\uFB7F'} },
                { '\u0688', new List<char> { '\u0688', '\0', '\0', '\uFB89'} },
                { '\u068C', new List<char> { '\u068C', '\0', '\0', '\uFB85'} },
                { '\u068D', new List<char> { '\u068D', '\0', '\0', '\uFB83'} },
                { '\u068E', new List<char> { '\u068E', '\0', '\0', '\uFB87'} },
                { '\u0691', new List<char> { '\u0691', '\0', '\0', '\uFB8D'} },
                { '\u0698', new List<char> { '\u0698', '\0', '\0', '\uFB8B'} },
                { '\u06A4', new List<char> { '\u06A4', '\uFB6C', '\uFB6D', '\uFB6B'} },
                { '\u06A6', new List<char> { '\u06A6', '\uFB70', '\uFB71', '\uFB6F'} },
                { '\u06A9', new List<char> { '\u06A9', '\uFB90', '\uFB91', '\uFB8F'} },
                { '\u06AD', new List<char> { '\u06AD', '\uFBD5', '\uFBD6', '\uFBD4'} },
                { '\u06AF', new List<char> { '\u06AF', '\uFB94', '\uFB95', '\uFB93'} },
                { '\u06B1', new List<char> { '\u06B1', '\uFB9C', '\uFB9D', '\uFB9B'} },
                { '\u06B3', new List<char> { '\u06B3', '\uFB98', '\uFB99', '\uFB97'} },
                { '\u06BA', new List<char> { '\u06BA', '\0', '\0', '\uFB9F'} },
                { '\u06BB', new List<char> { '\u06BB', '\uFBA2', '\uFBA3', '\uFBA1'} },
                { '\u06BE', new List<char> { '\u06BE', '\uFBAC', '\uFBAD', '\uFBAB'} },
                { '\u06C0', new List<char> { '\u06C0', '\0', '\0', '\uFBA5'} },
                { '\u06C1', new List<char> { '\u06C1', '\uFBA8', '\uFBA9', '\uFBA7'} },
                { '\u06C5', new List<char> { '\u06C5', '\0', '\0', '\uFBE1'} },
                { '\u06C6', new List<char> { '\u06C6', '\0', '\0', '\uFBDA'} },
                { '\u06C7', new List<char> { '\u06C7', '\0', '\0', '\uFBD8'} },
                { '\u06C8', new List<char> { '\u06C8', '\0', '\0', '\uFBDC'} },
                { '\u06C9', new List<char> { '\u06C9', '\0', '\0', '\uFBE3'} },
                { '\u06CB', new List<char> { '\u06CB', '\0', '\0', '\uFBDF'} },
                { '\u06CC', new List<char> { '\u06CC', '\uFBFE', '\uFBFF', '\uFBFD'} },
                { '\u06D0', new List<char> { '\u06D0', '\uFBE6', '\uFBE7', '\uFBE5'} },
                { '\u06D2', new List<char> { '\u06D2', '\0', '\0', '\uFBAF'} },
                { '\u06D3', new List<char> { '\u06D3', '\0', '\0', '\uFBB1'} },
            };



        private static void processLetterInfo(char c, int indexToLetterInfo, StringBuilder shapedString)
        {
            // Now now position of character, add correctly shapped letter to shapped string
            if (letters.TryGetValue(c, out List<char> letterInfo))
            {
                if (letterInfo[indexToLetterInfo] != '\0')
                {
                    shapedString.Append(letterInfo[indexToLetterInfo]);
                }
                else
                {
                    shapedString.Append(letterInfo[0]);
                }
            }
            else
            {
                shapedString.Append(c);
            }
        }

        private static bool IsWhiteSpaceOrPunctuation(char c)
        {
            return Char.IsWhiteSpace(c) || punctuation.Contains(c);
        }

        private static string ReshapeLetters(string initString)
        {
            StringBuilder shapedString = new();

            // initial char
            processLetterInfo(initString[0], 1, shapedString);


            // Handle all other characters
            for (int i = 1; i < initString.Length - 1; i++)
            {
                // Ignore white space characters
                if (!IsWhiteSpaceOrPunctuation(initString[i]))
                {
                    int indexToLetterInfo = 0;

                    // Initial character if space is before it
                    if (IsWhiteSpaceOrPunctuation(initString[i - 1]))
                    {
                        indexToLetterInfo = 1;
                    }

                    // last character if space is after it
                    else if (IsWhiteSpaceOrPunctuation(initString[i + 1]))
                    {

                        // if previous character does not connect, we are isolated form
                        if (letters.TryGetValue(initString[i - 1], out List<char> letterInfo))
                        {
                            if (letterInfo[1] != '\0' && letterInfo[2] != '\0')
                                indexToLetterInfo = 3;
                            else
                            {
                                indexToLetterInfo = 0;
                            }

                        }
                    }

                    // middle character otherwise
                    else
                    {
                        bool previousConnected = true;

                        // if previous character does not connect, we are initial form
                        if (letters.TryGetValue(initString[i - 1], out List<char> letterInfo))
                        {
                            if (letterInfo[1] != '\0' && letterInfo[2] != '\0')
                                indexToLetterInfo = 2;
                            else
                            {
                                indexToLetterInfo = 1;
                                previousConnected = false;
                            }

                        }

                        // If currenct character does not connect, we are final form.
                        if (previousConnected && letters.TryGetValue(initString[i], out letterInfo))
                        {
                            if (letterInfo[1] == '\0' && letterInfo[2] == '\0')
                                indexToLetterInfo = 3;
                        }

                        // If the next character does not have a final form, like hamza, we are final form or isolated.
                        char nextChar = initString[i + 1];
                        if (!char.IsWhiteSpace(nextChar))
                        {
                            letters.TryGetValue(nextChar, out letterInfo);
                            bool nextCharConnects = letterInfo[3] != '\0';

                            if (!nextCharConnects)
                            {
                                letters.TryGetValue(initString[i], out letterInfo);

                                // Next does not connect, but previous char does, we are final form
                                if (previousConnected && letterInfo[3] != '\0')
                                {
                                    indexToLetterInfo = 3;
                                }
                                // Otherwise we are isolated.
                                else
                                {
                                    indexToLetterInfo = 0;
                                }
                            }
                            
                        }
                    }


                    // Now now position of character, add correctly shapped letter to shapped string
                    processLetterInfo(initString[i], indexToLetterInfo, shapedString);
                }
                else
                {
                    shapedString.Append(initString[i]);
                }

            }

            // process last char
            if (initString.Length > 1)
            {
                // if previous character does not connect, we are isolated form
                if (letters.TryGetValue(initString[initString.Length - 2], out List<char> letterInfo))
                {
                    if (letterInfo[1] != '\0' && letterInfo[2] != '\0')
                        processLetterInfo(initString[initString.Length - 1], 3, shapedString);
                    else
                        processLetterInfo(initString[initString.Length - 1], 0, shapedString);
                }
                else
                {
                    shapedString.Append(initString[initString.Length - 1]);
                }
            }
                

            return shapedString.ToString();
        }

        private static string ReshapeLigatures(string initString)
        {
            // TODO: This is not efficient at all, but CPUs are fast enough and the processed strings are tiny
            foreach (KeyValuePair<string, string> entry in ligatures)
            {
               initString =  initString.Replace(entry.Key, entry.Value);
            }

            return initString;
        }

        public static string Reshape(string initString)
        {
            if (initString.Length <= 0)
            {
                return "";
            }

            string s = ReshapeLigatures(initString);
            s = ReshapeLetters(s);

            System.Diagnostics.Debug.WriteLine("S", s);

            char[] charArray = s.ToCharArray();

            System.Diagnostics.Debug.WriteLine("CharArray", charArray);
            Array.Reverse(charArray);
            System.Diagnostics.Debug.WriteLine("CharArray", charArray);
            return new string(charArray);
        }
    }
}
