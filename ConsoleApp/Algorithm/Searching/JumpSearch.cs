namespace Algorithm.Searching;

public static class JumpSearch
{
    /// <summary>
    /// Searchs <paramref name="value"/> in <paramref name="values"/> using Jump Search algorithm. Time complexity of this algorithm is square root O(√(n))
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int Search(int[] values, int value)
    {
        int m = (int)System.Math.Sqrt(values.Length);
        for (int i = 0; i < values.Length; i += m)
        {
            if (value < values[i])
            {
                for (int j = System.Math.Max(0, i - m); j < i; j++)
                {
                    if (values[j] == value)
                    {
                        return j;
                    }
                }
            }
        }

        for (int i = System.Math.Max(0, values.Length - m); i < values.Length; i++)
        {
            if (values[i] == value)
            {
                return i;
            }
        }

        return -1;
    }
}
