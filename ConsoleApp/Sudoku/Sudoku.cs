using System;
using System.Collections.Generic;
using System.Text;

namespace Games;

/// <summary>
/// Developed by Weylyn
/// </summary>
public class Sudoku
{
    /// <summary>
    /// 9x9 integer array for storing cell values
    /// </summary>
    private int[,] _cells { get; set; }

    /// <summary>
    /// Returns the value of cell for specified row and column
    /// </summary>
    /// <param name="r">Row of the cell</param>
    /// <param name="c">Column of the cell</param>
    /// <returns>Value of the cell</returns>
    public int this[int r, int c]
    {
        get
        {
            return _cells[r, c];
        }
    }

    /// <summary>
    /// Returns the sudoku as visualized
    /// </summary>
    public string Board
    {
        get
        {
            var board = "";
            for (int row = 0; row < 9; row++)
            {
                if (row % 3 == 0)
                    board += "─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─\n";

                for (int column = 0; column < 9; column++)
                {
                    if (column % 3 == 0)
                        board += "| ";

                    board += ((this[row, column] == 0) ? "." : this[row, column]) + " ";
                }

                board += "|\n";
            }

            board += "─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─";

            return board;
        }
    }

    /// <summary>
    /// Returns the empty cell count
    /// </summary>
    public int Empty
    {
        get
        {
            int count = 0;

            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if (this[row, column] == 0)
                        count++;
                }
            }

            return count;
        }
    }

    /// <summary>
    /// Creates an empty sudoku
    /// </summary>
    public Sudoku()
    {
        _cells = new int[9, 9];
    }

    /// <summary>
    /// Creates sudoku with difficulty level
    /// </summary>
    /// <param name="difficulty">Can be between 1-4</param>
    public Sudoku(int difficulty) : this()
    {
        if (difficulty < 1 || difficulty > 4)
            return;

        for (int row = 0; row < 9; row++)
            for (int column = 0; column < 9; column++)
                _cells[row, column] = (row * 3 + row / 3 + column) % 9 + 1;

        int row1, row2, column1, column2, value1, value2;
        row1 = row2 = column1 = column2 = 0;
        var rd = new Random();
        for (int i = 0; i < difficulty * 20; i++)
        {
            value1 = rd.Next(1, 10);
            value2 = rd.Next(1, 10);
            for (int squareRow = 0; squareRow < 9; squareRow += 3)
            {
                for (int squareColumn = 0; squareColumn < 9; squareColumn += 3)
                {
                    for (int row = 0; row < 3; row++)
                    {
                        for (int column = 0; column < 3; column++)
                        {

                            if (this[squareRow + row, squareColumn + column] == value1)
                            {
                                row1 = squareRow + row;
                                column1 = squareColumn + column;
                            }

                            else if (this[squareRow + row, squareColumn + column] == value2)
                            {
                                row2 = squareRow + row;
                                column2 = squareColumn + column;
                            }
                        }
                        _cells[row1, column1] = value2;
                        _cells[row2, column2] = value1;
                    }
                }
            }
        }

        for (int i = 0; i < difficulty * 15; i++)
        {
            int row = rd.Next(0, 9);
            int column = rd.Next(0, 9);
            _cells[row, column] = 0;
        }
    }

    /// <summary>
    /// Returns the cell can be fillable with value
    /// </summary>
    /// <param name="row">Row of the cell</param>
    /// <param name="column">Column of the cell</param>
    /// <param name="value">Value to set</param>
    public bool Fillable(int row, int column, int value)
    {
        if (value > 9 || value < 1)
            return false;

        if (row > 8 || row < 0 || column > 8 || column < 0)
            return false;

        if (this[row, column] != 0)
            return false;

        for (int i = 0; i < 9; i++)
            if (this[row, i] == value)
                return false;

        for (int i = 0; i < 9; i++)
            if (this[i, column] == value)
                return false;

        var square = (row / 3) + ((column / 3) * 3);
        var squareRowStart = (square % 3) * 3;
        var squareColumnStart = (square / 3) * 3;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (this[squareRowStart + i, squareColumnStart + j] == value)
                    return false;

        return true;
    }

    /// <summary>
    /// Returns the values that cell can be fillable with them
    /// </summary>
    /// <param name="row">Row of the cell</param>
    /// <param name="column">Column of the cell</param>
    /// <param name="value">Value to set</param>
    public List<int> Fillables(int row, int column)
    {
        var fillabes = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        for (int i = 1; i < 10; i++)
        {
            if (!Fillable(row, column, i))
                fillabes.Remove(i);
        }

        return fillabes;
    }

    /// <summary>
    /// Fills the cell with given value
    /// </summary>
    /// <param name="row">Row of the cell</param>
    /// <param name="column">Column of the cell</param>
    /// <param name="value">Value to set</param>
    public bool Fill(int row, int column, int value)
    {
        if (!Fillable(row, column, value))
            return false;

        _cells[row, column] = value;
        return true;
    }

    public void Solve()
    {
        var first = Empty;
        for (int row = 0; row < 9; row++)
        {
            for (int column = 0; column < 9; column++)
            {
                var fillables = Fillables(row, column);
                if (fillables.Count == 1)
                    _cells[row, column] = fillables[0];
            }
        }

        if (first > Empty && Empty > 0)
            Solve();
    }
}
