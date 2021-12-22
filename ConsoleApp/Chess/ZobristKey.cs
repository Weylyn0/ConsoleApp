using System;

namespace Chess;

public static class ZobristKey
{
    private static readonly int Seed = 1171789088;
    private static readonly Random Random = new(Seed);

    private static readonly ulong[][] PieceKeys = new ulong[64][];
    private static readonly ulong[] CastlingKeys = new ulong[16];
    private static readonly ulong[] EnpassantKeys = new ulong[9];
    private static readonly ulong BlackToMove;

    static ZobristKey()
    {
        for (int square = 0; square < 64; square++)
        {
            PieceKeys[square] = new ulong[16];
            for (int piece = 0; piece < 16; piece++)
            {
                PieceKeys[square][piece] = RandomUnsigned64BitNumber();
            }
        }

        for (int flag = 0; flag < 16; flag++)
            CastlingKeys[flag] = RandomUnsigned64BitNumber();

        for (int file = 0; file < 9; file++)
            EnpassantKeys[file] = RandomUnsigned64BitNumber();

        BlackToMove = RandomUnsigned64BitNumber();
    }

    public static ulong Create(Board board)
    {
        ulong zobristKey = 0UL;
        for (int square = 0; square < 64; square++)
            if (!Piece.IsEmpty(board.Squares[square]))
                zobristKey ^= PieceKeys[square][board.Squares[square]];

        zobristKey ^= EnpassantKeys[board.EnpassantSquare == -1 ? 8 : board.EnpassantSquare & 7];

        if (!board.WhiteToMove)
            zobristKey ^= BlackToMove;

        zobristKey ^= CastlingKeys[board.Castlings];

        return zobristKey;
    }

    private static ulong RandomUnsigned64BitNumber()
    {
        byte[] buffer = new byte[8];
        Random.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }
}
