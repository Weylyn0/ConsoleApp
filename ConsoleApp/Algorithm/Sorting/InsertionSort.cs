namespace Algorithm.Sorting;

public class InsertionSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Insertion Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is quadratic O(n^2)
    /// </summary>
    /// <param name="values"></param>
    /// <param name="descending"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        for (int i = 1; i < values.Length; i++)
        {
            int value = values[i];
            int j = i - 1;

            while (j >= 0 && (descending ? values[j] < value : values[j] > value))
            {
                values[j + 1] = values[j];
                j--;
            }

            values[j + 1] = value;
        }
    }
}
