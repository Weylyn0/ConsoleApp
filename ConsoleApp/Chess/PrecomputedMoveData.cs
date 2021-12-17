using System;

namespace Chess;

public static class PrecomputedMoveData
{

    /* North, South, West, East, NorthWest, SouthEast, NorthEast, SouthWest */
    public static int[] DirectionsOffsets = { 8, -8, -1, 1, 7, -7, 9, -9 };
    public static int[][] Rays = new int[64][];

    public static int[] KnightPattern = { -17, -15, -10, -6, 6, 10, 15, 17 };

    public static int[] DirectionLookup = new int[127];

    static PrecomputedMoveData()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                int north = 7 - rank;
                int south = rank;
                int west = file;
                int east = 7 - file;

                int squareIndex = rank * 8 + file;
                Rays[squareIndex] = new int[8]
                {
                    north,
                    south,
                    west,
                    east,
                    Math.Min(north, west),
                    Math.Min(south, east),
                    Math.Min(north, east),
                    Math.Min(south, west)
                };
            }
        }

        for (int i = 0; i < 127; i++)
        {
            int offset = i - 63;
            int absOffset = Math.Abs(offset);
            int absDir = 1;
            if (absOffset % 9 == 0)
            {
                absDir = 9;
            }
            else if (absOffset % 8 == 0)
            {
                absDir = 8;
            }
            else if (absOffset % 7 == 0)
            {
                absDir = 7;
            }
            DirectionLookup[i] = Array.IndexOf(DirectionsOffsets, absDir * -Math.Sign(offset));
        }
    }

    public static int Lookup(int startingSquare, int endingSquare)
    {
        int lookup = DirectionLookup[Math.Abs(startingSquare - endingSquare + 63)];
        return lookup;
    }
}
