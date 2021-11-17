using System;
using System.Collections.Generic;

namespace ConsoleApp.Algorithm;

public class Integer
{
    /// <summary>
    /// Swaps the <see cref="int"/> values using XOR (^)
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void Swap(ref int x, ref int y)
    {
        x ^= y;
        y ^= x;
        x ^= y;
    }
}

/// <summary>
/// Includes function with recursion
/// </summary>
public class Recursion
{
    /// <summary>
    /// Returns numbers from the <paramref name="value"/> to 0
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string FromValueToOne(int value)
    {
        if (value > 0)
            return value + " " + FromValueToOne(value - 1);

        return "0";
    }

    /// <summary>
    /// Returns numbers from the 0 to <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string FromOneToValue(int value)
    {
        if (value > 0)
            return FromOneToValue(value - 1) + " " + value;

        return "0";
    }

    /// <summary>
    /// Returns sum of numbers from 0 to the <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int SumOfFirstNNumber(int value)
    {
        if (value > 0)
            return value + SumOfFirstNNumber(value - 1);

        return 0;
    }

    /// <summary>
    /// Returns the <paramref name="value"/> as seperated by digits
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string DigitSeperator(int value)
    {
        if (value > 0)
            return DigitSeperator(value / 10) + " " + (value % 10);

        return "";
    }

    /// <summary>
    /// Returns the digit count of <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int DigitCountOfValue(int value)
    {
        if (value > 0)
            return 1 + DigitCountOfValue(value / 10);

        return 0;
    }

    /// <summary>
    /// Returns the even values in given range
    /// </summary>
    /// <param name="start">Start value of range</param>
    /// <param name="end">End value of range</param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string EvensInRange(int start, int end)
    {
        if (start < end)
            return ((start % 2 == 0) ? start + " " : "") + EvensInRange(start + 1, end);

        return "";
    }

    /// <summary>
    /// Returns the odd values in given range
    /// </summary>
    /// <param name="start">Start value of range</param>
    /// <param name="end">End value of range</param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string OddsInRange(int start, int end)
    {
        if (start < end)
            return ((start % 2 == 1) ? start + " " : "") + OddsInRange(start + 1, end);

        return "";
    }

    /// <summary>
    /// Returns true if the <paramref name="value"/> is a prime number
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static bool IsPrime(int value)
    {
        static bool IsPrime(int value, int k)
        {
            if (k < value / 2)
                return (value % k != 0) & IsPrime(value, k + 1);

            return true;
        }

        return IsPrime(value, 2);
    }

    /// <summary>
    /// Returns true if the <paramref name="value"/> is a palindrome string
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static bool IsPalindrome(string value)
    {
        if (value.Length > 1)
            return (value[0] == value[^1]) & IsPalindrome(value[1..^1]);

        return true;
    }

    /// <summary>
    /// Returns factorial of the <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Factorial(int value)
    {
        if (value > 1)
            return value * Factorial(value - 1);

        return 1;
    }

    /// <summary>
    /// Returns the integer from fibonacci array for <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Fibonacci(int value)
    {
        if (value > 2)
            return Fibonacci(value - 1) + Fibonacci(value - 2);

        else if (value == 0)
            return 0;

        return 1;
    }

    /// <summary>
    /// Returns fibonacci array from 0 to <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns<see cref="string"/>></returns>
    /// <exception cref="StackOverflowException"/>
    public static string FibonacciArray(int value)
    {
        if (value > -1)
            return FibonacciArray(value - 1) + " " + Fibonacci(value);

        return "";
    }

    /// <summary>
    /// Returns sum of the fibonacci array from 0 to <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns<see cref="int"/>></returns>
    /// <exception cref="StackOverflowException"/>
    public static int FibonacciSum(int value)
    {
        if (value > -1)
            return Fibonacci(value) + FibonacciSum(value - 1);

        return 0;
    }

    /// <summary>
    /// Returns the great common divisor of <paramref name="x"/> and <paramref name="y"/>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int GreatCommonDivisor(int x, int y)
    {
        if (y == 0)
            return x;

        return GreatCommonDivisor(y, x % y);
    }

    /// <summary>
    /// Returns the least common multiple of <paramref name="x"/> and <paramref name="y"/> using great common divisor
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int LeastCommonMultiple(int x, int y)
    {
        return (x * y) / GreatCommonDivisor(y, x % y);
    }

    /// <summary>
    /// Converts decimal <paramref name="value"/> to binary
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string DecimalToBinary(int value)
    {
        if (value > 0)
            return DecimalToBinary(value / 2) + (value % 2);

        return "";
    }

    /// <summary>
    /// Converts decimal <paramref name="value"/> to hex
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string DecimalToHex(int value)
    {
        if (value > 0)
            return DecimalToHex(value / 16) + (char)(48 + ((value % 16 > 9) ? value % 16 + 7 : value % 16));

        return "";
    }

    /// <summary>
    /// Returns string <paramref name="value"/> as reversed
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static string Reverse(string value)
    {
        if (value.Length > 0)
            return value[^1] + Reverse(value[..^1]);

        return "";
    }

    /// <summary>
    /// Calculates the <paramref name="e"/> th exponent of <paramref name="b"/>
    /// </summary>
    /// <param name="b"></param>
    /// <param name="e"></param>
    /// <returns><see cref="double"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static double Power(double b, double e)
    {
        if (e < 0)
        {
            b = 1 / b;
            e = -e;
        }

        if (e > 0 && e < 1)
        {
            static double Root(double value, int degree, double currentRoot)
            {
                double root = (value / currentRoot + currentRoot) / degree;

                if (currentRoot == root)
                    return root;

                return Root(value, degree, root);
            }

            return Root(b, 2, b / 2);
        }

        if (e > 0)
            return b * Power(b, e - 1);

        return 1;
    }

    /// <summary>
    /// Calculates the possible combinations of <paramref name="k"/> items from group of <paramref name="n"/> elements
    /// </summary>
    /// <param name="n"></param>
    /// <param name="k"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Combination(int n, int k)
    {
        if (k == 0 || k == n)
            return 1;

        if (k < 0 || k > n)
            return -1;

        return Combination(n - 1, k) + Combination(n - 1, k - 1);
    }

    /// <summary>
    /// Calculates the possible permutation of <paramref name="k"/> items from group of <paramref name="n"/> elements
    /// </summary>
    /// <param name="n"></param>
    /// <param name="k"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Permutation(int n, int k)
    {
        if (k == 0)
            return 1;

        if (k < 0 || k > n)
            return -1;

        return n * Permutation(n - 1, k - 1);
    }

    /// <summary>
    /// Calculates the sum of the values that returns from <paramref name="func"/> from <paramref name="i"/> to <paramref name="n"/>
    /// </summary>
    /// <param name="i">Start index of Sigma</param>
    /// <param name="n">End index of Sigma</param>
    /// <param name="func">Function of Sigma for <paramref name="i"/> and <paramref name="n"/> (e.g: (i, k) => i * k )</param>
    /// <returns><see cref="double"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static double Sigma(double i, double n, Func<double, double, double> func)
    {
        if (i > n)
            return 0;

        return func.Invoke(i, n) + Sigma(i + 1, n, func);
    }

    /// <summary>
    /// Calculates the product of the values that returns from <paramref name="func"/> from <paramref name="i"/> to <paramref name="n"/>
    /// </summary>
    /// <param name="i">Start index of Pi</param>
    /// <param name="n">End index of Pi</param>
    /// <param name="func">Function of Pi for <paramref name="i"/> and <paramref name="n"/> (e.g: (i, k) => i * k )</param>
    /// <returns><see cref="double"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static double Pi(double i, double n, Func<double, double, double> func)
    {
        if (i > n)
            return 1;

        return func.Invoke(i, n) * Pi(i + 1, n, func);
    }
}

public class Array
{
    /// <summary>
    /// Returns the minimum value in <paramref name="array"/>
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="int"/></returns>
    public static int SmallestValue(int[] array)
    {
        if (array.Length == 0)
            return 0;

        int x = array[0];
        for (int i = 1; i < array.Length; i++)
            if (array[i] < x)
                x = array[i];

        return x;
    }

    /// <summary>
    /// Returns the maximum value in <paramref name="array"/>
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="int"/></returns>
    public static int GreatestValue(int[] array)
    {
        if (array.Length == 0)
            return 0;

        int x = array[0];
        for (int i = 1; i < array.Length; i++)
            if (array[i] > x)
                x = array[i];

        return x;
    }

    /// <summary>
    /// Returns <paramref name="array"/> as ordered by ascending
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="Array{int}"/></returns>
    public static int[] OrderbyAscending(int[] array)
    {
        var ordered = new int[array.Length];
        array.CopyTo(ordered, 0);
        int changed = -1;
        while (changed != 0)
        {
            changed = 0;
            for (int i = 0; i < ordered.Length - 1; i++)
            {
                if (ordered[i] > ordered[i + 1])
                {
                    int temp = ordered[i];
                    ordered[i] = ordered[i + 1];
                    ordered[i + 1] = temp;
                    changed++;
                }
            }
        }
        return ordered;
    }

    /// <summary>
    /// Returns the pairs from <paramref name="numbers"/> that sum of them is equal to <paramref name="value"/>
    /// </summary>
    /// <param name="numbers"></param>
    /// <param name="value"></param>
    /// <returns><see cref="List{(int, int)}"/></returns>
    static List<(int, int)> FindSumPair(int[] numbers, int value)
    {
        var pairs = new List<(int, int)>();
        for (int i = 0; i < numbers.Length; i++)
            for (int j = i + 1; j < numbers.Length; j++)
                if (numbers[i] + numbers[j] == value)
                    pairs.Add((numbers[i], numbers[j]));

        return pairs;
    }

    /// <summary>
    /// Returns the index of given <paramref name="item"/> in array using binary search algorithm
    /// </summary>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns><see cref="int"></returns>
    public static int BinarySearch(int[] array, int item)
    {
        static int BinarySearch(int[] array, int item, int start, int end)
        {
            if (end >= start)
            {
                int middle = (end + start) / 2;

                if (array[middle] == item)
                    return middle;

                if (array[middle] > item)
                    return BinarySearch(array, item, start, middle - 1);

                return BinarySearch(array, item, middle + 1, end);
            }

            return -1;
        }

        return BinarySearch(array, item, 0, array.Length - 1);
    }

    /// <summary>
    /// Returns the possible permutations of given array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="k"></param>
    /// <returns><see cref="List{string}"/></returns>
    public static List<string> Permutation<T>(T[] array, int k)
    {
        static List<string> Permutation(T[] array, int k, int m)
        {
            static void Swap(ref T a, ref T b)
            {
                if (a.Equals(b))
                    return;

                var temp = a;
                a = b;
                b = temp;
            }

            var list = new List<string>();

            if (k == m)
                list.Add($"({string.Join(", ", array)})");

            else
                for (int i = k; i <= m; i++)
                {
                    Swap(ref array[k], ref array[i]);
                    list.AddRange(Permutation(array, k + 1, m));
                    Swap(ref array[k], ref array[i]);
                }

            return list;
        }

        return Permutation(array, k, array.Length - 1);
    }

    /// <summary>
    /// Returns the given array as reversed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns><see cref="{T}[]"/></returns>
    public static T[] Reverse<T>(T[] array)
    {
        var shadow = new T[array.Length];
        array.CopyTo(shadow, 0);
        for (int i = 0; i < shadow.Length / 2; i++)
        {
            var temp = shadow[i];
            shadow[i] = shadow[^(i + 1)];
            shadow[^(i + 1)] = temp;
        }
        return shadow;
    }
}

public class Algebra
{
    /// <summary>
    /// Returns true if the <paramref name="point"/> is in the triangle created with <paramref name="X"/>, <paramref name="Y"/> and <paramref name="Z"/>. Points are double array with length of 2
    /// </summary>
    /// <param name="X">First point of the triangle</param>
    /// <param name="Y">Second point if the triangle</param>
    /// <param name="Z">Third point if the triangle</param>
    /// <param name="point">The point where we will check if it is inside the triangle</param>
    /// <returns><see cref="bool"/></returns>
    public static bool PointWithinTriangle(double[] X, double[] Y, double[] Z, double[] point)
    {
        var triangle = new double[3][] { X, Y, Z };
        for (int i = 0; i < 3; i++)
        {
            var A = triangle[i];
            var B = triangle[(i + 1) % 3];
            var C = triangle[(i + 2) % 3];
            double M = (A[1] - B[1]) / (A[0] - B[0]);
            if ((C[1] - ((C[0] - A[0]) * M) > A[1]) != (point[1] - ((point[0] - A[0]) * M) > A[1]))
                return false;
        }
        return true;
    }

    public static bool PointWithinPyramid(double[] X, double[] Y, double[] Z, double[] T, double[] point)
    {
        var pyramid = new double[4][] { X, Y, Z, T };
        bool b;

        for (int i = 0; i < 4; i++)
        {
            for (int j = i + 1; j < 4; j++)
            {
                for (int k = j + 1; k < 4; k++)
                {
                    b = false;
                    for (int u = 0; u < 3; u++)
                    {
                        for (int v = u + 1; v < 3; v++)
                        {
                            if (PointWithinTriangle(new double[] { pyramid[i][u], pyramid[i][v] }, new double[] { pyramid[j][u], pyramid[j][v] }, new double[] { pyramid[k][u], pyramid[k][v] }, new double[] { point[u], point[v] }))
                            {
                                b = true;
                            }
                        }
                    }
                    if (!b)
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> has exactly 3 divisors
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool ExactlyThree(int value)
    {
        int divided = 0;
        for (int i = 2; i <= value / 2; i++)
            if (value % i == 0)
                divided++;

        return divided == 1;
    }

    /// <summary>
    /// Returns the given <paramref name="value"/> is a valid leap year
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsLeapYear(int value)
    {
        while (value % 100 == 0)
            value /= 100;

        return value % 4 == 0;
    }
}

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
}
