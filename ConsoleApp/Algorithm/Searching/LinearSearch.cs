namespace Algorithm.Searching;

public class LinearSearch
{
    /// <summary>
    /// Searchs <paramref name="value"/> in <paramref name="values"/> using Linear Search algorithm. Time complexity of this algorithm is linear O(n)
    /// </summary>
    /// <param name="values"></param>
    /// <param name="value"></param>
    /// <returns><see cref="int"/></returns>
    public static int Search(int[] values, int value)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] == value)
            {
                return i;
            }
        }

        return -1;
    }
}
