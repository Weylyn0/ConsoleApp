using System;
using System.Collections.Generic;
using System.Text;

namespace Colors;

public class Color
{
    public byte Red { get; set; }
    public byte Green { get; set; }
    public byte Blue { get; set; }

    public Color(byte r, byte g, byte b)
    {
        Red = r;
        Green = g;
        Blue = b;
    }

    public Color(int rawValue)
    {
        Red = (byte)((rawValue >> 16) & 0b11111111);
        Green = (byte)((rawValue >> 8) & 0b11111111);
        Blue = (byte)(rawValue & 0b11111111);
    }

    public int RawValue
    {
        get
        {
            return ((Red << 16) + (Green << 8) + Blue);
        }
    }

    public string HexCode
    {
        get
        {
            return $"#{Red:X2}{Green:X2}{Blue:X2}";
        }
    }

    public static Color IndianRed()
        => new(13458524);
}
