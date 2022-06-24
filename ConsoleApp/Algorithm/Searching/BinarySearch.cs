namespace Algorithm.Searching;

public static class BinarySearch
{
    /// <summary>
    /// Searchs <paramref name="value"/> in <paramref name="values"/> using Binary Search algorithm. Time complexity of this algorithm is logarithmic O(log(n))
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

        int middle = (left + right) / 2;

        if (values[middle] == value)
            return middle;

        return (value < values[middle]) ? Search(values, value, left, middle - 1) : Search(values, value, middle + 1, right);
    }
}
