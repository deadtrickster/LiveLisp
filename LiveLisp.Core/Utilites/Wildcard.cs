using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveLisp.Core.Utilites
{
    public class Wildcard
    {
        /// <summary>
        /// http://untweaked.wordpress.com/2008/06/03/wildcard-matching-in-c/#comment-19
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="input"></param>
        /// <param name="caseInsensitive"></param>
        /// <returns></returns>
        public static bool IsMatch(string pattern, string input, bool caseInsensitive)
        {
            if (pattern == "*")
                return true;

            int offsetInput = 0;
            bool isAsterix = false;
            int i;

            while (true)
            {
                for (i = 0; i < pattern.Length; )
                {

                    switch (pattern[i])
                    {
                        case '?':
                            isAsterix = false;
                            offsetInput++;
                            break;
                        case '*':
                            isAsterix = true;
                            while (i < pattern.Length &&
                                        pattern[i] == '*')
                            {
                                i++;
                            }

                            if (i >= pattern.Length)
                                return true;
                            continue;
                        default:
                            if (offsetInput >= input.Length)
                                return false;

                            if ((caseInsensitive ?
                                   char.ToLower(input[offsetInput]) :
                                   input[offsetInput])
                                !=
                                (caseInsensitive ?
                                   char.ToLower(pattern[i]) :
                                   pattern[i]))
                            {
                                if (!isAsterix)
                                    return false;
                                offsetInput++;
                                continue;
                            }
                            offsetInput++;
                            break;
                    } // end switch
                    i++;
                } // end for

                // have we finished parsing our input?
                if (i > input.Length)
                    return false;

                // do we have any lingering asterixes we need to skip?
                while (i < pattern.Length && pattern[i] == '*')
                    ++i;

                // final evaluation. The index should be pointing at the
                // end of the string.
                return (offsetInput == input.Length);
            }

        }
    }
}
