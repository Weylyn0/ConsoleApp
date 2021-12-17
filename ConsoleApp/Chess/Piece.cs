namespace Chess;

public static class Piece
{
    public const int None = 0;      /* 000 */
    public const int Pawn = 1;      /* 001 */
    public const int King = 2;      /* 010 */
    public const int Knight = 3;    /* 011 */
    public const int Bishop = 5;    /* 101 */
    public const int Rook = 6;      /* 110 */
    public const int Queen = 7;     /* 111 */

    public const int White = 8;     /* 01000 */
    public const int Black = 16;    /* 10000 */

    private const int typeMask = 0b00111;
    private const int colourMask = 0b11000;
    private const int slidingPieceMask = 0b00100;
    private const int bishopOrQueenMask = 0b00101;
    private const int rookOrQueenMask = 0b00110;

    private const string UpperPieceSymbols = "_PKN_BRQ";
    private const string LowerPieceSymbols = "_pkn_brq";

    public static int Type(int piece)
    {
        return piece & typeMask;
    }

    public static int Colour(int piece)
    {
        return piece & colourMask;
    }

    public static bool IsType(int piece, int type)
    {
        return (piece & typeMask) == type;
    }

    public static bool IsColour(int piece, int colour)
    {
        return (piece & colour) == colour;
    }

    public static bool IsWhite(int piece)
    {
        return (piece & White) == White;
    }

    public static bool IsEmpty(int piece)
    {
        return (piece & typeMask) == None || (piece & typeMask) == 4;
    }

    public static bool IsSlidingPiece(int piece)
    {
        return (piece & slidingPieceMask) == 4;
    }

    public static bool IsBishopOrQueen(int piece)
    {
        return (piece & bishopOrQueenMask) == bishopOrQueenMask;
    }

    public static bool IsRookOrQueen(int piece)
    {
        return (piece & rookOrQueenMask) == rookOrQueenMask;
    }

    public static int GetPromotionType(int flag) => flag switch
    {
        MoveFlags.PromotionKnight => Knight,
        MoveFlags.PromotionBishop => Bishop,
        MoveFlags.PromotionRook => Rook,
        MoveFlags.PromotionQueen => Queen,
        _ => 0,
    };

    public static int GetPromotionFlag(int piece) => piece switch
    {
        Knight => MoveFlags.PromotionKnight,
        Bishop => MoveFlags.PromotionBishop,
        Rook => MoveFlags.PromotionRook,
        Queen => MoveFlags.PromotionQueen,
        _ => 0
    };

    public static int GetPieceType(char c)
    {
        return LowerPieceSymbols.IndexOf(char.ToLower(c));
    }

    public static int GetPiece(char c)
    {
        return UpperPieceSymbols.Contains(char.ToUpper(c)) ? (char.IsUpper(c) ? White : Black) | UpperPieceSymbols.IndexOf(char.ToUpper(c)) : None;
    }

    public static char GetSymbol(int piece)
    {
        return IsWhite(piece) ? UpperPieceSymbols[Type(piece)] : LowerPieceSymbols[Type(piece)];
    }
}
