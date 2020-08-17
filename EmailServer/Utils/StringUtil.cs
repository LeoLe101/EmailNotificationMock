using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace EmailServer
{
    /// <summary>
    /// String manipulation helpers
    /// </summary>
    public static class StringUtil
    {
        public static bool AreEqual(string input1, string input2, bool ignoreCase = true)
        {
            // Both input must be actual strings to be considered eq
            if ((input1 == null) || (input2 == null))
            {
                return false;
            }

            if (ignoreCase)
            {
                input1 = !String.IsNullOrWhiteSpace(input1) ? input1.ToUpper() : null;
                input2 = !String.IsNullOrWhiteSpace(input2) ? input2.ToUpper() : null;
            }

            return input1?.Trim() == input2?.Trim();
        }

        // NOTE: There is a bug with using EmailAddressAttribute & MailAddress
        // Details here - https://github.com/dotnet/corefx/issues/28501
        // Alt using MailAddress - https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        // Alt using RegEx - https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        public static bool IsValidEmail(string input)
        {
            return new EmailAddressAttribute().IsValid(input);
        }

        public static bool IsValidPhone(string input)
        {
            return new PhoneAttribute().IsValid(input);
        }

        public static string ToNameCase(string input)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                string inputRes = "";

                // Handles the white space case
                string[] inputSegment = input.Split(" ");

                // Fence-post
                bool initPass = true;
                foreach (string i in inputSegment)
                {
                    string iterStr = StringUtil._ToNameCaseHelper(i);

                    if (initPass)
                    {
                        initPass = false;
                    }
                    else
                    {
                        inputRes += " ";
                    }

                    inputRes += iterStr;
                }

                return inputRes;
            }
            else
            {
                // null, undefined, or empty, just return back the original string
                return input;
            }
        }

        public static string GetFullName(string firstName, string lastName)
        {
            firstName = !String.IsNullOrWhiteSpace(firstName) ? firstName.Trim() : "";
            lastName = !String.IsNullOrWhiteSpace(lastName) ? lastName.Trim() : "";
            string fullName = $"{firstName} {lastName}";
            return StringUtil.ToNameCase(fullName.Trim());
        }

        //  Used to generate unique IDs that are sensitive to capitalization
        //  Currently this returns both lower and upper case + numbers
        //  5 chars will give you 62^5 unique IDs = 916,132,832 (~1 billion)
        //  6 chars will give you 62^6 unique IDs = 56,800,235,584 (56+ billion)
        //  This is used by tinyurl & bit.ly
        public static string GenerateRandStrBase62(int length)
        {
            return StringUtil._GenerateRandString(61, length);
        }

        //  Used to generate unique IDs that are insensitive to capitalization
        //  Currently this returns all lower case alphabets + numbers
        //  6 chars will give you 36^6 unique IDs = 2,176,782,336 (2+ billion)
        //  7 chars will give you 36^7 unique IDs = 78,364,164,096 (78+ billion)
        //  This should be used for loan app human readable IDs. We do not need to
        //  be unique for all loan apps, just unique within a FIN.
        public static string GenerateRandStrBase36(int length)
        {
            return StringUtil._GenerateRandString(35, length);
        }


        #region Helpers

        private static string _ToNameCaseHelper(string input)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                string inputRes = "";

                // Handles the Mary-Anne case
                string[] inputSegment = input.Split("-");

                // Fence-post
                bool initPass = true;
                foreach (string i in inputSegment)
                {
                    string inputTemp = "";

                    if (i.Length == 0)
                    {
                        // Nothing to do
                    }
                    else if (i.Length == 1)
                    {
                        // A, B, C, etc.
                        inputTemp = i.ElementAt(0).ToString().ToUpper();
                    }
                    else
                    {
                        inputTemp = i.ElementAt(0).ToString().ToUpper();
                        inputTemp += i.Substring(1).ToLower();
                    }

                    if (initPass)
                    {
                        initPass = false;
                    }
                    else
                    {
                        inputRes += "-";
                    }

                    inputRes += inputTemp;
                }

                return inputRes;
            }
            else
            {
                // null, undefined, or empty, just return back the original string
                return input;
            }
        }

        //  Defaults to base 36 w/ len of 7 for 78+ billion unique IDs
        //  Base 36 ensures we do not have to worrry about capitalization
        //  variation
        private static string _GenerateRandString(int targetBase = 35, int length = 7)
        {
            // Init char set
            char[] base62chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            // Init random seeder
            Random random = new Random();
            // Init string builder
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(base62chars[random.Next(targetBase)]);
            }

            return sb.ToString();
        }

        #endregion
    }
}
