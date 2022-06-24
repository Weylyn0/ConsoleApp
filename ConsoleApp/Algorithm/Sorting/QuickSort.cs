namespace Algorithm.Sorting;

public static class QuickSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Quick Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is log-linear O(nlog(n))
    /// </summary>
    /// <param name="values"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        Sort(values, 0, values.Length - 1, descending);
    }

    private static void Sort(int[] values, int left, int right, bool descending)
    {
        if (left >= right)
            return;

        int v = Partition(values, left, right, descending);
        Sort(values, left, v - 1, descending);
        Sort(values, v + 1, right, descending);
    }

    private static int Partition(int[] values, int left, int right, bool descending)
    {
        int pivot = values[right];
        int index = left - 1;
        int temp;
        for (int i = left; i < right; i++)
        {
            if (descending != values[i] <= pivot)
            {
                index++;
                temp = values[i];
                values[i] = values[index];
                values[index] = temp;
            }
        }
        index++;
        temp = values[right];
        values[right] = values[index];
        values[index] = temp;
        return index;
    }
}
