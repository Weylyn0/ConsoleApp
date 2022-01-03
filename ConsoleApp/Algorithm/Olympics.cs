using System;
using System.Collections.Generic;

namespace Algorithm;

public class Olympics
{
    public static int Marble(int minGrandchild, int maxGrandchild, int minMarble, int maxMarble, int pocketSize)
    {
        int minSadGrandchild = -1;
        int currentMarble = 0;

        for (int marble = minMarble; marble <= maxMarble; marble++)
        {
            int sadGrandchild = 0;
            for (int grandchild = minGrandchild; grandchild <= maxGrandchild; grandchild++)
            {
                int surplus = (marble % grandchild);
                if (surplus > pocketSize)
                {
                    sadGrandchild += (grandchild - surplus);
                    if (sadGrandchild > minSadGrandchild)
                    {
                        break;
                    }
                }
            }
            if (minSadGrandchild == -1 || sadGrandchild <= minSadGrandchild)
            {
                minSadGrandchild = sadGrandchild;
                currentMarble = marble;
            }
        }

        return currentMarble;
    }

    public static string GridWithForce(int[][] grid)
    {
        static int[] Find(int[][] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < array[i].Length; j++)
                {
                    if (array[i][j] == value)
                        return new int[] { i, j };
                }
            }

            return new int[] { -1, -1 };
        }

        static bool ZeroBetwenColumn(bool[][] painted, int column1, int column2, int row)
        {
            for (int k = Math.Min(column1, column2); k <= Math.Max(column1, column2); k++)
            {
                if (!painted[row][k])
                {
                    return true;
                }
            }
            return false;
        }

        static bool ZeroBetwenRow(bool[][] painted, int row1, int row2, int column)
        {
            for (int k = Math.Min(row1, row2); k <= Math.Max(row1, row2); k++)
            {
                if (!painted[k][column])
                {
                    return true;
                }
            }
            return false;
        }

        /* Checking grid is a valid item for this function */
        if (grid.Length == 0)
            return string.Empty;

        for (int i = 0; i < grid.Length; i++)
            if (grid[i].Length == 0 || grid[i].Length != grid[0].Length)
                return string.Empty;

        var check = new List<int>();
        for (int i = 0; i < check.Count; i++)
            check.Add(i + 1);

        for (int i = 0; i < grid.Length; i++)
            for (int j = 0; j < grid[i].Length; j++)
                check.Remove(grid[i][j]);

        if (check.Count > 0)
            return string.Empty;

        /* Defining Variables */
        bool[][] painted = new bool[grid.Length][];
        for (int i = 0; i < grid.Length; i++)
            painted[i] = new bool[grid[i].Length];

        int turn = 1;
        string result = string.Empty;

        /* Executing Algortihm */
        while (turn <= grid.Length * grid[0].Length)
        {
            var c = Find(grid, turn);
            painted[c[0]][c[1]] = true;
            bool failed = false;

            for (int i = 1; i <= turn; i++)
            {
                for (int j = i + 1; j <= turn; j++)
                {
                    var iCor = Find(grid, i);
                    var jCor = Find(grid, j);

                    if ((ZeroBetwenColumn(painted, iCor[1], jCor[1], iCor[0]) || ZeroBetwenRow(painted, iCor[0], jCor[0], jCor[1])) && (ZeroBetwenColumn(painted, iCor[1], jCor[1], jCor[0]) || ZeroBetwenRow(painted, iCor[0], jCor[0], iCor[1])))
                        failed = true;
                }

                if (failed)
                    break;
            }

            result += failed ? 0 : 1;
            turn++;
        }

        return result;
    }

    public static int SpecialWalk(int n, int k, int[] bindings, string specials)
    {
        if (n < 1 || n > 300000)
            return 0;

        if (k < 0 || k > 1000000000)
            return 0;

        if (bindings.Length != n - 1)
            return 0;

        if (specials.Length != n)
            return 0;

        static List<int> WalkableCircles(int[] bindings, int circle)
        {
            var list = (circle == 0) ? new List<int>() : new List<int>() { bindings[circle - 1] };
            for (int i = 0; i < bindings.Length; i++)
                if (bindings[i] == circle)
                    list.Add(i + 1);

            return list;
        }

        static List<List<int>> FindWay(int[] bindings, int circle, int passed)
        {
            var walkables = WalkableCircles(bindings, circle);
            walkables.Remove(passed);
            if (walkables.Count == 0)
                return new List<List<int>> { new List<int> { circle } };

            var list = new List<List<int>>();
            for (int i = 0; i < walkables.Count; i++)
            {
                var way = FindWay(bindings, walkables[i], circle);
                foreach (var item in way)
                {
                    item.Insert(0, circle);
                }
                list.AddRange(way);
            }

            return list;
        }

        static List<List<int>> FindWays(int[] bindings, int circle)
        {
            var list = new List<List<int>>();
            var walkables = WalkableCircles(bindings, circle);
            foreach (var walkable in walkables)
            {
                list.AddRange(FindWay(bindings, walkable, circle));
            }
            return list;
        }

        static List<int> FindBestWay(int k, int[] bindings, string specials, int circle)
        {
            var ways = FindWays(bindings, circle);
            var specialPositions = new List<int>();
            var anySpecial = false;
            for (int i = 0; i < ways.Count; i++)
            {
                specialPositions.Add(-1);
                for (int j = ways[i].Count - 1; j > -1; j--)
                {
                    if (specials[ways[i][j]] == '1')
                    {
                        specialPositions[i] = j;
                        anySpecial = true;
                        break;
                    }
                }
            }

            if (k == 0 || !anySpecial)
            {
                int index = 0;
                for (int i = 0; i < ways.Count; i++)
                {
                    if (ways[i].Count > ways[index].Count)
                    {
                        index = i;
                    }
                }
                return ways[index];
            }

            else
            {
                int index = 0;
                for (int i = 0; i < ways.Count; i++)
                {
                    if (specialPositions[i] > specialPositions[index])
                    {
                        index = i;
                    }
                }
                for (int i = specialPositions[index] + 1; i < ways[index].Count;)
                {
                    ways[index].RemoveAt(i);
                }
                return ways[index];
            }
        }

        static void Process(List<int> way, ref int k, ref int length, ref int circle)
        {
            k--;
            length += way.Count;
            circle = way[^1];
        }

        int length = 0;
        for (int start = 0; start < n; start++)
        {
            var way = FindBestWay(k, bindings, specials, start);
            int l = k;
            int newLength = 0;
            int circle = start;
            Process(way, ref l, ref newLength, ref circle);
            while (l > -1 && specials[circle] == '1')
            {
                var best = FindBestWay(l, bindings, specials, circle);
                Process(best, ref l, ref newLength, ref circle);
            }
            if (newLength > length)
                length = newLength;
        }

        return length;
    }

}
