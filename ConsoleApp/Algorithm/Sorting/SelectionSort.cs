namespace Algorithm.Sorting;

public class SelectionSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Selection Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is quadratic O(n^2)
    /// </summary>
    /// <param name="values"></param>
    /// <param name="descending"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        for (int i = 0; i < values.Length - 1; i++)
        {
            int index = i;

            for (int j = (i + 1); j < values.Length; j++)
            {
                if (descending ? values[j] > values[index] : values[j] < values[index])
                {
                    index = j;
                }
            }

            if (index != i)
            {
                values[i] ^= values[index];
                values[index] ^= values[i];
                values[i] ^= values[index];
            }
        }
    }
}
