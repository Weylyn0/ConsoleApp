namespace Algorithm.Sorting;

public class QuickSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Quick Sort algorithm in ascending order. Time complexity of this algorithm is log-linear O(nlog(n))
    /// </summary>
    /// <param name="values"></param>
    public static void Sort(int[] values)
    {
        Sort(values, 0, values.Length - 1);
    }

    private static void Sort(int[] values, int left, int right)
    {
        if (left >= right)
            return;

        int v = Partition(values, left, right);
        Sort(values, left, v - 1);
        Sort(values, v + 1, right);
    }

    private static int Partition(int[] values, int left, int right)
    {
        int pivot = values[right];
        int index = left - 1;
        for (int i = left; i < right; i++)
        {
            if (values[i] <= pivot)
            {
                index++;
                values[index] ^= values[i];
                values[i] ^= values[index];
                values[index] ^= values[i];
            }
        }
        index++;
        values[index] ^= values[right];
        values[right] ^= values[index];
        values[index] ^= values[right];
        return index;
    }
}
