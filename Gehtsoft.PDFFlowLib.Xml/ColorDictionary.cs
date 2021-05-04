using System;
using System.Collections.Generic;
using System.Text;
using Gehtsoft.PDFFlow.Models.Shared;

namespace Gehtsoft.PDFFlowLib.Xml
{
    /// <summary>
    /// <para>Dictionary of HTML color names.</para>
    /// <para>See w3scool [eurl=https://www.w3schools.com/colors/colors_names.asp]For color names[/eurl]</para>
    /// <para>Note: the dictionary is case insensitive</para>
    /// </summary>
    internal sealed class ColorDictionary
    {
        private static ColorDictionary gInst;

        /// <summary>
        /// Gets an instance of the color dictionary.
        /// </summary>
        public static ColorDictionary Instance => gInst ??= new ColorDictionary();

        /// <summary>
        /// Checks whether the dictionary has the color definition
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsDefined(string name) => mColors.ContainsKey(name);

        /// <summary>
        /// Gets the color by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Color this[string name] => mColors[name];

        private readonly Dictionary<string, Color> mColors;

        private class NoCaseComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

            public int GetHashCode(string obj) => obj.ToLower().GetHashCode();
        }

        private ColorDictionary()
        {
            mColors = new Dictionary<string, Color>(new NoCaseComparer())
            {
                { "AliceBlue", new Color(0xF0, 0xF8, 0xFF) },
                { "AntiqueWhite", new Color(0xFA, 0xEB, 0xD7) },
                { "Aqua", new Color(0x00, 0xFF, 0xFF) },
                { "Aquamarine", new Color(0x7F, 0xFF, 0xD4) },
                { "Azure", new Color(0xF0, 0xFF, 0xFF) },
                { "Beige", new Color(0xF5, 0xF5, 0xDC) },
                { "Bisque", new Color(0xFF, 0xE4, 0xC4) },
                { "Black", new Color(0x00, 0x00, 0x00) },
                { "BlanchedAlmond", new Color(0xFF, 0xEB, 0xCD) },
                { "Blue", new Color(0x00, 0x00, 0xFF) },
                { "BlueViolet", new Color(0x8A, 0x2B, 0xE2) },
                { "Brown", new Color(0xA5, 0x2A, 0x2A) },
                { "BurlyWood", new Color(0xDE, 0xB8, 0x87) },
                { "CadetBlue", new Color(0x5F, 0x9E, 0xA0) },
                { "Chartreuse", new Color(0x7F, 0xFF, 0x00) },
                { "Chocolate", new Color(0xD2, 0x69, 0x1E) },
                { "Coral", new Color(0xFF, 0x7F, 0x50) },
                { "CornflowerBlue", new Color(0x64, 0x95, 0xED) },
                { "Cornsilk", new Color(0xFF, 0xF8, 0xDC) },
                { "Crimson", new Color(0xDC, 0x14, 0x3C) },
                { "Cyan", new Color(0x00, 0xFF, 0xFF) },
                { "DarkBlue", new Color(0x00, 0x00, 0x8B) },
                { "DarkCyan", new Color(0x00, 0x8B, 0x8B) },
                { "DarkGoldenRod", new Color(0xB8, 0x86, 0x0B) },
                { "DarkGray", new Color(0xA9, 0xA9, 0xA9) },
                { "DarkGrey", new Color(0xA9, 0xA9, 0xA9) },
                { "DarkGreen", new Color(0x00, 0x64, 0x00) },
                { "DarkKhaki", new Color(0xBD, 0xB7, 0x6B) },
                { "DarkMagenta", new Color(0x8B, 0x00, 0x8B) },
                { "DarkOliveGreen", new Color(0x55, 0x6B, 0x2F) },
                { "DarkOrange", new Color(0xFF, 0x8C, 0x00) },
                { "DarkOrchid", new Color(0x99, 0x32, 0xCC) },
                { "DarkRed", new Color(0x8B, 0x00, 0x00) },
                { "DarkSalmon", new Color(0xE9, 0x96, 0x7A) },
                { "DarkSeaGreen", new Color(0x8F, 0xBC, 0x8F) },
                { "DarkSlateBlue", new Color(0x48, 0x3D, 0x8B) },
                { "DarkSlateGray", new Color(0x2F, 0x4F, 0x4F) },
                { "DarkSlateGrey", new Color(0x2F, 0x4F, 0x4F) },
                { "DarkTurquoise", new Color(0x00, 0xCE, 0xD1) },
                { "DarkViolet", new Color(0x94, 0x00, 0xD3) },
                { "DeepPink", new Color(0xFF, 0x14, 0x93) },
                { "DeepSkyBlue", new Color(0x00, 0xBF, 0xFF) },
                { "DimGray", new Color(0x69, 0x69, 0x69) },
                { "DimGrey", new Color(0x69, 0x69, 0x69) },
                { "FireBrick", new Color(0xB2, 0x22, 0x22) },
                { "FloralWhite", new Color(0xFF, 0xFA, 0xF0) },
                { "ForestGreen", new Color(0x22, 0x8B, 0x22) },
                { "Fuchsia", new Color(0xFF, 0x00, 0xFF) },
                { "Gainsboro", new Color(0xDC, 0xDC, 0xDC) },
                { "GhostWhite", new Color(0xF8, 0xF8, 0xFF) },
                { "Gold", new Color(0xFF, 0xD7, 0x00) },
                { "GoldenRod", new Color(0xDA, 0xA5, 0x20) },
                { "Gray", new Color(0x80, 0x80, 0x80) },
                { "Grey", new Color(0x80, 0x80, 0x80) },
                { "Green", new Color(0x00, 0x80, 0x00) },
                { "GreenYellow", new Color(0xAD, 0xFF, 0x2F) },
                { "HoneyDew", new Color(0xF0, 0xFF, 0xF0) },
                { "HotPink", new Color(0xFF, 0x69, 0xB4) },
                { "IndianRed", new Color(0xCD, 0x5C, 0x5C) },
                { "Indigo", new Color(0x4B, 0x00, 0x82) },
                { "Ivory", new Color(0xFF, 0xFF, 0xF0) },
                { "Khaki", new Color(0xF0, 0xE6, 0x8C) },
                { "Lavender", new Color(0xE6, 0xE6, 0xFA) },
                { "LavenderBlush", new Color(0xFF, 0xF0, 0xF5) },
                { "LawnGreen", new Color(0x7C, 0xFC, 0x00) },
                { "LemonChiffon", new Color(0xFF, 0xFA, 0xCD) },
                { "LightBlue", new Color(0xAD, 0xD8, 0xE6) },
                { "LightCoral", new Color(0xF0, 0x80, 0x80) },
                { "LightCyan", new Color(0xE0, 0xFF, 0xFF) },
                { "LightGoldenRodYellow", new Color(0xFA, 0xFA, 0xD2) },
                { "LightGray", new Color(0xD3, 0xD3, 0xD3) },
                { "LightGrey", new Color(0xD3, 0xD3, 0xD3) },
                { "LightGreen", new Color(0x90, 0xEE, 0x90) },
                { "LightPink", new Color(0xFF, 0xB6, 0xC1) },
                { "LightSalmon", new Color(0xFF, 0xA0, 0x7A) },
                { "LightSeaGreen", new Color(0x20, 0xB2, 0xAA) },
                { "LightSkyBlue", new Color(0x87, 0xCE, 0xFA) },
                { "LightSlateGray", new Color(0x77, 0x88, 0x99) },
                { "LightSlateGrey", new Color(0x77, 0x88, 0x99) },
                { "LightSteelBlue", new Color(0xB0, 0xC4, 0xDE) },
                { "LightYellow", new Color(0xFF, 0xFF, 0xE0) },
                { "Lime", new Color(0x00, 0xFF, 0x00) },
                { "LimeGreen", new Color(0x32, 0xCD, 0x32) },
                { "Linen", new Color(0xFA, 0xF0, 0xE6) },
                { "Magenta", new Color(0xFF, 0x00, 0xFF) },
                { "Maroon", new Color(0x80, 0x00, 0x00) },
                { "MediumAquaMarine", new Color(0x66, 0xCD, 0xAA) },
                { "MediumBlue", new Color(0x00, 0x00, 0xCD) },
                { "MediumOrchid", new Color(0xBA, 0x55, 0xD3) },
                { "MediumPurple", new Color(0x93, 0x70, 0xDB) },
                { "MediumSeaGreen", new Color(0x3C, 0xB3, 0x71) },
                { "MediumSlateBlue", new Color(0x7B, 0x68, 0xEE) },
                { "MediumSpringGreen", new Color(0x00, 0xFA, 0x9A) },
                { "MediumTurquoise", new Color(0x48, 0xD1, 0xCC) },
                { "MediumVioletRed", new Color(0xC7, 0x15, 0x85) },
                { "MidnightBlue", new Color(0x19, 0x19, 0x70) },
                { "MintCream", new Color(0xF5, 0xFF, 0xFA) },
                { "MistyRose", new Color(0xFF, 0xE4, 0xE1) },
                { "Moccasin", new Color(0xFF, 0xE4, 0xB5) },
                { "NavajoWhite", new Color(0xFF, 0xDE, 0xAD) },
                { "Navy", new Color(0x00, 0x00, 0x80) },
                { "OldLace", new Color(0xFD, 0xF5, 0xE6) },
                { "Olive", new Color(0x80, 0x80, 0x00) },
                { "OliveDrab", new Color(0x6B, 0x8E, 0x23) },
                { "Orange", new Color(0xFF, 0xA5, 0x00) },
                { "OrangeRed", new Color(0xFF, 0x45, 0x00) },
                { "Orchid", new Color(0xDA, 0x70, 0xD6) },
                { "PaleGoldenRod", new Color(0xEE, 0xE8, 0xAA) },
                { "PaleGreen", new Color(0x98, 0xFB, 0x98) },
                { "PaleTurquoise", new Color(0xAF, 0xEE, 0xEE) },
                { "PaleVioletRed", new Color(0xDB, 0x70, 0x93) },
                { "PapayaWhip", new Color(0xFF, 0xEF, 0xD5) },
                { "PeachPuff", new Color(0xFF, 0xDA, 0xB9) },
                { "Peru", new Color(0xCD, 0x85, 0x3F) },
                { "Pink", new Color(0xFF, 0xC0, 0xCB) },
                { "Plum", new Color(0xDD, 0xA0, 0xDD) },
                { "PowderBlue", new Color(0xB0, 0xE0, 0xE6) },
                { "Purple", new Color(0x80, 0x00, 0x80) },
                { "RebeccaPurple", new Color(0x66, 0x33, 0x99) },
                { "Red", new Color(0xFF, 0x00, 0x00) },
                { "RosyBrown", new Color(0xBC, 0x8F, 0x8F) },
                { "RoyalBlue", new Color(0x41, 0x69, 0xE1) },
                { "SaddleBrown", new Color(0x8B, 0x45, 0x13) },
                { "Salmon", new Color(0xFA, 0x80, 0x72) },
                { "SandyBrown", new Color(0xF4, 0xA4, 0x60) },
                { "SeaGreen", new Color(0x2E, 0x8B, 0x57) },
                { "SeaShell", new Color(0xFF, 0xF5, 0xEE) },
                { "Sienna", new Color(0xA0, 0x52, 0x2D) },
                { "Silver", new Color(0xC0, 0xC0, 0xC0) },
                { "SkyBlue", new Color(0x87, 0xCE, 0xEB) },
                { "SlateBlue", new Color(0x6A, 0x5A, 0xCD) },
                { "SlateGray", new Color(0x70, 0x80, 0x90) },
                { "SlateGrey", new Color(0x70, 0x80, 0x90) },
                { "Snow", new Color(0xFF, 0xFA, 0xFA) },
                { "SpringGreen", new Color(0x00, 0xFF, 0x7F) },
                { "SteelBlue", new Color(0x46, 0x82, 0xB4) },
                { "Tan", new Color(0xD2, 0xB4, 0x8C) },
                { "Teal", new Color(0x00, 0x80, 0x80) },
                { "Thistle", new Color(0xD8, 0xBF, 0xD8) },
                { "Tomato", new Color(0xFF, 0x63, 0x47) },
                { "Turquoise", new Color(0x40, 0xE0, 0xD0) },
                { "Violet", new Color(0xEE, 0x82, 0xEE) },
                { "Wheat", new Color(0xF5, 0xDE, 0xB3) },
                { "White", new Color(0xFF, 0xFF, 0xFF) },
                { "WhiteSmoke", new Color(0xF5, 0xF5, 0xF5) },
                { "Yellow", new Color(0xFF, 0xFF, 0x00) },
                { "YellowGreen", new Color(0x9A, 0xCD, 0x32) }
            };
        }
    }
}
