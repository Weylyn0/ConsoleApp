using System;

namespace Algorithm;

/// <summary>
/// Includes functions made with bitwise operators
/// </summary>
public class Bitwise
{
    /// <summary>
    /// Sets the bit that found in <paramref name="index"/> to 1
    /// </summary>
    /// <param name="x"></param>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public static int SetOne(int x, int index)
    {
        return (x | (1 << index));
    }

    /// <summary>
    /// Sets the bit that found in <paramref name="index"/> to 0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public static int SetZero(int x, int index)
    {
        return (x & (0 << index));
    }

    /// <summary>
    /// Increases <paramref name="x"/> by 1 without using ++ operator
    /// </summary>
    /// <param name="x"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Increment(int x)
    {
        if (x == -1)
            return 0;

        if ((x & 1) == 0)
            return x | 1;

        return (Increment(x >> 1) << 1);
    }

    /// <summary>
    /// Decreases <paramref name="x"/> by 1 without using -- operator
    /// </summary>
    /// <param name="x"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Decrement(int x)
    {
        if (x == 0)
            return -1;

        if ((x & 1) == 1)
            return x ^ 1;

        return (Decrement(x >> 1) << 1) | 1;
    }

    /// <summary>
    /// Returns exclusive or of <paramref name="x"/> and <paramref name="y"/> without using ^
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int ExclusiveOr(int x, int y)
    {
        if (x == 0 || x == -1)
            return y;

        if (y == 0 || y == -1)
            return x;

        return (ExclusiveOr(x >> 1, y >> 1) << 1) | ((x & 1) == (y & 1) ? 0 : 1);
    }

    /// <summary>
    /// Adds <paramref name="x"/> and <paramref name="y"/> using bitwise operators
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Add(int x, int y)
    {
        return (y == 0) ? x : Add(x ^ y, (x & y) << 1);
    }

    /// <summary>
    /// Subtracts <paramref name="y"/> from <paramref name="x"/> using bitwise operators
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Subtract(int x, int y)
    {
        return (y == 0) ? x : Subtract(x ^ y, (~x & y) << 1);
    }

    /// <summary>
    /// Products <paramref name="x"/> and <paramref name="y"/> using bitwise operators
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Product(int x, int y)
    {
        return (y == 0) ? 0 : (((y & 1) == 1) ? x : 0) + Product(x << 1, y >> 1);
    }

    /// <summary>
    /// Divides <paramref name="x"/> to <paramref name="y"/> using bitwise operators
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="int"/></returns>
    /// <exception cref="StackOverflowException"/>
    public static int Divide(int x, int y)
    {
        if (x < y)
            return 0;

        return Add(1, Divide(Subtract(x, y), y));
    }

    /// <summary>
    /// Returns true if <paramref name="x"/> is a even number
    /// </summary>
    /// <param name="x"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsEven(int x)
    {
        return ((x & 1) == 0);
    }

    /// <summary>
    /// Returns true if <paramref name="x"/> is a odd number
    /// </summary>
    /// <param name="x"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsOdd(int x)
    {
        return ((x & 1) == 1);
    }

    /// <summary>
    /// Returns true if <paramref name="x"/> and <paramref name="y"/> greater or lesser than 0
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsOppositeSigns(int x, int y)
    {
        return ((x ^ y) < 0);
    }

    /// <summary>
    /// Swaps the <see cref="int"/> values using exclusive or
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void Swap(ref int x, ref int y)
    {
        x ^= y;
        y ^= x;
        x ^= y;
    }

    /// <summary>
    /// Returns logarithm of <paramref name="x"/> with base 2
    /// </summary>
    /// <param name="x"></param>
    /// <returns><see cref="int"/></returns>
    public static int Log2(int x)
    {
        if ((x >>= 1) > 0)
            return 1 + Log2(x);

        return 0;
    }
}
