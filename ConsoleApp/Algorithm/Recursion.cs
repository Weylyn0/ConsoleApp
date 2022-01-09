using System;

namespace Algorithm;

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
    /// Returns the digit sum of <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int DigitSumOfValue(int value)
    {
        if (value > 0)
            return (value % 10) + DigitSumOfValue(value / 10);

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
    /// Returns the digit sum of values from <paramref name="start"/> to <paramref name="end"/>
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns><see cref="int"/></returns>
    public static int DigitSumOfRange(int start, int end)
    {
        if (start < end)
            return DigitSumOfValue(start) + DigitSumOfRange(start + 1, end);

        return 0;
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
