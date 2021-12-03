using System;
using System.Collections.Generic;
using Algorithm;
using Chess;
using Games;

namespace ConsoleApp;

class Program
{
    static void PlayChess()
    {
        var board = Board.StartingChessPosition();
        bool moved = true;
        do
        {
            if (moved)
                Console.WriteLine(board.AsString + Environment.NewLine);
            string uci = Console.ReadLine().Trim();
            if (uci == "resign")
                break;
            moved = board.PushUci(uci);
        } while (board.EndFlag == EndFlags.None);
        Console.WriteLine(board.EndFlag);
        Console.WriteLine(board.Winner);
    }

    static void PlaySudoku()
    {
        var sudoku = new Sudoku(3);
        bool filled = true;

        do
        {
            if (filled)
                Console.WriteLine(sudoku.Board);

            string[] input = Console.ReadLine().Trim().Split(' ');
            if (input.Length != 3)
                continue;

            if (!int.TryParse(input[0], out int row))
                continue;

            if (!int.TryParse(input[1], out int column))
                continue;

            if (!int.TryParse(input[2], out int value))
                continue;

            filled = sudoku.Fill(row, column, value);

        } while (true);
    }

    static void StartTouchTyping(string text)
    {
        var index = 0;
        var k = 0;
        var t = 0;
        DateTime start = DateTime.Now;
        var colours = new ConsoleColor[text.Length];
        for (int i = 0; i < colours.Length; i++)
            colours[i] = ConsoleColor.White;

        Console.WriteLine(text);
        while (true)
        {
            var info = Console.ReadKey();

            if (index == 0)
                start = DateTime.Now;

            if (char.ToLower(info.KeyChar) == char.ToLower(text[index]))
            {
                colours[index] = ConsoleColor.Green;
                t++;
            }

            else
            {
                colours[index] = ConsoleColor.Red;
            }

            index++;
            k++;

            Console.Clear();
            for (int i = 0; i < text.Length; i++)
            {
                Console.ForegroundColor = colours[i];
                Console.Write(text[i].ToString());
                Console.ResetColor();
            }
            Console.WriteLine();

            if (index == text.Length)
                break;
        }

        Console.WriteLine($" -WPM: {(int)((k / 5) / (DateTime.Now - start).TotalMinutes)}");
        Console.WriteLine($" -Accuracy Percent: {((t / (double)text.Length) * 100):0.00}");
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Impending doom..."); 
    }
}

public class Questions
{
    static string GridWithForce(int[][] grid)
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

    static int Marble(int minGrandchild, int maxGrandchild, int minMarble, int maxMarble, int pocketSize)
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
}
