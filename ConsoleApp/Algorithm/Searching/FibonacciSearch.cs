namespace Algorithm.Searching;

public static class FibonacciSearch
{
    /// <summary>
    /// Searchs <paramref name="value"/> in <paramref name="values"/> using Fibonacci Search algorithm. Time complexity of this algorithm is logarithmic O(log(n))
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int Search(int[] values, int value)
    {
        int fi1 = 0;
        int fi2 = 1;
        int fi = fi1 + fi2;
        int offset = -1;

        while (fi < values.Length)
        {
            fi1 = fi2;
            fi2 = fi;
            fi = fi1 + fi2;
        }

        while (fi > 1)
        {
            int index = System.Math.Min(offset + fi1, values.Length - 1);

            if (values[index] == value)
            {
                return index;
            }

            else if (values[index] < value)
            {
                fi = fi2;
                fi2 = fi1;
                fi1 = fi - fi2;
                offset = index;
            }

            else
            {
                fi = fi1;
                fi2 -= fi1;
                fi1 = fi - fi2;
            }
        }

        if (fi1 == 0 && values[^1] == value)
            return values.Length - 1;

        return -1;
    }
}
