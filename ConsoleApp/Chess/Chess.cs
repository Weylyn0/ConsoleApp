using System;
using System.Collections.Generic;
using System.Text;

namespace Chess;

#region Objects

/// <summary>
/// Board object
/// </summary>
public class Board
{
    /// <summary>
    /// Stores pieces
    /// </summary>
    private readonly int[] Squares;

    /// <summary>
    /// Stores moves that played
    /// </summary>
    internal List<Move> Moves;

    #region Board Indexers

    /// <summary>
    /// Returns piece from given square <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns><see cref="int"/></returns>
    public int this[int index]
    {
        get
        {
            if (index < 0 || index > 63)
                return 0;

            return Squares[index];
        }
    }

    /// <summary>
    /// Returns piece from given <paramref name="file"/> and <paramref name="rank"/>
    /// </summary>
    /// <param name="file"></param>
    /// <param name="rank"></param>
    /// <returns><see cref="int"/></returns>
    public int this[int file, int rank]
    {
        get
        {
            if (file < 1 || file > 8)
                return 0;

            if (rank < 1 || rank > 8)
                return 0;

            return Squares[((rank - 1) * 8) + (file - 1)];
        }
    }

    #endregion

    #region Board Properties

    /// <summary>
    /// Returns total move count
    /// </summary>
    public int MoveCount
    {
        get
        {
            lock (Moves)
            {
                return Moves.Count;
            }
        }
    }

    /// <summary>
    /// Returns the colour to move
    /// </summary>
    public int ColourToMove
    {
        get
        {
            return (((MoveCount % 2) == 0) ? Piece.White : Piece.Black);
        }
    }

    /// <summary>
    /// Returns the opponent colour
    /// </summary>
    public int OpponentColour
    {
        get
        {
            return Piece.OppositColour(ColourToMove);
        }
    }

    /// <summary>
    /// Returns the white kings location
    /// </summary>
    public int WhiteKing
    {
        get
        {
            lock (Squares)
            {
                for (int i = 0; i < 64; i++)
                    if (Squares[i] == (Piece.King | Piece.White))
                        return i;

                return -1;
            }
        }
    }

    /// <summary>
    /// Returns the black kings location
    /// </summary>
    public int BlackKing
    {
        get
        {
            lock (Squares)
            {
                for (int i = 0; i < 64; i++)
                    if (Squares[i] == (Piece.King | Piece.Black))
                        return i;

                return -1;
            }
        }
    }

    /// <summary>
    /// Using for testing on console
    /// </summary>
    public string AsString
    {
        get
        {
            lock (Squares)
            {
                string boardString = string.Empty;
                for (int rank = 0; rank < 8; rank++)
                {
                    for (int file = 0; file < 8; file++)
                    {
                        int index = (7 - rank) * 8 + file;
                        boardString += Piece.PieceSymbol(Squares[index]) + " ";
                    }
                    boardString += "\b\n";
                }
                return boardString[..^1];
            }
        }
    }

    #endregion

    #region Board Constructers

    /// <summary>
    /// Generates an empty board
    /// </summary>
    public Board()
    {
        Squares = new int[64];
        Moves = new List<Move>();
    }

    /// <summary>
    /// Generates board with starting chess position
    /// </summary>
    public static Board StartingChessPosition()
    {
        var board = new Board();

        /* White Pieces */
        board.Squares[0] = Piece.Rook | Piece.White;
        board.Squares[1] = Piece.Knight | Piece.White;
        board.Squares[2] = Piece.Bishop | Piece.White;
        board.Squares[3] = Piece.Queen | Piece.White;
        board.Squares[4] = Piece.King | Piece.White;
        board.Squares[5] = Piece.Bishop | Piece.White;
        board.Squares[6] = Piece.Knight | Piece.White;
        board.Squares[7] = Piece.Rook | Piece.White;

        /* White Pawns */
        board.Squares[8] = Piece.Pawn | Piece.White;
        board.Squares[9] = Piece.Pawn | Piece.White;
        board.Squares[10] = Piece.Pawn | Piece.White;
        board.Squares[11] = Piece.Pawn | Piece.White;
        board.Squares[12] = Piece.Pawn | Piece.White;
        board.Squares[13] = Piece.Pawn | Piece.White;
        board.Squares[14] = Piece.Pawn | Piece.White;
        board.Squares[15] = Piece.Pawn | Piece.White;

        /* Black Pawns */
        board.Squares[48] = Piece.Pawn | Piece.Black;
        board.Squares[49] = Piece.Pawn | Piece.Black;
        board.Squares[50] = Piece.Pawn | Piece.Black;
        board.Squares[51] = Piece.Pawn | Piece.Black;
        board.Squares[52] = Piece.Pawn | Piece.Black;
        board.Squares[53] = Piece.Pawn | Piece.Black;
        board.Squares[54] = Piece.Pawn | Piece.Black;
        board.Squares[55] = Piece.Pawn | Piece.Black;

        /* Black Pieces */
        board.Squares[56] = Piece.Rook | Piece.Black;
        board.Squares[57] = Piece.Knight | Piece.Black;
        board.Squares[58] = Piece.Bishop | Piece.Black;
        board.Squares[59] = Piece.Queen | Piece.Black;
        board.Squares[60] = Piece.King | Piece.Black;
        board.Squares[61] = Piece.Bishop | Piece.Black;
        board.Squares[62] = Piece.Knight | Piece.Black;
        board.Squares[63] = Piece.Rook | Piece.Black;

        return board;
    }

    #endregion

    #region Move Generate

    /// <summary>
    /// Calculates legal moves for <see cref="ColourToMove"/>
    /// </summary>
    /// <returns><see cref="List{Move}"/></returns>
    public List<Move> CalculateLegalMoves()
    {
        return CalculateLegalMoves(ColourToMove, OpponentColour);
    }

    /// <summary>
    /// Calculates legal moves for <see cref="OpponentColour"/>
    /// </summary>
    /// <returns><see cref="List{Move}"/></returns>
    public List<Move> CalculateLegalMovesForOpponent()
    {
        return CalculateLegalMoves(OpponentColour, ColourToMove);
    }

    /// <summary>
    /// Calculates legal moves for <paramref name="friendlyColour"/>
    /// </summary>
    /// <param name="friendlyColour"></param>
    /// <param name="opponentColour"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> CalculateLegalMoves(int friendlyColour, int opponentColour)
    {
        var moves = new List<Move>();
        for (int squareIndex = 0; squareIndex < 64; squareIndex++)
        {
            if (!Piece.IsEmpty(Squares[squareIndex]))
            {
                if (Piece.IsColour(Squares[squareIndex], friendlyColour))
                {
                    if (Piece.IsSlidingPiece(Squares[squareIndex]))
                    {
                        moves.AddRange(GenerateSlidingMoves(squareIndex, friendlyColour, opponentColour));
                    }

                    else if (Piece.IsType(Squares[squareIndex], Piece.Knight))
                    {
                        moves.AddRange(GenerateKnightMoves(squareIndex, friendlyColour, opponentColour));
                    }

                    else if (Piece.IsType(Squares[squareIndex], Piece.King))
                    {
                        moves.AddRange(GenerateKingMoves(squareIndex, friendlyColour, opponentColour));
                    }

                    else
                    {
                        moves.AddRange(GeneratePawnMoves(squareIndex, friendlyColour, opponentColour));
                    }
                }
            }
        }
        return moves;
    }

    /// <summary>
    /// Generates queen, rook and bishop moves for <paramref name="friendlyColour"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="friendlyColour"></param>
    /// <param name="opponentColour"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateSlidingMoves(int squareIndex, int friendlyColour, int opponentColour)
    {
        var moves = new List<Move>();
        int startDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Bishop) ? 4 : 0;
        int endDirectionIndex = Piece.IsType(Squares[squareIndex], Piece.Rook) ? 4 : 8;
        for (int directionIndex = startDirectionIndex; directionIndex < endDirectionIndex; directionIndex++)
        {
            for (int n = 0; n < PrecomputedMoveData.NumSquaresToEdge[squareIndex][directionIndex]; n++)
            {
                int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex] * (n + 1);

                /* Blocked by friendly piece */
                if (Piece.IsColour(Squares[targetSquare], friendlyColour))
                    break;

                moves.Add(new Move(squareIndex, targetSquare));

                /* Capturing opponent piece */
                if (Piece.IsColour(Squares[targetSquare], opponentColour))
                    break;
            }
        }
        return moves;
    }

    /// <summary>
    /// Generates knight moves for <paramref name="friendlyColour"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="friendlyColour"></param>
    /// <param name="opponentColour"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateKnightMoves(int squareIndex, int friendlyColour, int opponentColour)
    {
        var moves = new List<Move>();
        for (int knightSquareIndex = 0; knightSquareIndex < 8; knightSquareIndex++)
        {
            int targetSquare = PrecomputedMoveData.KnightSquares[squareIndex][knightSquareIndex];

            /* targetSquare is out of bounds */
            if (targetSquare == -1)
                continue;

            /* Square blocked by friendly piece */
            if (Piece.IsColour(Squares[targetSquare], friendlyColour))
                continue;

            moves.Add(new Move(squareIndex, targetSquare));
        }
        return moves;
    }

    /// <summary>
    /// Generates king moves for <paramref name="friendlyColour"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="friendlyColour"></param>
    /// <param name="opponentColour"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GenerateKingMoves(int squareIndex, int friendlyColour, int opponentColour)
    {
        var moves = new List<Move>();
        for (int directionIndex = 0; directionIndex < 8; directionIndex++)
        {
            int targetSquare = squareIndex + PrecomputedMoveData.DirectionOffsets[directionIndex];

            /* Square out of bounds */
            if (targetSquare < 0 || targetSquare > 63)
                continue;

            /* Square blocked by friendly piece */
            if (Piece.IsColour(Squares[targetSquare], friendlyColour))
                continue;

            moves.Add(new Move(squareIndex, targetSquare));
        }

        if (Moves.FindAll(x => x.Piece == Squares[squareIndex]).Count == 0)
        {
            var rookMoves = Moves.FindAll(x => x.Piece == (friendlyColour | Piece.Rook));
            if (rookMoves.FindAll(x => x.Index == ((friendlyColour == Piece.White) ? 0 : 56)).Count == 0)
            {
                moves.Add(new Move(squareIndex, squareIndex - 2, Move.Flags.QueenCastling));
            }

            if (rookMoves.FindAll(x => x.Index == ((friendlyColour == Piece.White) ? 7 : 63)).Count == 0)
            {
                moves.Add(new Move(squareIndex, squareIndex + 2, Move.Flags.KingCastling));
            }
        }

        return moves;
    }

    /// <summary>
    /// Generates pawn moves for <paramref name="friendlyColour"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="friendlyColour"></param>
    /// <param name="opponentColour"></param>
    /// <returns><see cref="List{Move}"/></returns>
    private List<Move> GeneratePawnMoves(int squareIndex, int friendlyColour, int opponentColour)
    {
        var moves = new List<Move>();
        int rank = squareIndex / 8;
        int file = squareIndex % 8;
        int startingRank = Piece.IsColour(Squares[squareIndex], Piece.White) ? 1 : 6;
        int promotionRank = 7 - startingRank;
        int direction = Piece.IsColour(Squares[squareIndex], Piece.White) ? 8 : -8;

        /* Move forward */
        int targetSquare = squareIndex + direction;
        if (Piece.IsEmpty(Squares[targetSquare]))
        {
            if (rank == promotionRank)
            {
                for (int i = 0; i < 4; i++)
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.PromotionKnight << i));
                }
            }

            else
            {
                moves.Add(new Move(squareIndex, targetSquare));
            }

            /* Can play two squares on starting rank */
            if (rank == startingRank)
            {
                targetSquare += direction;
                if (Piece.IsEmpty(Squares[targetSquare]))
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.DoublePawnPush));
                }
            }
        }

        /* Move with capture */
        if (file > 0)
        {
            targetSquare = squareIndex + direction - 1;
            if (Piece.IsColour(Squares[targetSquare], opponentColour))
            {
                if (rank == promotionRank)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        moves.Add(new Move(squareIndex, targetSquare, Move.Flags.PromotionKnight << i));
                    }
                }

                else
                {
                    moves.Add(new Move(squareIndex, targetSquare));
                }
            }

            else if (MoveCount > 0)
            {
                var lastMove = Moves[^1];
                if (lastMove.HasFlag(Move.Flags.DoublePawnPush) && lastMove.TargetIndex == squareIndex - 1)
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.EnpassantCapture));
                }
            }
        }

        if (file < 8)
        {
            targetSquare = squareIndex + direction + 1;
            if (Piece.IsColour(Squares[targetSquare], opponentColour))
            {
                if (rank == promotionRank)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        moves.Add(new Move(squareIndex, targetSquare, Move.Flags.PromotionKnight << i));
                    }
                }

                else
                {
                    moves.Add(new Move(squareIndex, targetSquare));
                }
            }

            else if (MoveCount > 0)
            {
                var lastMove = Moves[^1];
                if (lastMove.HasFlag(Move.Flags.DoublePawnPush) && lastMove.TargetIndex == squareIndex + 1)
                {
                    moves.Add(new Move(squareIndex, targetSquare, Move.Flags.EnpassantCapture));
                }
            }
        }

        return moves;
    }

    #endregion

    #region Execute Move

    public bool MakeMove(string san)
    {
        var move = Move.ParseSan(this, san);
        return MakeMove(move.Index, move.TargetIndex, move.Flag);
    }

    /// <summary>
    /// Makes move without flag
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="targetIndex"></param>
    /// <returns><see cref="bool"/></returns>
    public bool MakeMove(int currentIndex, int targetIndex)
    {
        return MakeMove(currentIndex, targetIndex, Move.Flags.None);
    }

    /// <summary>
    /// Makes move with flag
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="targetIndex"></param>
    /// <param name="flag"></param>
    /// <returns><see cref="bool"/></returns>
    public bool MakeMove(int currentIndex, int targetIndex, int flag)
    {
        if (currentIndex < 0 || currentIndex > 63)
            return false;

        if (targetIndex < 0 || targetIndex > 63)
            return false;

        var legalMoves = CalculateLegalMoves();
        var opponentMoves = CalculateLegalMovesForOpponent();

        foreach (var legalMove in legalMoves)
        {
            if (legalMove.Index == currentIndex && legalMove.TargetIndex == targetIndex && legalMove.Flag == flag)
            {
                legalMove.WithPieces(Squares[currentIndex], Squares[targetIndex]);
                Squares[currentIndex] = Piece.None;

                if (Piece.IsType(legalMove.Piece, Piece.Pawn))
                {
                    if (legalMove.HasPromotionFlag())
                    {
                        Squares[targetIndex] = Piece.Promote(legalMove.Piece, legalMove.GetPromotionType());
                    }

                    else if (legalMove.HasFlag(Move.Flags.EnpassantCapture))
                    {
                        legalMove.WithPieces(legalMove.Piece, Squares[targetIndex + (Piece.IsColour(ColourToMove, Piece.White) ? -8 : 8)]);
                        Squares[targetIndex + (Piece.IsColour(ColourToMove, Piece.White) ? -8 : 8)] = Piece.None;
                        Squares[targetIndex] = legalMove.Piece;
                    }

                    else
                    {
                        Squares[targetIndex] = legalMove.Piece;
                    }
                }

                else if (Piece.IsType(legalMove.Piece, Piece.King))
                {
                    if (legalMove.HasCastlingFlag())
                    {
                        int startIndex = legalMove.HasFlag(Move.Flags.KingCastling) ? 0 : -4;
                        int endIndex = (startIndex == 0) ? 3 : 0;
                        int rookIndex = legalMove.Index + ((startIndex == 0) ? endIndex : startIndex);
                        bool castlingFailed = false;
                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            if (!Piece.IsEmpty(Squares[i + legalMove.Index]) && i > startIndex && i < endIndex)
                            {
                                castlingFailed = true;
                                break;
                            }

                            if (IsUnderThreat(i + legalMove.Index, opponentMoves))
                            {
                                castlingFailed = true;
                                break;
                            }
                        }

                        if (castlingFailed)
                        {
                            Squares[currentIndex] = legalMove.Piece;
                            return false;
                        }

                        Squares[legalMove.TargetIndex] = legalMove.Piece;
                        Squares[(legalMove.Index + legalMove.TargetIndex) / 2] = Squares[rookIndex];
                        Squares[rookIndex] = Piece.None;
                    }

                    else
                    {
                        Squares[targetIndex] = legalMove.Piece;
                    }
                }

                else
                {
                    Squares[targetIndex] = legalMove.Piece;
                }

                opponentMoves = CalculateLegalMovesForOpponent();
                int kingSquare = Piece.IsColour(ColourToMove, Piece.White) ? WhiteKing : BlackKing;
                Moves.Add(legalMove);
                if (IsUnderThreat(kingSquare, opponentMoves))
                {
                    Takeback();
                    return false;
                }

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the <paramref name="squareIndex"/> is under threat by <paramref name="opponentMoves"/>
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="opponentMoves"></param>
    /// <returns><see cref="bool"/></returns>
    private bool IsUnderThreat(int squareIndex, List<Move> opponentMoves)
    {
        int opponentColour = Piece.OppositColour(Squares[squareIndex]);
        if (opponentMoves.FindAll(x => x.TargetIndex == squareIndex).Count > 0)
            return true;

        return false;
    }

    /// <summary>
    /// Takebacks the last move
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool Takeback()
    {
        if (Moves.Count == 0)
            return false;

        var move = Moves[^1];
        if (move.GetType() != typeof(Move) || move.HasSystemFlag())
            return false;

        if (move.HasPromotionFlag())
        {
            Squares[move.Index] = Piece.Pawn | Piece.Colour(move.Piece);
            Squares[move.TargetIndex] = move.TargetPiece;
        }

        else if (move.HasFlag(Move.Flags.EnpassantCapture))
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = Piece.None;
            Squares[move.TargetIndex + ((move.TargetIndex - move.Index) > 0 ? -8 : 8)] = move.TargetPiece;
        }

        else if (move.HasCastlingFlag())
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = Piece.None;
            Squares[(move.Index + move.TargetIndex) / 2] = Piece.None;
            Squares[(Piece.IsColour(move.Piece, Piece.White) ? 0 : 56) + (move.HasFlag(Move.Flags.QueenCastling) ? 0 : 7)] = Piece.Rook | Piece.Colour(move.Piece);
        }

        else
        {
            Squares[move.Index] = move.Piece;
            Squares[move.TargetIndex] = move.TargetPiece;
        }

        Moves.RemoveAt(Moves.Count - 1);
        return true;
    }

    #endregion

    /// <summary>
    /// Put piece into board as illegal
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="piece"></param>
    public void PutPiece(int squareIndex, int piece)
    {
        if (squareIndex < 0 || squareIndex > 63)
            return;

        Squares[squareIndex] = piece;
    }
}

/// <summary>
/// Piece object
/// </summary>
public static class Piece
{
    public const int None = 0;      /* 000 */
    public const int Pawn = 1;      /* 001 */
    public const int King = 2;      /* 010 */
    public const int Knight = 3;    /* 011 */
    public const int Bishop = 4;    /* 100 */
    public const int Rook = 5;      /* 101 */
    public const int Queen = 6;     /* 110 */

    public const int White = 8;     /* 01... */
    public const int Black = 16;    /* 10... */

    public const string Symbols = ".PKNBRQ";

    /// <summary>
    /// Returns <paramref name="piece"/> colour
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int Colour(int piece)
    {
        return (piece & 0b11000);
    }

    /// <summary>
    /// Returns opposit of <paramref name="piece"/> colour
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int OppositColour(int piece)
    {
        return ((piece & 0b11000) == White) ? Black : White;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> type
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="int"/></returns>
    public static int Type(int piece)
    {
        return (piece & 0b111);
    }

    /// <summary>
    /// Returns <paramref name="piece"/> has <paramref name="colour"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="colour"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsColour(int piece, int colour)
    {
        return (piece & 0b11000) == colour;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is <paramref name="type"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="type"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsType(int piece, int type)
    {
        return (piece & 0b111) == type;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is an empty square
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsEmpty(int piece)
    {
        return (piece & 0b111) == None;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is queen, rook or bishop
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsSlidingPiece(int piece)
    {
        return (piece & 0b100) == 0b100;
    }

    /// <summary>
    /// Returns <paramref name="piece"/> is a promotion type piece
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsPromotionType(int piece)
    {
        return IsSlidingPiece(piece) || IsType(piece, Knight);
    }

    /// <summary>
    /// Promotes <paramref name="piece"/> to <paramref name="type"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="type"></param>
    /// <returns><see cref="int"/></returns>
    public static int Promote(int piece, int type)
    {
        return (piece & 0b11000) | type;
    }

    /// <summary>
    /// Returns symbol from <paramref name="piece"/>
    /// </summary>
    /// <param name="piece"></param>
    /// <returns><see cref="string"/></returns>
    public static string PieceSymbol(int piece)
    {
        string symbol = $"{Symbols[(piece & 0b111)]}";
        return IsColour(piece, Black) ? symbol.ToLower() : symbol;
    }

    /// <summary>
    /// Returns piece from <paramref name="symbol"/>
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns><see cref="int"/></returns>
    public static int FromSymbol(char symbol)
    {
        int piece = Symbols.IndexOf(symbol);
        piece = piece == -1 ? 0 : piece;
        return piece | (char.IsLower(symbol) ? Black : White);
    }
}

/// <summary>
/// Chess move object
/// </summary>
public struct Move
{
    private int Current;
    private int Target;
    private int MoveFlag;

    private const int indexMask = 0b111111;
    private const int pieceMask = 0b11111;

    /// <summary>
    /// Move flags
    /// </summary>
    public readonly struct Flags
    {
        public static readonly int None = 0;
        public static readonly int PromotionKnight = 1;
        public static readonly int PromotionBishop = 2;
        public static readonly int PromotionRook = 4;
        public static readonly int PromotionQueen = 8;
        public static readonly int DoublePawnPush = 16;
        public static readonly int EnpassantCapture = 32;
        public static readonly int KingCastling = 64;
        public static readonly int QueenCastling = 128;
        public static readonly int System = 256;
    }

    #region Move Properties

    /// <summary>
    /// Move's current index
    /// </summary>
    public int Index
    {
        get
        {
            return ((Current >> 5) & indexMask);
        }
    }

    /// <summary>
    /// Move's target index
    /// </summary>
    public int TargetIndex
    {
        get
        {
            return ((Target >> 5) & indexMask);
        }
    }

    /// <summary>
    /// Move's piece
    /// </summary>
    public int Piece
    {
        get
        {
            return (Current & pieceMask);
        }
    }

    /// <summary>
    /// Move's target piece
    /// </summary>
    public int TargetPiece
    {
        get
        {
            return (Target & pieceMask);
        }
    }

    /// <summary>
    /// Move's flag
    /// </summary>
    public int Flag
    {
        get
        {
            return MoveFlag;
        }
    }

    #endregion

    #region Move Methods

    /// <summary>
    /// Generates move with no flag
    /// </summary>
    /// <param name="current">Current square index</param>
    /// <param name="target">Target square index</param>
    public Move(int current, int target) : this(current, target, Flags.None)
    {
        
    }

    /// <summary>
    /// Generates move with flag
    /// </summary>
    /// <param name="current">Current square index</param>
    /// <param name="target">Target square index</param>
    /// <param name="flag">Move flag</param>
    public Move(int current, int target, int flag)
    {
        Current = (current << 5);
        Target = (target << 5);
        MoveFlag = flag;
    }

    /// <summary>
    /// Assigns pieces to move
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="targetPiece"></param>
    public void WithPieces(int piece, int targetPiece)
    {
        Current = (Index << 5) + piece;
        Target = (TargetIndex << 5) + targetPiece;
    }

    /// <summary>
    /// Returns move has given <paramref name="flag"/>
    /// </summary>
    /// <param name="flag"></param>
    /// <returns><see cref="bool"/></returns>
    public bool HasFlag(int flag)
    {
        return (Flag == flag);
    }

    /// <summary>
    /// Returns move contains no flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool NoFlag()
    {
        return (Flag == Flags.None);
    }

    /// <summary>
    /// Returns move has any promotion flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasPromotionFlag()
    {
        return (Flags.PromotionKnight <= Flag) && (Flag <= Flags.PromotionQueen);
    }

    /// <summary>
    /// Returns promotion piece type if move has promotion flag
    /// </summary>
    /// <returns><see cref="int"/></returns>
    public int GetPromotionType()
    {
        for (int i = 0; i < 4; i++)
            if ((Flag >> i) == 1)
                return i + 3;

        return 0;
    }

    /// <summary>
    /// Returns move has king or queen side castlings
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasCastlingFlag()
    {
        return (Flag == Flags.KingCastling) || (Flag == Flags.QueenCastling);
    }

    /// <summary>
    /// Returns move has system flag
    /// </summary>
    /// <returns><see cref="bool"/></returns>
    public bool HasSystemFlag()
    {
        return (Flag == Flags.System);
    }

    #endregion

    #region Move Formats

    private static readonly string Files = "abcdefgh";

    /// <summary>
    /// Converts <paramref name="squareIndex"/> to Universal Chess Interface
    /// </summary>
    /// <param name="move"></param>
    /// <returns><see cref="string"/></returns>
    public static string Uci(int squareIndex)
    {
        return string.Empty + Files[squareIndex % 8] + (squareIndex / 8 + 1);
    }

    /// <summary>
    /// Converts <paramref name="move"/> to Universal Chess Interface
    /// </summary>
    /// <param name="move"></param>
    /// <returns><see cref="string"/></returns>
    public static string Uci(Move move)
    {
        return Uci(move.Index) + Uci(move.TargetIndex);
    }

    /// <summary>
    /// Converts <paramref name="move"/> to Standard Algebraic Notation
    /// </summary>
    /// <param name="move"></param>
    /// <returns><see cref="string"/></returns>
    public static string San(Move move)
    {
        return (Chess.Piece.IsType(move.Piece, Chess.Piece.Pawn) ? string.Empty : Chess.Piece.PieceSymbol(move.Piece)) + (Chess.Piece.IsEmpty(move.TargetPiece) ? string.Empty : "x") + Uci(move.TargetIndex);
    }

    public static Move ParseSan(Board board, string san)
    {
        /* TODO Update Parsing using RegEx */
        san = san.Trim();
        string clearedSan = san.Replace("x", "").Replace("+", "").Replace("#", "").Trim();
        if (clearedSan.Length == 0)
            throw new InvalidSanStringException(san);

        var legalMoves = board.CalculateLegalMoves();
        int direction = Chess.Piece.IsColour(board.ColourToMove, Chess.Piece.White) ? 8 : -8;
        int startingRank = (direction == 8) ? 1 : 6;
        int promotionRank = 7 - startingRank;
        int kingSquare = (direction == 8) ? board.WhiteKing : board.BlackKing;

        if (san == "0-0")
        {   
            return new Move(kingSquare, kingSquare + 2, Flags.KingCastling);
        }

        if (san == "0-0-0")
        {
            return new Move(kingSquare, kingSquare - 2, Flags.QueenCastling);
        }

        if (Files.Contains(clearedSan[0]))
        {
            if (clearedSan.Length == 2)
            {
                int squareIndex = ParseUci(clearedSan);
                var move = legalMoves.Find(move => (move.TargetIndex == squareIndex) && Chess.Piece.IsType(board[move.Index], Chess.Piece.Pawn));
                if (!move.Equals(default))
                    return move;
            }

            else if (clearedSan.Length == 3)
            {
                int squareIndex = ParseUci(clearedSan[1..]);
                int file = Files.IndexOf(clearedSan[0]);
                var move = legalMoves.Find(move => ((move.Target == squareIndex) && ((move.Index % 8) == file)) && Chess.Piece.IsType(board[move.Index], Chess.Piece.Pawn));
                if (!move.Equals(default))
                    return move;
            }
        }
        
        int piece = Chess.Piece.FromSymbol(clearedSan[0]);
        if (clearedSan.Length > 2 && !Chess.Piece.IsType(piece, Chess.Piece.None)) 
        {
            int squareIndex = ParseUci(clearedSan[1..3]);
            if (clearedSan.Length == 5)
            {
                if (clearedSan[3] == '=')
                {
                    int promotionType = Chess.Piece.FromSymbol(char.ToLower(clearedSan[4]));
                    if (Chess.Piece.IsPromotionType(promotionType))
                    {
                        var move = legalMoves.Find(move => Chess.Piece.IsType(board[move.Index], Chess.Piece.Type(piece)) && move.TargetIndex == squareIndex && move.GetPromotionType() == promotionType);
                        if (!move.Equals(default))
                            return move;
                    }
                }
            }

            else
            {
                var move = legalMoves.Find(move => Chess.Piece.IsType(board[move.Index], Chess.Piece.Type(piece)) && move.TargetIndex == squareIndex);
                if (!move.Equals(default))
                    return move;
            }
        }

        throw new InvalidSanStringException(san);
    }

    public static int ParseUci(string uci)
    {
        if (uci.Length != 2)
            throw new InvalidUciStringException(uci);

        int file = Files.IndexOf(uci[0]);
        if (file == -1 || !int.TryParse($"{uci[1]}", out int rank))
            throw new InvalidUciStringException(uci);

        if (rank < 1 || rank > 8)
            throw new InvalidUciStringException(uci);

        return ((rank - 1) * 8 + file);
    }

    #endregion
}

#endregion

#region Utility

/// <summary>
/// Contains precomputed move data for using in move functions
/// </summary>
public static class PrecomputedMoveData
{
    /// <summary>
    /// Direction offsets to North, South, West, East, NorthWest, SouthEast, NorthEast, SouthWest
    /// </summary>
    public static readonly int[] DirectionOffsets = new int[] { 8, -8, -1, 1, 7, -7, 9, -9 };

    /// <summary>
    /// Stores number squares to board edge for each direction
    /// </summary>
    public static readonly int[][] NumSquaresToEdge = new int[64][];

    /// <summary>
    /// Stores square index for knight moves
    /// </summary>
    public static readonly int[][] KnightSquares = new int[64][];

    static PrecomputedMoveData()
    {
        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                int numNorth = 7 - rank;
                int numSouth = rank;
                int numWest = file;
                int numEast = 7 - file;

                int squareIndex = rank * 8 + file;

                NumSquaresToEdge[squareIndex] = new int[]
                {
                    numNorth,
                    numSouth,
                    numWest,
                    numEast,
                    Math.Min(numNorth, numWest),
                    Math.Min(numSouth, numEast),
                    Math.Min(numNorth, numEast),
                    Math.Min(numSouth, numWest)
                };

                KnightSquares[squareIndex] = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
 
                if (rank > 1)
                {
                    if (file > 0)
                        KnightSquares[squareIndex][0] = squareIndex - 17;

                    if (file < 7)
                        KnightSquares[squareIndex][1] = squareIndex - 15;
                }

                if (rank < 6)
                {
                    if (file > 0)
                        KnightSquares[squareIndex][2] = squareIndex + 15;

                    if (file < 7)
                        KnightSquares[squareIndex][3] = squareIndex + 17;
                }

                if (file > 1)
                {
                    if (rank > 0)
                        KnightSquares[squareIndex][4] = squareIndex - 10;

                    if (rank < 7)
                        KnightSquares[squareIndex][5] = squareIndex + 6;
                }

                if (file < 6)
                {
                    if (rank > 0)
                        KnightSquares[squareIndex][6] = squareIndex - 6;

                    if (rank < 7)
                        KnightSquares[squareIndex][7] = squareIndex + 10;
                }
            }
        }
    }
}

public static class PortableGameNotation
{
    /// <summary>
    /// Generates Portable Game Notation from <paramref name="board"/>
    /// </summary>
    /// <param name="board"></param>
    /// <param name="white">White Player</param>
    /// <param name="black">Black Player</param>
    /// <returns><see cref="string"/></returns>
    public static string GeneratePgn(Board board, string white = "Unknown", string black = "Unknown")
    {
        string pgn = $"[White \"{white}\"]\n[Black \"{black}\"]\n[Date \"{DateTime.UtcNow.ToShortDateString()}\"]\n\n";
        for (int i = 0; i < (board.MoveCount + 1) / 2; i++)
        {
            pgn += $"{(i + 1)}. {Move.San(board.Moves[i * 2])} ";
            pgn += ((i * 2 + 1) < board.MoveCount) ? $"{Move.San(board.Moves[i * 2 + 1])} " : string.Empty;
        }
        return pgn;
    }

    /// <summary>
    /// Generates <see cref="Board"/> from <paramref name="pgn"/>
    /// </summary>
    /// <param name="pgn">Portable Game Notation to load</param>
    /// <returns><see cref="Board"/></returns>
    public static Board LoadPgn(string pgn)
    {
        var board = new Board();
        /* TODO */
        return board;
    }
}

public static class ForsythEdwardsNotation
{
    /// <summary>
    /// Forsyth-Edwards Notation for starting chess position
    /// </summary>
    public static readonly string StaringPositionFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    /// <summary>
    /// Generates Forsyth-Edwards Notation from <paramref name="board"/>
    /// </summary>
    /// <param name="board"></param>
    /// <returns><see cref="string"/></returns>
    public static string GenerateFen(Board board)
    {
        string fen = string.Empty;
        int empty;
        for (int rank = 0; rank < 8; rank++)
        {
            empty = 0;
            for (int file = 0; file < 8; file++)
            {
                int squareIndex = (7 - rank) * 8 + file;
                if (Piece.IsEmpty(board[squareIndex]))
                {
                    empty++;
                }

                else
                {
                    fen += (empty > 0 ? empty : string.Empty) + Piece.PieceSymbol(board[squareIndex]);
                    empty = 0;
                }
            }

            if (empty > 0)
                fen += empty;

            fen += "/";
        }

        fen += $"\b {(Piece.IsColour(board.ColourToMove, Piece.White) ? 'w' : 'b')}";

        string castlings = string.Empty;
        if (board.Moves.FindAll(x => x.Piece == (Piece.King | Piece.White)).Count == 0)
        {
            if (board.Moves.FindAll(x => x.Index == 7).Count == 0)
                castlings += "K";

            if (board.Moves.FindAll(x => x.Index == 0).Count == 0)
                castlings += "Q";
        }
        if (board.Moves.FindAll(x => x.Piece == (Piece.King | Piece.Black)).Count == 0)
        {
            if (board.Moves.FindAll(x => x.Index == 63).Count == 0)
                castlings += "k";

            if (board.Moves.FindAll(x => x.Index == 56).Count == 0)
                castlings += "q";
        }
        fen += $" {(string.IsNullOrEmpty(castlings) ? '-' : castlings)}";

        string enpassantSquare = string.Empty;
        if (board.MoveCount > 0)
        {
            var lastMove = board.Moves[^1];
            if (!lastMove.HasSystemFlag())
            {
                if (lastMove.HasFlag(Move.Flags.DoublePawnPush))
                {
                    enpassantSquare = Move.Uci(lastMove.TargetIndex + (((lastMove.TargetIndex / 8) == 3) ? -8 : 8));
                }
            }
        }
        fen += $" {(string.IsNullOrEmpty(enpassantSquare) ? '-' : enpassantSquare)}";
        int halfMove = 0;
        for (int i = board.MoveCount - 1; i > -1; i--)
        {
            if (board.Moves[i].Equals(null))
                continue;

            if (!Piece.IsEmpty(board.Moves[i].TargetPiece))
            {
                halfMove = board.MoveCount - 1 - i;
                break;
            }
        }
        fen += $" {halfMove} {(board.MoveCount + 2) / 2}";
        return fen;
    }

    /// <summary>
    /// Generates <see cref="Board"/> from given <paramref name="fen"/>
    /// </summary>
    /// <param name="fen">Forsyth-Edwards Notation to load</param>
    /// <returns><see cref="Board"/></returns>
    /// <exception cref="InvalidFenException"></exception>
    public static Board LoadFen(string fen)
    {
        try
        {
            Board board = new Board();
            string[] splittedFen = fen.Split(' ');
            if (splittedFen.Length != 6)
                throw new InvalidFenException(fen);

            string[] vs = splittedFen[0].Split('/');
            if (vs.Length != 8)
                throw new InvalidFenException(fen);

            int file;
            for (int rank = 0; rank < 8; rank++)
            {
                file = 0;
                foreach (var character in vs[rank])
                {
                    if (char.IsDigit(character))
                        file += int.Parse($"{character}") - 1;

                    else
                        board.PutPiece((7 - rank) * 8 + file, Piece.FromSymbol(character));
                    file++;
                }
            }
            int turn = (splittedFen[1] == "w") ? Piece.White : Piece.Black;

            if (!splittedFen[2].Contains("Q"))
                board.Moves.Add(new Move(0, 0, Move.Flags.System));

            if (!splittedFen[2].Contains("K"))
                board.Moves.Add(new Move(7, 7, Move.Flags.System));

            if (!splittedFen[2].Contains("q"))
                board.Moves.Add(new Move(56, 56, Move.Flags.System));

            if (!splittedFen[2].Contains("k"))
                board.Moves.Add(new Move(63, 63, Move.Flags.System));

            if (!int.TryParse(splittedFen[5], out int moveCount))
                throw new InvalidFenException(fen);

            for (int i = 0; i < board.MoveCount - ((moveCount - 1) * 2 + (turn == Piece.White ? 0 : 1)); i++)
                board.Moves.Add(new Move(-1, -1, Move.Flags.System));

            return board;
        }

        catch
        {
            throw new InvalidFenException(fen);
        }
    }
}

#endregion

#region Exceptions
public class InvalidFenException : Exception
{
    public string Fen { get; set; }

    public InvalidFenException(string fen) : base("Passed FEN is not a valid item.")
    {
        Fen = fen;
    }
}

public class InvalidUciStringException : Exception
{
    public string Uci { get; set; }

    public InvalidUciStringException(string uci) : base("Passed UCI is not a valid item for this.")
    {
        Uci = uci;
    }
}

public class InvalidSanStringException : Exception
{
    public string San { get; set; }

    public InvalidSanStringException(string san) : base("Passed SAN is not a valid item for this.")
    {
        San = san;
    }
}

#endregion