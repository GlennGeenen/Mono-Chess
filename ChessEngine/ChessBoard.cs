namespace MonoChess
{
    internal static class Constants
    {
        public const int SquareSize = 60;
        public const int NumberOfRanks = 8;
        public const int NumberOfFiles = 8;
        public const int GameOverValue = 32767;
        public const int ply = 5;
    }

    public enum ChessPieceColor
    {
        White,
        Black
    }

    public enum ChessPieceType
    {
        King,
        Queen,
        Rook,
        Bishop,
        Knight,
        Pawn,
        None
    }

    public class ChessBoard
    {
        internal int Score;
        internal bool blackInCheck;
        internal bool blackInMate;
        internal bool whiteInCheck;
        internal bool whiteInMate;
        internal bool staleMate;
        internal byte FiftyMove;
        internal byte RepeatedMove;
        internal bool blackCastled;
        internal bool whiteCastled;

        internal bool EndGamePhase;
        internal uint MoveCount;
        internal byte EnPassantPosition;
        internal bool InsufficientMaterial;

        internal ChessPieceColor EnPassantColor;
        internal ChessPieceColor WhoseMove;
        internal MoveContent LastMove;
        internal ChessPiece[] pieces;

        public ChessBoard()
        {
			Score = 0;
			blackInCheck = false;
			blackInMate = false;
			whiteInCheck = false;
			whiteInMate = false;
			staleMate = false;
			FiftyMove = 0;
			RepeatedMove = 0;
			blackCastled = false;
			whiteCastled = false;
			EndGamePhase = false;
			MoveCount = 0;
			EnPassantPosition = 0;
			InsufficientMaterial = false;
			EnPassantColor = ChessPieceColor.White;
			WhoseMove = ChessPieceColor.White;
			LastMove = new MoveContent();
			pieces = new ChessPiece[64];
        }

        public ChessBoard(ChessBoard board)
        {
            Score = board.Score;
            EndGamePhase = board.EndGamePhase;
            WhoseMove = board.WhoseMove;
            MoveCount = board.MoveCount;
            FiftyMove = board.FiftyMove;
            RepeatedMove = board.RepeatedMove;

            blackCastled = board.blackCastled;
            blackInCheck = board.blackInCheck;
            blackInMate = board.blackInMate;
            whiteCastled = board.whiteCastled;
            whiteInCheck = board.whiteInCheck;
            whiteInMate = board.whiteInMate;
            staleMate = board.staleMate;
            EnPassantColor = board.EnPassantColor;
            EnPassantPosition = board.EnPassantPosition;
            InsufficientMaterial = board.InsufficientMaterial;

            LastMove = new MoveContent(board.LastMove);

            pieces = new ChessPiece[64];
            for (byte x = 0; x < 64; ++x)
            {
                if (board.pieces[x] != null)
                {
                    pieces[x] = new ChessPiece(board.pieces[x]);
                }
            }
        }

        public ChessBoard(int score)
        {
            Score = score;
        }

        internal static int Sort(ChessBoard s2, ChessBoard s1)
        {
            return (s1.Score).CompareTo(s2.Score);
        }
    }
}
