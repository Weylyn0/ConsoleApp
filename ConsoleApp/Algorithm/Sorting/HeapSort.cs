namespace Algorithm.Sorting;

public static class HeapSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Heap Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is log-linear O(nlog(n))
    /// </summary>
    /// <param name="values"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        for (int i = values.Length / 2 - 1; i >= 0; i--)
        {
            Heapify(values, values.Length, i, descending);
        }

        for (int i = values.Length - 1; i > 0; i--)
        {
            values[i] ^= values[0];
            values[0] ^= values[i];
            values[i] ^= values[0];

            Heapify(values, i, 0, descending);
        }
    }

    private static void Heapify(int[] values, int last, int root, bool descending)
    {
        int index = root;
        int left = 2 * root + 1;
        int right = 2 * root + 2;

        if (left < last && (descending != values[index] < values[left]))
        {
            index = left;
        }

        if (right < last && (descending != values[index] < values[right]))
        {
            index = right;
        }

        if (index != root)
        {
            values[root] ^= values[index];
            values[index] ^= values[root];
            values[root] ^= values[index];

            Heapify(values, last, index, descending);
        }
    }
}
