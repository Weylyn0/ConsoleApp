namespace Chess;

public struct MoveFlags
{
    public const int None = 0;
    public const int PromotionKnight = 1;
    public const int PromotionBishop = 2;
    public const int PromotionRook = 4;
    public const int PromotionQueen = 8;
    public const int DoublePawnPush = 16;
    public const int EnpassantCapture = 32;
    public const int WhiteQueenCastling = 64;
    public const int WhiteKingCastling = 128;
    public const int BlackQueenCastling = 256;
    public const int BlackKingCastling = 512;
}
