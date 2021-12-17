namespace Chess;

public struct MoveFlags
{
    public const byte None = 0;
    public const byte PromotionKnight = 1;
    public const byte PromotionBishop = 2;
    public const byte PromotionRook = 4;
    public const byte PromotionQueen = 8;
    public const byte DoublePawnPush = 16;
    public const byte EnpassantCapture = 32;
    public const byte QueensideCastling = 64;
    public const byte KingsideCastling = 128;
}
