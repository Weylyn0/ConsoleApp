namespace Algorithm.Sorting;

public class RadixSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Radix Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is linear O(n + k) where k is the maximum element in the array
    /// </summary>
    /// <param name="values"></param>
    /// <param name="descending"></param>
    public static void Sort(int[] values, bool descending = false)
    {
        int maxValue = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            if (maxValue < values[i])
            {
                maxValue = values[i];
            }
        }

        for (int k = 1; maxValue != 0; maxValue /= 10, k *= 10)
        {
            for (int i = 0; i < values.Length; i++)
            {
                for (int j = (i + 1); j < values.Length; j++)
                {
                    int f = (values[i] / k) % 10;
                    int s = (values[j] / k) % 10;
                    if (descending ? f < s : f > s)
                    {
                        values[i] ^= values[j];
                        values[j] ^= values[i];
                        values[i] ^= values[j];
                    }
                }
            }
        }
    }
}
