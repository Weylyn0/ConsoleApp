namespace Algorithm.Sorting;

public static class BubbleSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Bubble Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is quadratic O(n^2)
    /// </summary>
    /// <param name="values"></param>
    /// <param name="descending"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        for (int i = 0; i < values.Length; i++)
        {
            for (int j = (i + 1); j < values.Length; j++)
            {
                if (descending != values[i] > values[j])
                {
                    values[i] ^= values[j];
                    values[j] ^= values[i];
                    values[i] ^= values[j];
                }
            }
        }
    }
}
