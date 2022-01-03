using System.Collections.Generic;

namespace Algorithm;

public class Arrays
{
    /// <summary>
    /// Returns the minimum value in <paramref name="array"/>
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="int"/></returns>
    public static int SmallestValue(int[] array)
    {
        if (array.Length == 0)
            return 0;

        int x = array[0];
        for (int i = 1; i < array.Length; i++)
            if (array[i] < x)
                x = array[i];

        return x;
    }

    /// <summary>
    /// Returns the maximum value in <paramref name="array"/>
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="int"/></returns>
    public static int GreatestValue(int[] array)
    {
        if (array.Length == 0)
            return 0;

        int x = array[0];
        for (int i = 1; i < array.Length; i++)
            if (array[i] > x)
                x = array[i];

        return x;
    }

    /// <summary>
    /// Returns <paramref name="array"/> as ordered by ascending
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="System.Array"/></returns>
    public static int[] OrderbyAscending(int[] array)
    {
        var ordered = new int[array.Length];
        array.CopyTo(ordered, 0);
        int changed = -1;
        while (changed != 0)
        {
            changed = 0;
            for (int i = 0; i < ordered.Length - 1; i++)
            {
                if (ordered[i] > ordered[i + 1])
                {
                    int temp = ordered[i];
                    ordered[i] = ordered[i + 1];
                    ordered[i + 1] = temp;
                    changed++;
                }
            }
        }
        return ordered;
    }

    /// <summary>
    /// Sorts the <see cref="int"/> array with bubble sort algorithm
    /// </summary>
    /// <param name="array"></param>
    /// <returns><see cref="System.Array"/></returns>
    public static int[] BubbleSort(int[] array)
    {
        var shadow = (int[])array.Clone();
        for (int i = 0; i < shadow.Length; i++)
        {
            for (int j = i + 1; j < shadow.Length; j++)
            {
                if (shadow[i] > shadow[j])
                {
                    shadow[i] ^= shadow[j];
                    shadow[j] ^= shadow[i];
                    shadow[i] ^= shadow[j];
                }
            }
        }
        return shadow;
    }

    /// <summary>
    /// Returns the pairs from <paramref name="numbers"/> that sum of them is equal to <paramref name="value"/>
    /// </summary>
    /// <param name="numbers"></param>
    /// <param name="value"></param>
    /// <returns><see cref="List{T}"/></returns>
    static List<(int, int)> FindSumPair(int[] numbers, int value)
    {
        var pairs = new List<(int, int)>();
        for (int i = 0; i < numbers.Length; i++)
            for (int j = i + 1; j < numbers.Length; j++)
                if (numbers[i] + numbers[j] == value)
                    pairs.Add((numbers[i], numbers[j]));

        return pairs;
    }

    /// <summary>
    /// Returns the index of given <paramref name="item"/> in array using binary search algorithm
    /// </summary>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns><see cref="int"></returns>
    /// <exception cref="System.StackOverflowException"/>
    public static int BinarySearch(int[] array, int item)
    {
        static int BinarySearch(int[] array, int item, int start, int end)
        {
            if (end >= start)
            {
                int middle = (end + start) / 2;

                if (array[middle] == item)
                    return middle;

                if (array[middle] > item)
                    return BinarySearch(array, item, start, middle - 1);

                return BinarySearch(array, item, middle + 1, end);
            }

            return -1;
        }

        return BinarySearch(array, item, 0, array.Length - 1);
    }

    /// <summary>
    /// Returns the possible permutations of given array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="k"></param>
    /// <returns><see cref="List{T}"/></returns>
    /// <exception cref="System.StackOverflowException"/>
    public static List<string> Permutation<T>(T[] array, int k)
    {
        static List<string> Permutation(T[] array, int k, int m)
        {
            static void Swap(ref T a, ref T b)
            {
                if (a.Equals(b))
                    return;

                var temp = a;
                a = b;
                b = temp;
            }

            var list = new List<string>();

            if (k == m)
                list.Add($"({string.Join(", ", array)})");

            else
                for (int i = k; i <= m; i++)
                {
                    Swap(ref array[k], ref array[i]);
                    list.AddRange(Permutation(array, k + 1, m));
                    Swap(ref array[k], ref array[i]);
                }

            return list;
        }

        return Permutation(array, k, array.Length - 1);
    }

    /// <summary>
    /// Returns the given array as reversed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns><see cref="System.Array"/></returns>
    public static T[] Reverse<T>(T[] array)
    {
        var shadow = new T[array.Length];
        array.CopyTo(shadow, 0);
        for (int i = 0; i < shadow.Length / 2; i++)
        {
            var temp = shadow[i];
            shadow[i] = shadow[^(i + 1)];
            shadow[^(i + 1)] = temp;
        }
        return shadow;
    }
}
