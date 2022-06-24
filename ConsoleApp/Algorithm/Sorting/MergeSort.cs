namespace Algorithm.Sorting;

public static class MergeSort
{
    /// <summary>
    /// Sorst <paramref name="values"/> using Merge Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is log-linear O(nlog(n))
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

        int middle = (left + right - 1) / 2;

        Sort(values, left, middle, descending);
        Sort(values, middle + 1, right, descending);

        Merge(values, left, middle, right, descending);
    }

    private static void Merge(int[] values, int left, int middle, int right, bool descending)
    {
        var leftSubArray = new int[middle - left + 1];
        var rightSubArray = new int[right - middle];

        for (int i = 0; i < leftSubArray.Length; i++)
        {
            leftSubArray[i] = values[left + i];
        }

        for (int i = 0; i < rightSubArray.Length; i++)
        {
            rightSubArray[i] = values[middle + i + 1];
        }

        int l = 0;
        int r = 0;
        int k = left;

        while (l < leftSubArray.Length && r < rightSubArray.Length)
        {
            if (descending != leftSubArray[l] < rightSubArray[r])
            {
                values[k] = leftSubArray[l];
                l++;
            }

            else
            {
                values[k] = rightSubArray[r];
                r++;
            }

            k++;
        }

        while (l < leftSubArray.Length)
        {
            values[k] = leftSubArray[l];
            l++;
            k++;
        }

        while (r < rightSubArray.Length)
        {
            values[k] = rightSubArray[r];
            r++;
            k++;
        }
    }
}
