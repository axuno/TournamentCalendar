// Copyright (c) 2012, Yves Goergen, http://unclassified.software/source/colormath
//
// Copying and distribution of this file, with or without modification, are permitted provided the
// copyright notice and this notice are preserved. This file is offered as-is, without any warranty.


// See also HSL Calculator on
// https://www.w3schools.com/colors/colors_hsl.asp

// VERY simple Javascript color picker:
// http://jscolor.com/
// and https://www.w3schools.com/colors/colors_picker.asp

using System.ComponentModel;
using System.Drawing;

namespace Axuno.ColorTools
{
    public static class ColorMath
    {
        public static Color? FromHtmlColor(string htmlColor)
        {
            if (htmlColor.StartsWith("#") && (htmlColor.Length == 7 || htmlColor.Length == 4) &&
                htmlColor.Substring(1).All(c => "ABCDEF0123456789".IndexOf(char.ToUpper(c)) != -1))
            {
                return (Color?) new ColorConverter().ConvertFromString(htmlColor);
            }
            var converter = new ColorConverter();
            var stdColors = (TypeConverter.StandardValuesCollection?)converter.GetStandardValues();
            if (stdColors != null && stdColors.Cast<Color>().Any(col => col.Name.Equals(htmlColor, StringComparison.OrdinalIgnoreCase)))
            {
                return (Color?) converter.ConvertFromString(htmlColor);
            }
            throw new ArgumentException($"Invalid argument '{htmlColor}'");
        }


        /// <summary>
        /// Blends (mixes) two colors in the specified ratio.
        /// </summary>
        /// <param name="color1">First color.</param>
        /// <param name="color2">Second color.</param>
        /// <param name="ratio">Ratio between both colors. 0 for first color, 1 for second color.</param>
        /// <returns></returns>
        public static Color Blend(Color color1, Color color2, double ratio)
        {
            var a = (int) Math.Round(color1.A * (1 - ratio) + color2.A * ratio);
            var r = (int) Math.Round(color1.R * (1 - ratio) + color2.R * ratio);
            var g = (int) Math.Round(color1.G * (1 - ratio) + color2.G * ratio);
            var b = (int) Math.Round(color1.B * (1 - ratio) + color2.B * ratio);
            return Color.FromArgb(a, r, g, b);
        }

        public static Color Darken(Color color, double ratio)
        {
            return Blend(color, Color.Black, ratio);
        }

        public static Color Lighten(Color color, double ratio)
        {
            return Blend(color, Color.White, ratio);
        }

        public static HslColor RgbToHsl(Color rgb)
        {
            // Translated from JavaScript, part of coati
            double h, s, l;
            var r = (double) rgb.R / 255;
            var g = (double) rgb.G / 255;
            var b = (double) rgb.B / 255;
            var min = Math.Min(Math.Min(r, g), b);
            var max = Math.Max(Math.Max(r, g), b);

            l = (max + min) / 2;

            if (max == min)
                h = 0;
            else if (max == r)
                h = (60 * (g - b) / (max - min)) % 360;
            else if (max == g)
                h = (60 * (b - r) / (max - min) + 120) % 360;
            else //if (max == b)
                h = (60 * (r - g) / (max - min) + 240) % 360;
            if (h < 0)
                h += 360;

            if (max == min)
                s = 0;
            else if (l <= 0.5)
                s = (max - min) / (2 * l);
            else
                s = (max - min) / (2 - 2 * l);

            return new HslColor((byte) Math.Round((h / 360 * 256) % 256), (byte) Math.Round(s * 255),
                (byte) Math.Round(l * 255));
        }

        public static Color HslToRgb(HslColor hsl)
        {
            // Translated from JavaScript, part of coati
            var h = (double) hsl.H / 256;
            var s = (double) hsl.S / 255;
            var l = (double) hsl.L / 255;
            double q;
            if (l < 0.5)
                q = l * (1 + s);
            else
                q = l + s - l * s;
            var p = 2 * l - q;
            var t = new double[] {h + 1.0 / 3, h, h - 1.0 / 3};
            var rgb = new byte[3];
            for (var i = 0; i < 3; i++)
            {
                if (t[i] < 0) t[i]++;
                if (t[i] > 1) t[i]--;
                if (t[i] < 1.0 / 6)
                    rgb[i] = (byte) Math.Round((p + ((q - p) * 6 * t[i])) * 255);
                else if (t[i] < 1.0 / 2)
                    rgb[i] = (byte) Math.Round(q * 255);
                else if (t[i] < 2.0 / 3)
                    rgb[i] = (byte) Math.Round((p + ((q - p) * 6 * (2.0 / 3 - t[i]))) * 255);
                else
                    rgb[i] = (byte) Math.Round(p * 255);
            }
            return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }

        /// <summary>
        /// Computes the real modulus value, not the division remainder.
        /// This differs from the % operator only for negative numbers.
        /// </summary>
        /// <param name="dividend">Dividend.</param>
        /// <param name="divisor">Divisor.</param>
        /// <returns></returns>
        private static int Mod(int dividend, int divisor)
        {
            if (divisor <= 0)
                throw new ArgumentOutOfRangeException("divisor", "The divisor cannot be zero or negative.");
            var i = dividend % divisor;
            if (i < 0) i += divisor;
            return i;
        }

        /// <summary>
        /// Computes the grey value value of a color.
        /// This gives an indicator for the lightness of a color.
        /// This calculates the "perceived brightness" and follows
        /// the specification "ITU-R Recommendation BT.601" (http://en.wikipedia.org/wiki/CCIR_601)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static byte ToGray(Color c)
        {
            return (byte) (c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
        }

        /// <summary>
        /// Determines whether the color is dark or light.
        ///
        /// Colors with a perceived brightness near the middle (e.g. 120-140) will be more subjective. For example, it's debatable whether red (FF0000), which evaluates to 139, is clearer with a black or white overlay.
        /// So the threshold is arbitrary in a range between about 120 to 140.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDarkColor(Color c)
        {
            return ToGray(c) < 0x90;
        }

        // Practical good idea:
        // Source: https://stackoverflow.com/questions/2241447/make-foregroundcolor-black-or-white-depending-on-background
        public static Color GetReadableForeColor(Color c)
        {
            return (((c.R + c.B + c.G) / 3) > 128) ? Color.Black : Color.White;
        }
    }


    /// <summary>
    /// See also http://sass-lang.com/documentation/Sass/Script/Functions.html 
    /// sass color functions are also working with HslColor
    /// </summary>
    public struct HslColor
    {
        private byte h, s, l;

        public byte H
        {
            get { return h; }
            set { h = value; }
        }

        public byte S
        {
            get { return s; }
            set { s = value; }
        }

        public byte L
        {
            get { return l; }
            set { l = value; }
        }

        public HslColor(byte h, byte s, byte l)
        {
            this.h = h;
            this.s = s;
            this.l = l;
        }
    }
}

// https://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
    public class HSLColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double hue = 1.0;
        private double saturation = 1.0;
        private double luminosity = 1.0;

        private const double scale = 240.0;

        public double Hue
        {
            get { return hue * scale; }
            set { hue = CheckRange(value / scale); }
        }
        public double Saturation
        {
            get { return saturation * scale; }
            set { saturation = CheckRange(value / scale); }
        }
        public double Luminosity
        {
            get { return luminosity * scale; }
            set { luminosity = CheckRange(value / scale); }
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        public string ToRGBString()
        {
            var color = (Color)this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
        }

        #region Casts to/from System.Drawing.Color
        public static implicit operator Color(HSLColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor.luminosity != 0)
            {
                if (hslColor.saturation == 0)
                    r = g = b = hslColor.luminosity;
                else
                {
                    var temp2 = GetTemp2(hslColor);
                    var temp1 = 2.0 * hslColor.luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor.hue);
                    b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }
        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }
        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor.luminosity < 0.5)  //<=??
                temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
            else
                temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
            return temp2;
        }

        public static implicit operator HSLColor(Color color)
        {
            var hslColor = new HSLColor();
            hslColor.hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
            hslColor.luminosity = color.GetBrightness();
            hslColor.saturation = color.GetSaturation();
            return hslColor;
        }
        #endregion

        public void SetRGB(int red, int green, int blue)
        {
            var hslColor = (HSLColor)Color.FromArgb(red, green, blue);
            this.hue = hslColor.hue;
            this.saturation = hslColor.saturation;
            this.luminosity = hslColor.luminosity;
        }

        public HSLColor() { }
        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }
        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }
        public HSLColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

    }
