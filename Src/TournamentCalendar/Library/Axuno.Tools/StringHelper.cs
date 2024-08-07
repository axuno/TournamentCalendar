using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;

/*
 *Based on C# String Library by Chad Finsterwald, http://www.wtfdeveloper.com/Default5.aspx
 */

namespace Axuno.Tools.String;

/// <summary>
/// Helper functions for String not already found in C#.
/// Inspired by PHP String Functions that are missing in .Net.
/// </summary>
public static class StringHelper
{
    // The timeout for the regular expression match in milliseconds.
    private static int _timeout = 1000;

    /// <summary>
    /// Base64 encodes a string.
    /// </summary>
    /// <param name="input">A string</param>
    /// <returns>A base64 encoded string</returns>
    public static string Base64StringEncode(string input)
    {
        var encbuff = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(encbuff);
    }

    /// <summary>
    /// Base64 decodes a string.
    /// </summary>
    /// <param name="input">A base64 encoded string</param>
    /// <returns>A decoded string</returns>
    public static string Base64StringDecode(string input)
    {
        var decbuff = Convert.FromBase64String(input);
        return System.Text.Encoding.UTF8.GetString(decbuff);
    }

    /// <summary>
    /// A case-sensitive replace function.
    /// </summary>
    /// <param name="input">The string to examine.</param>
    /// <param name="newValue">The value to replace.</param>
    /// <param name="oldValue">The new value to be inserted</param>
    /// <returns>A string</returns>
    public static string CaseInsenstiveReplace(string input, string newValue, string oldValue)
    {
        var regEx = new Regex(oldValue, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(_timeout));
        return regEx.Replace(input, newValue);
    }

    /// <summary>
    /// Removes all the words passed in the filter words parameters. Replacing is NOT
    /// case-sensitive.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <param name="filterWords">The words to replace in the input string.</param>
    /// <returns>A string.</returns>
    public static string FilterWords(string input, params string[] filterWords)
    {
        return StringHelper.FilterWords(input, char.MinValue, filterWords);
    }

    /// <summary>
    /// Removes all the words passed in the filter words parameters. Replacing is NOT
    /// case-sensitive.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <param name="mask">A character that is inserted for each letter of the replaced word.</param>
    /// <param name="filterWords">The words to replace in the input string.</param>
    /// <returns>A string.</returns>
    public static string FilterWords(string input, char mask, params string[] filterWords)
    {
        var stringMask = mask == char.MinValue ? string.Empty : mask.ToString();
        var totalMask = stringMask;

        foreach (var s in filterWords)
        {
            var regEx = new Regex(s, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(_timeout));

            if (stringMask.Length > 0)
            {
                for (var i = 1; i < s.Length; i++)
                    totalMask += stringMask;
            }

            input = regEx.Replace(input, totalMask);

            totalMask = stringMask;
        }

        return input;
    }

    /// <summary>
    /// Checks the passed string to see if has any of the passed words. Not case-sensitive.
    /// </summary>
    /// <param name="input">The string to check.</param>
    /// <param name="hasWords">The words to check for.</param>
    /// <returns>A collection of the matched words.</returns>
    public static MatchCollection HasWords(string input, params string[] hasWords)
    {
        var sb = new StringBuilder(hasWords.Length + 50);
        //sb.Append("[");

        foreach (var s in hasWords)
        {
            sb.AppendFormat("({0})|", StringHelper.HtmlSpecialEntitiesEncode(s.Trim()));
        }

        var pattern = sb.ToString();
        pattern = pattern.TrimEnd('|'); // +"]";

        var regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(_timeout));
        return regEx.Matches(input);
    }

    /// <summary>
    /// A wrapper around HttpUtility.HtmlEncode
    /// </summary>
    /// <param name="input">The string to be encoded</param>
    /// <returns>An encoded string</returns>
    public static string HtmlSpecialEntitiesEncode(string input)
    {
        return HttpUtility.HtmlEncode(input);
    }

    /// <summary>
    /// A wrapper around HttpUtility.HtmlDecode
    /// </summary>
    /// <param name="input">The string to be decoded</param>
    /// <returns>The decode string</returns>
    public static string HtmlSpecialEntitiesDecode(string input)
    {
        return HttpUtility.HtmlDecode(input);
    }

    /// <summary>
    /// MD5 encodes the passed string
    /// </summary>
    /// <param name="input">The string to encode.</param>
    /// <returns>An encoded string.</returns>
    public static string MD5String(string input)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.
        var md5Hasher = MD5.Create();

        // Convert the input string to a byte array and compute the hash.
        var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

        // Create a new StringBuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (var i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    /// <summary>
    /// Verified a string against the passed MD5 hash.
    /// </summary>
    /// <param name="input">The string to compare.</param>
    /// <param name="hash">The hash to compare against.</param>
    /// <returns>True if the input and the hash are the same, false otherwise.</returns>
    public static bool MD5VerifyString(string input, string hash)
    {
        // Hash the input.
        var hashOfInput = StringHelper.MD5String(input);

        // Create a StringComparer and compare the hashes.
        var comparer = StringComparer.OrdinalIgnoreCase;

        if (0 == comparer.Compare(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Left pads the passed input using the HTML non-breaking string entity (&nbsp;)
    /// for the total number of spaces.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="totalSpaces">The total number to pad the string.</param>
    /// <returns>A padded string.</returns>
    public static string PadLeftHtmlSpaces(string input, int totalSpaces)
    {
        var space = "&nbsp;";
        return PadLeft(input, space, totalSpaces * space.Length);
    }

    /// <summary>
    /// Left pads the passed input using the passed pad string
    /// for the total number of spaces.  It will not cut-off the pad even if it 
    /// causes the string to exceed the total width.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="pad">The string to use for padding.</param>
    /// <returns>A padded string.</returns>
    public static string PadLeft(string input, string pad, int totalWidth)
    {
        return StringHelper.PadLeft(input, pad, totalWidth, false);
    }

    /// <summary>
    /// Left pads the passed input using the passed pad string
    /// for the total number of spaces.  It will cut-off the pad so that  
    /// the string does not exceed the total width.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="pad">The string to use for padding.</param>
    /// <returns>A padded string.</returns>
    public static string PadLeft(string input, string pad, int totalWidth, bool cutOff)
    {
        if (input.Length >= totalWidth)
            return input;

        var padCount = pad.Length;
        var paddedString = input;

        while (paddedString.Length < totalWidth)
        {
            paddedString += pad;
        }

        // trim the excess.
        if (cutOff)
            paddedString = paddedString.Substring(0, totalWidth);

        return paddedString;
    }

    /// <summary>
    /// Right pads the passed input using the HTML non-breaking string entity (&nbsp;)
    /// for the total number of spaces.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="totalSpaces">The total number to pad the string.</param>
    /// <returns>A padded string.</returns>
    public static string PadRightHtmlSpaces(string input, int totalSpaces)
    {
        var space = "&nbsp;";
        return PadRight(input, space, totalSpaces * space.Length);
    }

    /// <summary>
    /// Right pads the passed input using the passed pad string
    /// for the total number of spaces.  It will not cut-off the pad even if it 
    /// causes the string to exceed the total width.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="pad">The string to uses as padding.</param>
    /// <returns>A padded string.</returns>
    public static string PadRight(string input, string pad, int totalWidth)
    {
        return StringHelper.PadRight(input, pad, totalWidth, false);
    }

    /// <summary>
    /// Right pads the passed input using the passed pad string
    /// for the total number of spaces.  It will cut-off the pad so that  
    /// the string does not exceed the total width.
    /// </summary>
    /// <param name="input">The string to pad.</param>
    /// <param name="pad">The string to uses as padding.</param>
    /// <returns>A padded string.</returns>
    public static string PadRight(string input, string pad, int totalWidth, bool cutOff)
    {
        if (input.Length >= totalWidth)
            return input;

        var paddedString = string.Empty;

        while (paddedString.Length < totalWidth - input.Length)
        {
            paddedString += pad;
        }

        // trim the excess.
        if (cutOff)
            paddedString = paddedString.Substring(0, totalWidth - input.Length);

        paddedString += input;

        return paddedString;
    }

    /// <summary>
    /// Removes the new line (\n) and carriage return (\r) symbols.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <returns>A string</returns>
    public static string RemoveNewLines(string input)
    {
        return StringHelper.RemoveNewLines(input, false);
    }

    /// <summary>
    /// Removes the new line (\n) and carriage return (\r) symbols.
    /// </summary>
    /// <param name="input">The string to search.</param>
    /// <param name="addSpace">If true, adds a space (" ") for each newline and carriage
    /// return found.</param>
    /// <returns>A string</returns>
    public static string RemoveNewLines(string input, bool addSpace)
    {
        var replace = string.Empty;
        if (addSpace)
            replace = " ";

        var pattern = @"[\r|\n]";
        var regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline, TimeSpan.FromMilliseconds(_timeout));

        return regEx.Replace(input, replace);
    }

    /// <summary>
    /// Reverse a string.
    /// </summary>
    /// <param name="input">The string to reverse</param>
    /// <returns>A string</returns>
    public static string Reverse(string input)
    {
        if (input.Length <= 1)
            return input;

        var c = input.ToCharArray();
        var sb = new StringBuilder(c.Length);
        for (var i = c.Length - 1; i > -1; i--)
            sb.Append(c[i]);

        return sb.ToString();
    }

    /// <summary>
    /// Converts a string to sentence case.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>A string</returns>
    public static string SentenceCase(string input)
    {
        if (input.Length < 1)
            return input;

        var sentence = input.ToLower();
        return sentence[0].ToString().ToUpper() + sentence.Substring(1);
    }

    /// <summary>
    /// Converts all spaces to HTML non-breaking spaces
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>A string</returns>
    public static string SpaceToNbsp(string input)
    {
        var space = "&nbsp;";
        return input.Replace(" ", space);
    }

    /// <summary>
    /// Removes all HTML tags from the passed string
    /// </summary>
    /// <param name="input">The string whose values should be replaced.</param>
    /// <returns>A string.</returns>
    public static string StripTags(string input)
    {
        var stripTags = new Regex("<(.|\n)+?>", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
        return stripTags.Replace(input, "");
    }

    /// <summary>
    /// Removes all HTML entities from the passed string
    /// </summary>
    /// <param name="input">The string whose values should be replaced.</param>
    /// <param name="retainSpace">true, if entities shall be replaced by spaces.</param>
    /// <returns>A string.</returns>
    public static string StripEntities(string input, bool retainSpace)
    {
        var replacement = string.Empty;
        if (retainSpace)
        {
            replacement = " ";
        }
        return Regex.Replace(input, "&[^;]*;", replacement, RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
    }


    /// <summary>
    /// Removes all whitespace from the passed string.
    /// </summary>
    /// <param name="input">The string whose values should be replaced.</param>
    /// <param name="retainSpace">true, if multiple spaces shall be replaced by single spaces.</param>
    /// <returns></returns>
    public static string StripWhiteSpace(string input, bool retainSpace)
    {
        var replacement = string.Empty;
        if (retainSpace)
        {
            replacement = " ";
        }
        return Regex.Replace(input, @"\s+", replacement, RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
    }


    /// <summary>
    /// Converts a string to title case.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>A string.</returns>
    public static string TitleCase(string input)
    {
        return TitleCase(input, true);
    }

    /// <summary>
    /// Converts a string to title case.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="ignoreShortWords">If true, does not capitalize words like
    /// "a", "is", "the", etc.</param>
    /// <returns>A string.</returns>
    public static string TitleCase(string input, bool ignoreShortWords)
    {
        List<string> ignoreWords = new();
        if (ignoreShortWords)
        {
            //TODO: Add more ignore words?
            ignoreWords = new List<string>
            {
                "a",
                "is",
                "was",
                "the"
            };
        }

        var tokens = input.Split(' ');
        var sb = new StringBuilder(input.Length);
        foreach (var s in tokens)
        {
            if (ignoreShortWords == true
                && s != tokens[0]
                && ignoreWords.Contains(s.ToLower()))
            {
                sb.Append(s + " ");
            }
            else
            {
                sb.Append(s[0].ToString().ToUpper());
                sb.Append(s.Substring(1).ToLower());
                sb.Append(" ");
            }
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Removes multiple spaces between words
    /// </summary>
    /// <param name="input">The string to trim.</param>
    /// <returns>A string.</returns>
    public static string TrimIntraWords(string input)
    {
        var regEx = new Regex(@"[\s]+", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
        return regEx.Replace(input, " ");
    }

    /// <summary>
    /// Converts new line(\n) and carriage return(\r) symbols to
    /// HTML line breaks.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <returns>A string.</returns>
    public static string NewLineToBreak(string input)
    {
        var regEx = new Regex(@"[\n|\r]+", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
        return regEx.Replace(input, "<br />");
    }

    /// <summary>
    /// Wraps the passed string up the 
    /// until the next whitespace on or after the total charCount has been reached
    /// for that line.  Uses the environment new line
    /// symbol for the break text.
    /// </summary>
    /// <param name="input">The string to wrap.</param>
    /// <param name="charCount">The number of characters per line.</param>
    /// <returns>A string.</returns>
    public static string WordWrap(string input, int charCount)
    {
        return StringHelper.WordWrap(input, charCount, false, Environment.NewLine);
    }

    /// <summary>
    /// Wraps the passed string up the total number of characters (if cuttOff is true)
    /// or until the next whitespace (if cutOff is false).  Uses the environment new line
    /// symbol for the break text.
    /// </summary>
    /// <param name="input">The string to wrap.</param>
    /// <param name="charCount">The number of characters per line.</param>
    /// <param name="cutOff">If true, will break in the middle of a word.</param>
    /// <returns>A string.</returns>
    public static string WordWrap(string input, int charCount, bool cutOff)
    {
        return StringHelper.WordWrap(input, charCount, cutOff, Environment.NewLine);
    }

    /// <summary>
    /// Wraps the passed string up the total number of characters (if cuttOff is true)
    /// or until the next whitespace (if cutOff is false).  Uses the passed breakText
    /// for lineBreaks.
    /// </summary>
    /// <param name="input">The string to wrap.</param>
    /// <param name="charCount">The number of characters per line.</param>
    /// <param name="cutOff">If true, will break in the middle of a word.</param>
    /// <param name="breakText">The line break text to use.</param>
    /// <returns>A string.</returns>
    public static string WordWrap(string input, int charCount, bool cutOff,
        string breakText)
    {
        var sb = new StringBuilder(input.Length + 100);
        var counter = 0;

        if (cutOff)
        {
            while (counter < input.Length)
            {
                if (input.Length > counter + charCount)
                {
                    sb.Append(input.AsSpan(counter, charCount));
                    sb.Append(breakText);
                }
                else
                {
                    sb.Append(input.AsSpan(counter));
                }
                counter += charCount;
            }
        }
        else
        {
            string[] strings = input.Split(' ');
            for (var i = 0; i < strings.Length; i++)
            {
                counter += strings[i].Length + 1; // the added one is to represent the inclusion of the space.
                if (i != 0 && counter > charCount)
                {
                    sb.Append(breakText);
                    counter = 0;
                }

                sb.Append(strings[i] + ' ');
            }
        }
        return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
    }

    /// <summary>
    /// Computes the left part of the input string up to the required length,
    /// but with a maximum of the original string length.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="length">The length of the left string part.</param>
    /// <returns></returns>
    public static string Left(string input, int length)
    {
        return input.Substring(0, length > input.Length ? input.Length : length);
    }

    /// <summary>
    /// Computes the right part of the input string up to the required length,
    /// but with a maximum of the original string length.
    /// </summary>
    /// <param name="input">The string to convert.</param>
    /// <param name="length">The length of the right string part.</param>
    /// <returns></returns>
    public static string Right(string input, int length)
    {
        return input.Substring(length > input.Length ? 0 : input.Length - length);
    }


    /// <summary>
    /// Method to make sure that user's inputs are not malicious
    /// </summary>
    /// <param name="text">User's Input</param>
    /// <param name="maxLength">Maximum length of input</param>
    /// <returns>The cleaned up version of the input</returns>
    public static string CleanInputText(string text, int maxLength)
    {
        text = text.Trim();
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        if (text.Length > maxLength)
            text = text.Substring(0, maxLength);
        text = Regex.Replace(text, "[\\s]{2,}", " ", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));	// two or more spaces
        text = String.StringHelper.StripTags(text);     // HTML tages
        text = String.StringHelper.StripEntities(text, false);  // HTML entities
        return text;
    }

    public static string CleanInputText(string text)
    {
        return CleanInputText(text, text.Length);
    }

    public static bool IsNumber(string strNumber)
    {
        return new Regex(@"^([0-9])[0-9]*(\.\w*)?$", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout)).IsMatch(strNumber);
    }

    public static bool IsSafeSqlString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
    }

    public static bool IsTime(string timeval)
    {
        return Regex.IsMatch(timeval, "^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
    }

    public static bool IsDate (string dateval)
    {
        return IsDate(dateval, System.Threading.Thread.CurrentThread.CurrentCulture);
    }

    public static bool IsDate(string dateval, System.Globalization.CultureInfo culture)
    {
        DateTime date;
        return DateTime.TryParse(dateval, culture,
            System.Globalization.DateTimeStyles.None, out date);

    }

    public static bool IsValidEmail(string input)
    {
        return Regex.IsMatch(input, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout));
    }

    public static float StrToFloat(object? strValue, float defValue)
    {
        if ((strValue == null) || (strValue.ToString()?.Length > 10))
        {
            return defValue;
        }
        var single1 = defValue;
        if (new Regex(@"^([-]|[0-9])[0-9]*(\.\w*)?$", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout)).IsMatch(strValue.ToString() ?? string.Empty))
        {
            single1 = Convert.ToSingle(strValue);
        }
        return single1;
    }

    public static int StrToInt(object? strValue, int defValue)
    {
        if ((strValue == null) || (strValue.ToString()?.Length > 9))
        {
            return defValue;
        }
        var num1 = defValue;
        if (new Regex("^([-]|[0-9])[0-9]*$", RegexOptions.None, TimeSpan.FromMilliseconds(_timeout)).IsMatch(strValue.ToString() ?? string.Empty))
        {
            num1 = Convert.ToInt32(strValue);
        }
        return num1;
    }

    public static DataTable ConvertHtmlTableToDataTable(string html)
    {
        // Declarations
        var dt = new DataTable();
        const string tableExpression = "<table[^>]*>(.*?)</table>";
        const string headerExpression = "<th[^>]*>(.*?)</th>";
        const string rowExpression = "<tr[^>]*>(.*?)</tr>";
        const string columnExpression = "<td[^>]*>(.*?)</td>";
            
        // Get a match for all the tables in the HTML
        var tableMatches = Regex.Matches(html, tableExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout));
        // Loop through each table element
        foreach (Match tableMatch in tableMatches)
        {
            // Reset the current row counter and the header flag
            var currRow = 0;
            var headerExists = false;
            // Add a new table to the DataSet
            dt = new DataTable();
            // Create the relevant amount of columns for this table (use the headers if they exist, otherwise use default names)
            if (tableMatch.Value.Contains("<th"))
            {
                // Set the HeadersExist flag
                headerExists = true;
                // Get a match for all the rows in the table
                var headerMatches = Regex.Matches(tableMatch.Value, headerExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout));
                // Loop through each header element
                foreach (Match headerMatch in headerMatches)
                {
                    dt.Columns.Add(headerMatch.Groups[1].ToString());
                }
            }
            else
            {
                for (var cols = 1;
                     cols <= Regex
                         .Matches(
                             Regex.Matches(
                                     Regex.Matches(tableMatch.Value, tableExpression,
                                             RegexOptions.Multiline | RegexOptions.Singleline |
                                             RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout))[0]
                                         .ToString(), rowExpression,
                                     RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout))[0]
                                 .ToString(), columnExpression,
                             RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase,
                             TimeSpan.FromMilliseconds(_timeout)).Count;
                     cols++)
                {
                    dt.Columns.Add("Column " + cols);
                }
            }
            // Get a match for all the rows in the table
            var rowMatches = Regex.Matches(tableMatch.Value, rowExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout));
            // Loop through each row element
            foreach (Match rowMatch in rowMatches)
            {
                // Only loop through the row if it isn't a header row
                if (!(currRow == 0 && headerExists))
                {
                    // Create a new row and reset the current column counter
                    var dr = dt.NewRow();
                    var currCol = 0;
                    // Get a match for all the columns in the row
                    var colMatches = Regex.Matches(rowMatch.Value, columnExpression, RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(_timeout));
                    // Loop through each column element
                    foreach (Match colMatch in colMatches)
                    {
                        // Add the value to the DataRow
                        dr[currCol] = StripTags(colMatch.Groups[1].ToString());
                        // Increase the current column
                        currCol++;
                    }
                    // Add the DataRow to the DataTable
                    dt.Rows.Add(dr);
                }
                // Increase the current row counter
                currRow++;
            }
        }
        return dt;
    }
}
