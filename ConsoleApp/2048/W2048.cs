using System;
using System.Collections.Generic;
using System.Text;

namespace Games;

public class W2048
{
    private int[] Squares { get; set; }

    public int Moves { get; private set; }

    public int this[int index]
    {
        get
        {
            if (index < 0 || 15 < index)
                return -1;

            return Squares[index];
        }
    }

    public string Board
    {
        get
        {
            string board = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    board += Squares[i * 4 + j] + " ";
                }
                board += "\n";
            }
            return board;
        }
    }

    public bool Has2048
    {
        get
        {
            for (int i = 0; i < 16; i++)
                if (Squares[i] == 2048)
                    return true;

            return false;
        }
    }

    public W2048()
    {
        Squares = new int[16];
        Moves = 0;
        PutRandomTwo(-1);
        PutRandomTwo(-4);
    }

    public bool MoveLeft() => Move(-1);
    public bool MoveRight() => Move(1);
    public bool MoveUp() => Move(-4);
    public bool MoveDown() => Move(4);

    private void OrderByZero(ref int[] array)
    {
        var shadow = new int[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != 0)
            {
                for (int j = 0; j < shadow.Length; j++)
                {
                    if (shadow[j] == 0)
                    {
                        shadow[j] = array[i];
                        break;
                    }
                }
            }
        }
        array = shadow;
    }

    private int Slide(int i, int direction)
    {
        if (i < 0 || i > 3)
            return 0;

        int count = 0;
        var values = new int[4];
        for (int j = 0; j < 4; j++)
        {
            values[j] = Squares[FindIndex(i, j, direction)];
        }
        OrderByZero(ref values);
        for (int j = 0; j < 4; j++)
        {
            if (Squares[FindIndex(i, j, direction)] != values[j])
            {
                Squares[FindIndex(i, j, direction)] = values[j];
                count++;
            }
        }
        return count;
    }

    private int Merge(int i, int direction)
    {
        if (i < 0 || i > 3)
            return 0;

        int count = 0;
        for (int j = 0; j < 3; j++)
        {
            int f = FindIndex(i, j, direction);
            if (Squares[f] == 0)
                continue;

            for (int k = j + 1; k < 4; k++)
            {
                int s = FindIndex(i, k, direction);
                if (Squares[s] == 0)
                    continue;

                if (Squares[f] == Squares[s])
                {
                    Squares[f] *= 2;
                    Squares[s] = 0;
                    j++;
                    count++;
                }
            }
        }
        return count;
    }

    private void PutRandomTwo(int direction)
    {
        var empty = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            int index = Math.Abs(4 / direction) * i + Math.Abs(direction) * 3;
            if (Squares[index] == 0)
            {
                empty.Add(index);
            }
        }

        if (empty.Count > 0)
        {
            int index = new Random().Next(0, empty.Count);
            Squares[empty[index]] = 2;
        }
    }

    private bool Move(int direction)
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            count += Merge(i, direction);
            count += Slide(i, direction);
        }

        if (count > 0)
        {
            PutRandomTwo(direction);
            Moves++;
            return true;
        }

        return false;
    }

    private int FindIndex(int i, int j, int direction) => direction switch
    {
        -1 => i * 4 + j,
        1 => i * 4 + 3 - j,
        -4 => j * 4 + i,
        4 => (3 - j) * 4 + i,
        _ => throw new ArgumentException("Invalid direction", nameof(direction))
    };
}
