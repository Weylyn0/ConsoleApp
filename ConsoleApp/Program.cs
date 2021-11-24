using System;
using System.Collections.Generic;
using System.IO;
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
        PlayChess();
    }
}
