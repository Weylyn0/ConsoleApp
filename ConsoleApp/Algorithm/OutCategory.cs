using System.Collections.Generic;

namespace Algorithm;

public class OutCategory
{
    /// <summary>
    /// Returns given <paramref name="value"/> as it's pronunciation
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    public static string NumberToEnglish(int value)
    {
        if (value > 1000)
            return $"{NumberToEnglish(value / 1000)} {NumberToEnglish(1000)} {NumberToEnglish(value % 1000)}";

        if (value > 100 && value < 1000)
            return $"{NumberToEnglish(value / 100)} {NumberToEnglish(100)} {NumberToEnglish(value % 100)}";

        if (value > 20 && value % 10 != 0)
            return $"{NumberToEnglish(value / 10 * 10)} {NumberToEnglish(value % 10)}";

        return value switch
        {
            0 => "zero",
            1 => "one",
            2 => "two",
            3 => "three",
            4 => "four",
            5 => "five",
            6 => "six",
            7 => "seven",
            8 => "eight",
            9 => "nine",
            10 => "ten",
            11 => "eleven",
            12 => "twelve",
            13 => "thirteen",
            14 => "fourteen",
            15 => "fifteen",
            16 => "sixteen",
            17 => "seventeen",
            18 => "eighteen",
            19 => "nineteen",
            20 => "twenty",
            30 => "thirty",
            40 => "forty",
            50 => "fifty",
            60 => "sixty",
            70 => "seventy",
            80 => "eighty",
            90 => "ninety",
            100 => "hundred",
            1000 => "thousand",
            _ => ""
        };
    }

    public static bool CanGiveBlood(string from, string to)
    {
        static bool CheckRh(char from, char to)
        {
            if (from == to)
                return true;

            if (from == '-' && to == '+')
                return true;

            return false;
        }

        if (from[..^1] == to[..^1])
        {
            return CheckRh(from[^1], to[^1]);
        }

        else
        {
            if (from[..^1] == "AB")
                return false;

            if (from[..^1] == "A" || from[..^1] == "B")
            {
                if ("AB".Replace(to[..^1], "") == from[..^1])
                    return false;

                return CheckRh(from[^1], to[^1]);
            }

            return CheckRh(from[^1], to[^1]);
        }
    }

    public static int LargestIsland(int[][] map)
    {
        if (map.Length < 1)
            return -1;

        for (int i = 1; i < map.Length; i++)
            if (map[i].Length != map[i - 1].Length)
                return -1;

        int currentSize = 0;
        var checkedSquares = new bool[map.Length][];
        for (int i = 0; i < checkedSquares.Length; i++)
            checkedSquares[i] = new bool[map[i].Length];

        static int Check(int[][] map, bool[][] checkedSquares, int i, int j)
        {
            if (0 > i || i >= map.Length)
                return 0;

            if (0 > j || j >= map[i].Length)
                return 0;

            if (map[i][j] == 0 || checkedSquares[i][j])
                return 0;

            int size = 1;
            checkedSquares[i][j] = true;
            for (int v = -1; v < 2; v++)
                for (int h = -1; h < 2; h++)
                    if (v != 0 || h != 0)
                        size += Check(map, checkedSquares, i + v, j + h);

            return size;
        }

        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                int newSize = Check(map, checkedSquares, i, j);

                if (newSize > currentSize)
                    currentSize = newSize;
            }
        }

        return currentSize;
    }

    /// <summary>
    /// Returns the farey sequence until count reachs the <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="List{T}"/></returns>
    public static List<double> FareySequence(int value)
    {
        var list = new List<double>() { 0 };
        if (value < 1)
            return list;

        for (double i = 1; i < value; i++)
        {
            for (double j = value; j > i; j--)
            {
                var farey = i / j;
                if (!list.Contains(farey))
                    list.Add(farey);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list.Count; j++)
            {
                if (list[i] < list[j])
                {
                    var temp = list[i];
                    list[i] = list[j];
                    list[j] = temp;
                }
            }
        }

        list.Add(1);
        return list;
    }

    /// <summary>
    /// Returns the ulam sequence until count reachs the <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="List{T}"/></returns>
    public static List<int> UlamSequence(int value)
    {
        var list = new List<int>();
        if (value < 1)
            return list;

        list.Add(1);
        if (value == 1)
            return list;

        list.Add(2);
        if (value == 2)
            return list;

        int next = list[^1] + 1;
        int count;
        while (list.Count < value)
        {
            count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = (i + 1); j < list.Count; j++)
                {
                    if (list[i] + list[j] == next)
                    {
                        count++;
                    }
                }
            }
            if (count == 1)
            {
                list.Add(next);
            }
            next++;
        }

        return list;
    }

    /// <summary>
    /// Finds the smallest number that is evenly divisible by the integers 1 through <paramref name="value"/> inclusive.
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int SmallestNumber(int value)
    {
        static int GreatCommonDivisor(int x, int y)
        {
            if (y == 0)
                return x;

            return GreatCommonDivisor(y, x % y);
        }

        if (value < 1)
            return 0;

        int num = 1;
        for (int i = 1; i <= value; i++)
        {
            int gcd = GreatCommonDivisor(num, i);
            num *= (gcd > 1) ? i / gcd : i;
        }

        return num;
    }

    public static int[][] RotateMatrix(int[][] matrix, int rotation)
    {
        var shadow = (int[][])matrix.Clone();
        rotation %= 4;
        rotation = (rotation > 0) ? rotation : rotation + 4;

        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix.Length; j++)
            {

            }
        }

        //TODO Rotation

        return shadow;
    }
}
