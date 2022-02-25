namespace Algorithm;

/// <summary>
/// Includes functions made with bitwise operators
/// </summary>
public class Bitwise
{
    /// <summary>
    /// Sets the bit of <paramref name="value"/> that found in <paramref name="index"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public static int Set(int value, int index)
    {
        return value | (1 << index);
    }

    /// <summary>
    /// Resets the bit of <paramref name="value"/> that found in <paramref name="index"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public static int Reset(int value, int index)
    {
        return value & (0 << index);
    }

    /// <summary>
    /// Gets the bit of <paramref name="value"/> that found in <paramref name="index"/>
    /// </summary>
    /// <param name="value"></param>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public static int Get(int value, int index)
    {
        return value & (1 << index);
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is a even number
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsEven(int value)
    {
        return (value & 1) == 0;
    }

    /// <summary>
    /// Returns true if <paramref name="value"/> is a odd number
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsOdd(int value)
    {
        return (value & 1) == 1;
    }

    /// <summary>
    /// Returns true if <paramref name="x"/> and <paramref name="y"/> has opposit signs
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsOppositeSigns(int x, int y)
    {
        return (x ^ y) < 0;
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
    /// Returns the <paramref name="n"/>-th power of 2
    /// </summary>
    /// <param name="n"></param>
    /// <returns><see cref="int"/></returns>
    public static int Power2(int n)
    {
        return 1 << n;
    }

    /// <summary>
    /// Returns binary logarithm of <paramref name="value"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int Log2(int value)
    {
        return ((value >>= 1) > 0) ? Log2(value) + 1 : 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int BinaryExponentiation(int x, int y)
    {
        if (y == 0)
            return 1;

        int e = BinaryExponentiation(x, y / 2);
        return ((y & 1) == 1) ? (e * e * x) : (e * e);
    }
}
