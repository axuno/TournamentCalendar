using System;
using System.Collections.Generic;

namespace TournamentCalendar.Collectors;

public static class StringExtensions
{
    /// <summary>
    /// This implements an allocation-free string creation to construct the path.
    /// This uses 3.5x LESS memory and is 2x faster than some alternate methods (StringBuilder, interpolation, string.Concat, etc.).
    /// </summary>
    /// <param name="str"></param>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string ConcatPath(this string str, params string[] paths)
    {
        const char separator = '/';
        if (str == null) throw new ArgumentNullException(nameof(str));

        var list = new List<ReadOnlyMemory<char>>();
        var first = str.AsMemory().TrimEnd(separator);

        // get length for initial string after it's trimmed
        var length = first.Length;
        list.Add(first);

        foreach (var path in paths)
        {
            var newPath = path.AsMemory().Trim(separator);
            length += newPath.Length + 1;
            list.Add(newPath);
        }

        var newString = string.Create(length, list, (chars, state) =>
        {
            // NOTE: We don't access the 'list' variable in this delegate since 
            // it would cause a closure and allocation. Instead we access the state parameter.

            // track our position within the string data we are populating
            var position = 0;

            // copy the first string data to index 0 of the Span<char>
            state[0].Span.CopyTo(chars);

            // update the position to the new length
            position += state[0].Span.Length;

            // start at index 1 when slicing
            for (var i = 1; i < state.Count; i++)
            {
                // add a separator in the current position and increment position by 1
                chars[position++] = separator;

                // copy each path string to a slice at current position
                state[i].Span.CopyTo(chars.Slice(position));

                // update the position to the new length
                position += state[i].Length;
            }
        });
        return newString;
    }
}
