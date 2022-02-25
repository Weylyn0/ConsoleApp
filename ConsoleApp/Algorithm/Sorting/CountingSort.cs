namespace Algorithm.Sorting;

public class CountingSort
{
    /// <summary>
    /// Sorts <paramref name="values"/> using Counting Sort algorithm in ascending or descending order depends on <paramref name="descending"/>. Time complexity of this algorithm is linear O(n + k) where k is the maximum element in the array
    /// </summary>
    /// <param name="values"></param>
    /// <param name="descending"></param>
    public static void Sort(uint[] values, bool descending = false)
    {
        uint maxValue = values[0];
        for (int i = 1; i < values.Length; i++)
        {
            if (maxValue < values[i])
            {
                maxValue = values[i];
            }
        }

        var count = new int[maxValue + 1];
        for (int i = 0; i < values.Length; i++)
        {
            count[values[i]]++;
        }

        uint k = descending ? maxValue - 1 : 0;
        for (uint i = 0; i <= maxValue; i++)
        {
            while (count[i] != 0)
            {
                values[k] = i;
                count[i]--;

                if (descending)
                {
                    k--;
                }

                else
                {
                    k++;
                }
            }
        }
    }
}
