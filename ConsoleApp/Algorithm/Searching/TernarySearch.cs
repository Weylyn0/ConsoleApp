namespace Algorithm.Searching;

public static class TernarySearch
{
    /// <summary>
    /// Searchs <paramref name="value"/> in <paramref name="values"/> using Ternary Search algorithm. Time complexity of this algorithm is logarithmic O(log(n)) in base 3
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int Search(int[] values, int value)
    {
        return Search(values, value, 0, values.Length - 1);
    }

    private static int Search(int[] values, int value, int left, int right)
    {
        if (right < left)
            return -1;

        int middle1 = left + (right - left) / 3;
        int middle2 = right - (right - left) / 3;

        if (values[middle1] == value)
            return middle1;

        if (values[middle2] == value)
            return middle2;

        if (value < values[middle1])
            return Search(values, value, left, middle1 - 1);

        if (value < values[middle2])
            return Search(values, value, middle1 + 1, middle2 - 1);

        return Search(values, value, middle2 + 1, right);
    }
}
