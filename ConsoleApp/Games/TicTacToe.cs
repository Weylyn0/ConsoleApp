namespace Games;

/// <summary>
/// A TicTacToe game developed by Weylyn
/// </summary>
public class TicTacToe
{
    /// <summary>
    /// Raw value of game (0 00 00 00 00 00 00 00 00 00)
    /// </summary>
    private int Value { get; set; }

    private const int playerOneMask = 0b010101;
    private const int playerTwoMask = 0b101010;

    /// <summary>
    /// Returns the square number with given index (0 - 8)
    /// </summary>
    /// <param name="index">Square's index</param>
    /// <returns>Current value of square (1 for X, 2 for O, 0 for empty square and -1 for invalid index)</returns>
    public int this[int index]
    {
        get
        {
            if (index > 8 || index < 0)
                return -1;

            return ((Value >> (2 * index)) & 0b11);
        }
    }

    public TicTacToe()
    {
        Value = 0;
    }

    /// <summary>
    /// Returns the board of game
    /// </summary>
    public string Board
    {
        get
        {
            string board = "";
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board += ((this[i * 3 + j] == 1) ? "X " : ((this[i * 3 + j] == 2) ? "O " : ". ")) + ((j == 2 && i != 2) ? "\n" : "");

            return board;
        }
    }

    /// <summary>
    /// Returns the current player that will be play
    /// </summary>
    public int Turn
    {
        get
        {
            return (Value >> 18);
        }
    }

    /// <summary>
    /// Returns the winner of the game. If it continues, returns 0 
    /// </summary>
    public int Winner
    {
        get
        {
            foreach (var mask in new[] { playerOneMask, playerTwoMask })
            {
                for (int i = 0; i < 3; i++)
                {
                    int value = (Value >> (6 * i)) & 0b111111;
                    if ((value & mask) == mask)
                    {
                        return (mask >> 4);
                    }

                    value = (this[i]) + (this[i + 3] << 2) + (this[i + 6] << 4);
                    if ((value & mask) == mask)
                    {
                        return (mask >> 4);
                    }
                }

                for (int i = 1; i < 3; i++)
                {
                    int value = (this[4 - 4 / i]) + (this[4] << 2) + (this[4 + 4 / i] << 4);
                    if ((value & mask) == mask)
                    {
                        return (mask >> 4);
                    }
                }
            }

            return 0;
        }
    }

    /// <summary>
    /// Returns false if game's status tie or win, else false
    /// </summary>
    public bool Active
    {
        get
        {
            if (Winner > 0)
            {
                return false;
            }

            else
            {
                for (int i = 0; i < 9; i++)
                {
                    if (this[i] == 0)
                        return true;
                }

                return false;
            }
        }
    }

    /// <summary>
    /// Fill the square with player's symbol
    /// </summary>
    /// <param name="index">Index of square to fill</param>
    public bool Fill(int index)
    {
        if (index > 8 || index < 0)
            return false;

        var square = this[index];

        if (square != 0)
            return false;

        Value += ((Turn == 0) ? 1 : 2) << (2 * index);
        Value += (Turn == 0) ? (1 << 18) : -(1 << 18);
        return true;
    }
}
