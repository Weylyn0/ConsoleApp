namespace Chess;

public struct BoardHistory
{
    public ulong Key { get; set; }
    public Move LastMove { get; set; }
    public int Castlings { get; set; }
    public int EnpassantSquare { get; set; }
    public int HalfMoveCount { get; set; }
    public int FullMoveCount { get; set; }
}
