﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NB.Tools.GeoSpatial
{
    /// <summary>Represents a Latitude/Longitude/Altitude coordinate.
	/// Source: http://www.codeproject.com/KB/recipes/geospatial.aspx
	/// </summary>
    public partial class Location
    {
        /// <summary>Allows parsing of strings.</summary>
        private static class Parser
        {
            private const string DegreePattern = @"
^\s*                 # Ignore any whitespace at the start of the string
(?<latSuf>[NS])?     # Optional suffix
(?<latDeg>.+?)       # Match anything and we'll try to parse it later
[D\*\u00B0]?\s*      # Degree symbol ([D|*|°] optional) followed by optional whitespace
(?<latSuf>[NS])?\s+  # Suffix could also be here. Need some whitespace to separate

(?<lonSuf>[EW])?     # Now try the longitude
(?<lonDeg>.+?)       # Degrees
[D\*\u00B0]?\s*      # Degree symbol + whitespace
(?<lonSuf>[EW])?     # Optional suffix
\s*$                 # Match the end of the string (ignoring whitespace)";

            private const string DegreeMinutePattern = @"
^\s*                 # Ignore any whitespace at the start of the string
(?<latSuf>[NS])?     # Optional suffix
(?<latDeg>.+?)       # Match anything
[D\*\u00B0\s]        # Degree symbol or whitespace
(?<latMin>.+?)       # Now look for minutes
[M'\u2032\u2019]?\s* # Minute symbol [single quote, prime, smart quote, M] + whitespace
(?<latSuf>[NS])?\s+  # Optional suffix + whitespace

(?<lonSuf>[EW])?      # Now try the longitude
(?<lonDeg>.+?)        # Degrees
[D\*\u00B0?\s]        # Degree symbol or whitespace
(?<lonMin>.+?)        # Minutes
[M'\u2032\u2019]?\s*  # Minute symbol
(?<lonSuf>[EW])?      # Optional suffix
\s*$                  # Match the end of the string (ignoring whitespace)";

            private const string DegreeMinuteSecondPattern = @"
^\s*                  # Ignore any whitespace at the start of the string
(?<latSuf>[NS])?      # Optional suffix
(?<latDeg>.+?)        # Match anything
[D\*\u00B0\s]         # Degree symbol/whitespace
(?<latMin>.+?)        # Now look for minutes
[M'\u2032\u2019\s]    # Minute symbol/whitespace
(?<latSec>.+?)        # Look for seconds
[""\u2033\u201D]?\s*  # Second symbol [double quote (c# escaped), double prime or smart doube quote] + whitespace
(?<latSuf>[NS])?\s+   # Optional suffix + whitespace

(?<lonSuf>[EW])?      # Now try the longitude
(?<lonDeg>.+?)        # Degrees
[D\*\u00B0\s]         # Degree symbol/whitespace
(?<lonMin>.+?)        # Minutes
[M'\u2032\u2019\s]    # Minute symbol/whitespace
(?<lonSec>.+?)        # Seconds
[""\u2033\u201D]?\s*  # Second symbol
(?<lonSuf>[EW])?      # Optional suffix
\s*$                  # Match the end of the string (ignoring whitespace)";

            private const string IsoPattern = @"
^\s*                                        # Match the start of the string, ignoring any whitespace
(?<latitude> [+-][0-9]{2,6}(?: \. [0-9]+)?) # The decimal digits and punctuation are strictly defined
(?<longitude>[+-][0-9]{3,7}(?: \. [0-9]+)?) # in the standard. The decimal part is optional.
(?<altitude> [+-][0-9]+(?: \. [0-9]+)?)?    # The altitude component is optional
/                                           # The string must be terminated by '/'";

            private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase;

            private static readonly Regex degreeRegex =
                new Regex(DegreePattern, Options);
            private static readonly Regex degreeMinuteRegex =
                new Regex(DegreeMinutePattern, Options);
            private static readonly Regex degreeMinuteSecondRegex =
                new Regex(DegreeMinuteSecondPattern, Options);

            private static readonly Regex isoRegex =
                new Regex(IsoPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace);

            /// <summary>
            /// Parses the input string for a value containg a pair of degree
            /// values.
            /// </summary>
            /// <param name="value">The input to parse.</param>
            /// <param name="provider">
            /// The culture-specific formatting information to use when parsing.
            /// </param>
            /// <returns>
            /// A Location representing the string on success; otherwise, null.
            /// </returns>
            internal static Location ParseDegrees(string value, IFormatProvider provider)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                return Parse(value, provider, degreeRegex);
            }

            /// <summary>
            /// Parses the input string for a value containg a pair of degree
            /// minute values.
            /// </summary>
            /// <param name="value">The input to parse.</param>
            /// <param name="provider">
            /// The culture-specific formatting information to use when parsing.
            /// </param>
            /// <returns>
            /// A Location representing the string on success; otherwise, null.
            /// </returns>
            internal static Location ParseDegreesMinutes(string value, IFormatProvider provider)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                return Parse(value, provider, degreeMinuteRegex);
            }

            /// <summary>
            /// Parses the input string for a value containg a pair of degree
            /// minute second values.
            /// </summary>
            /// <param name="value">The input to parse.</param>
            /// <param name="provider">
            /// The culture-specific formatting information to use when parsing.
            /// </param>
            /// <returns>
            /// A Location representing the string on success; otherwise, null.
            /// </returns>
            internal static Location ParseDegreesMinutesSeconds(string value, IFormatProvider provider)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }
                return Parse(value, provider, degreeMinuteSecondRegex);
            }

            /// <summary>
            /// Parses the specified input string for an ISO 6709 formatted
            /// coordinate from a string.
            /// </summary>
            /// <param name="value">The input to parse.</param>
            /// <returns>
            /// A Location representing the string on success; otherwise, null.
            /// </returns>
            internal static Location ParseIso(string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var match = isoRegex.Match(value);
                    if (match.Success)
                    {
                        Angle latitude = ParseIsoAngle(match.Groups[1].Value, 2);
                        Angle longitude = ParseIsoAngle(match.Groups[2].Value, 3);

                        double? altitude = null;
                        var group = match.Groups[3];
                        if (group.Success)
                        {
                            altitude = double.Parse(group.Value, CultureInfo.InvariantCulture);
                        }

                        return CreateLocation(latitude, longitude, altitude);
                    }
                }
                return null;
            }

            private static Location CreateLocation(Angle latitude, Angle longitude, double? altitude)
            {
                // Validate the angles to make sure they were correctly parsed
                // and that they are within range (prevents throwing exceptions
                // from the constructors).
                if ((latitude == null) ||
                    (longitude == null) ||
                    (Math.Abs(latitude.TotalDegrees) > 90.0) ||
                    (Math.Abs(longitude.TotalDegrees) > 180.0))
                {
                    return null;
                }

                if (altitude != null)
                {
                    return new Location(new Latitude(latitude), new Longitude(longitude), altitude.Value);
                }
                return new Location(new Latitude(latitude), new Longitude(longitude));
            }

            private static Location Parse(string input, IFormatProvider provider, Regex regex)
            {
                var match = regex.Match(input.Replace(", ", " "));
                if (match.Success)
                {
                    Angle latitude = ParseAngle(
                        provider,
                        TryGetValue(match, "latSuf"),
                        TryGetValue(match, "latDeg"),
                        TryGetValue(match, "latMin"),
                        TryGetValue(match, "latSec"));

                    Angle longitude = ParseAngle(
                        provider,
                        TryGetValue(match, "lonSuf"),
                        TryGetValue(match, "lonDeg"),
                        TryGetValue(match, "lonMin"),
                        TryGetValue(match, "lonSec"));

                    return CreateLocation(latitude, longitude, null);
                }
                return null;
            }

            private static Angle ParseAngle(IFormatProvider provider, string suffix, string degrees, string minutes = null, string seconds = null)
            {
                double degreeValue = 0;
                double minuteValue = 0;
                double secondValue = 0;

                // First try parsing the values (minutes and seconds are optional)
                if (!double.TryParse(degrees, NumberStyles.Float, provider, out degreeValue) ||
                    ((minutes != null) && !double.TryParse(minutes, NumberStyles.Float, provider, out minuteValue)) ||
                    ((seconds != null) && !double.TryParse(seconds, NumberStyles.Float, provider, out secondValue)))
                {
                    return null;
                }

                // We've parsed all the information! Now make everything the correct
                // sign, starting with degrees.
                if (!string.IsNullOrEmpty(suffix))
                {
                    // Make the angle a known sign and invert it if we need to
                    degreeValue = Math.Abs(degreeValue);
                    switch (suffix)
                    {
                        case "s":
                        case "S":
                        case "w":
                        case "W":
                            degreeValue = -degreeValue;
                            break;
                    }
                }

                // Now make minutes + seconds have the same sign
                minuteValue = MakeSameSign(degreeValue, minuteValue);
                secondValue = MakeSameSign(degreeValue, secondValue);

                // Return then angle
                return Angle.FromDegrees(degreeValue, minuteValue, secondValue);
            }

            private static Angle ParseIsoAngle(string value, int degreeDigits)
            {
                int decimalPoint = value.IndexOf('.');
                if (decimalPoint == -1)
                {
                    decimalPoint = value.Length;
                }

                Angle angle = null;

                // The only variable is the number of degree digits - there will
                // always be the sign, two minute digits and two seconds digits
                switch (decimalPoint - degreeDigits)
                {
                    case 1: // sign only : value represents degrees
                        angle = Angle.FromDegrees(
                            double.Parse(value.Substring(1), CultureInfo.InvariantCulture));
                        break;
                    case 3: // sign + MM : value is degrees and minutes
                        angle = Angle.FromDegrees(
                            int.Parse(value.Substring(1, degreeDigits), CultureInfo.InvariantCulture),
                            double.Parse(value.Substring(degreeDigits + 1), CultureInfo.InvariantCulture));
                        break;
                    case 5: // sign + MM + SS : value is degrees, minutes and seconds
                        angle = Angle.FromDegrees(
                            int.Parse(value.Substring(1, degreeDigits), CultureInfo.InvariantCulture),
                            int.Parse(value.Substring(degreeDigits + 1, 2), CultureInfo.InvariantCulture),
                            double.Parse(value.Substring(degreeDigits + 3), CultureInfo.InvariantCulture));
                        break;
                    default:
                        return null; // Invalid format
                }

                if (value[0] == '-') // Check the sign
                {
                    return Angle.Negate(angle);
                }
                return angle;
            }

            private static double MakeSameSign(double source, double value)
            {
                value = Math.Abs(value);
                if (Math.Sign(source) == -1)
                {
                    return -value;
                }
                return value;
            }

            private static string TryGetValue(System.Text.RegularExpressions.Match match, string groupName)
            {
                var group = match.Groups[groupName];

                // Need to check that only a single capture occured, as the suffixes are used more than once
                if (group.Success && (group.Captures.Count == 1))
                {
                    return group.Value;
                }
                return null;
            }
        }
    }
}