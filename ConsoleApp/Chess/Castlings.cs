namespace Chess;

public struct Castlings
{
    public const byte None = 0;
    public const byte WhiteQueenSide = 1;
    public const byte WhiteKingSide = 2;
    public const byte BlackQueenSide = 4;
    public const byte BlackKingSide = 8;
    public const byte White = WhiteKingSide | WhiteQueenSide;
    public const byte Black = BlackKingSide | BlackQueenSide;
    public const byte All = BlackKingSide | BlackQueenSide | WhiteKingSide | WhiteQueenSide;
}
