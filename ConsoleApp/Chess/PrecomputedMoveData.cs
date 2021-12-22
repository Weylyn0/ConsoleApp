namespace Chess;

public static class PrecomputedMoveData
{
    /* S, N, W, E, SW, NE, SE, NW */
    public static readonly int[] DirectionOffsets = new int[8] { -8, 8, -1, 1, -9, 9, -7, 7 };

    public static readonly ulong[][] PawnPushPatterns = new ulong[64][];
    public static readonly ulong[][] PawnCapturePatterns = new ulong[64][];
    public static readonly ulong[] KingPatterns = new ulong[64];
    public static readonly ulong[] KnightPatterns = new ulong[64];
    public static readonly ulong[][] RayPatterns = new ulong[64][];

    public static ulong[] Castlings = new ulong[4]
    {
        0x0000000000000060,
        0x000000000000000C,
        0x6000000000000000,
        0x0C00000000000000,
    };

    public const ulong FileA = 0x0101010101010101;
    public const ulong FileB = 0x0202020202020202;
    public const ulong FileC = 0x0404040404040404;
    public const ulong FileD = 0x0808080808080808;
    public const ulong FileE = 0x1010101010101010;
    public const ulong FileF = 0x2020202020202020;
    public const ulong FileG = 0x4040404040404040;
    public const ulong FileH = 0x8080808080808080;

    public const ulong Rank1 = 0x00000000000000FF;
    public const ulong Rank2 = 0x000000000000FF00;
    public const ulong Rank3 = 0x0000000000FF0000;
    public const ulong Rank4 = 0x00000000FF000000;
    public const ulong Rank5 = 0x000000FF00000000;
    public const ulong Rank6 = 0x0000FF0000000000;
    public const ulong Rank7 = 0x00FF000000000000;
    public const ulong Rank8 = 0xFF00000000000000;

    static PrecomputedMoveData()
    {
        for (int square = 0; square < 64; square++)
        {
            ulong bitboard = 1UL << square;

            PawnPushPatterns[square] = new ulong[2]
            {
                bitboard << 8,
                bitboard >> 8,
            };

            PawnCapturePatterns[square] = new ulong[2]
            {
                (bitboard << 7) & ~FileH | (bitboard << 9) & ~FileA,
                (bitboard >> 7) & ~FileA | (bitboard >> 9) & ~FileH,
            };

            ulong king = ((bitboard >> 1) & ~FileH) | ((bitboard << 1) & ~FileA);
            king |= (bitboard | king) >> 8 | (bitboard | king) << 8;
            KingPatterns[square] = king;

            ulong knight = 0UL;
            knight |= (bitboard >> 17) & ~(FileH);
            knight |= (bitboard >> 15) & ~(FileA);
            knight |= (bitboard >> 10) & ~(FileG | FileH);
            knight |= (bitboard >> 6) & ~(FileA | FileB);
            knight |= (bitboard << 6) & ~(FileG | FileH);
            knight |= (bitboard << 10) & ~(FileA | FileB);
            knight |= (bitboard << 15) & ~(FileH);
            knight |= (bitboard << 17) & ~(FileA);
            KnightPatterns[square] = knight;

            ulong south = 0x0080808080808080UL >> (square ^ 63);
            ulong north = 0x0101010101010100UL << square;
            ulong west = (1UL << square) - (1UL << (square & 56));
            ulong east = 2 * ((1UL << (square | 7)) - (1UL << square));

            int d = 8 * (square & 7) - (square & 56);
            int n = -d & (d >> 31);
            int s = d & (-d >> 31);
            ulong diagonal = (0x8040201008040201UL >> s) << n;

            d = 56 - 8 * (square & 7) - (square & 56);
            n = -d & (d >> 31);
            s = d & (-d >> 31);
            ulong antiDiagonal = (0x0102040810204080UL >> s) << n;

            ulong upperBits = ~1UL << square;
            ulong lowerBits = (1UL << square) - 1;

            RayPatterns[square] = new ulong[8]
            {
                south,
                north,
                west,
                east,
                diagonal & lowerBits,
                diagonal & upperBits,
                antiDiagonal & lowerBits,
                antiDiagonal & upperBits,
            };
        }
    }
}
