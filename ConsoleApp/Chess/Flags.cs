namespace Chess;

public class Flags
{
    public const int None = 0;

    /* Move Flags */
    public const int PromotionKnight = 1;
    public const int PromotionBishop = 2;
    public const int PromotionRook = 4;
    public const int PromotionQueen = 8;
    public const int DoublePawnPush = 16;
    public const int EnpassantCapture = 32;
    public const int KingsideCastling = 64;
    public const int QueensideCastling = 128;

    /* Castling Flags */
    public const int WhiteKingsideCastling = 1;
    public const int WhiteQueensideCastling = 2;
    public const int BlackKingsideCastling = 4;
    public const int BlackQueensideCastling = 8;
    public const int WhiteCastlings = 3;
    public const int BlackCastlings = 12;
    public const int AllCastlings = 15;
}
