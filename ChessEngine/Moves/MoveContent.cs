namespace MonoChess
{
    internal struct PieceMoving
    {
        internal byte DstPosition;
        internal bool Moved;
        internal ChessPieceColor PieceColor;
        internal ChessPieceType PieceType;
        internal byte SrcPosition;

        public PieceMoving(ChessPieceColor pieceColor, ChessPieceType pieceType, bool moved,
                           byte srcPosition, byte dstPosition)
        {
            PieceColor = pieceColor;
            PieceType = pieceType;
            SrcPosition = srcPosition;
            DstPosition = dstPosition;
            Moved = moved;
        }

        public PieceMoving(PieceMoving pieceMoving)
        {
            PieceColor = pieceMoving.PieceColor;
            PieceType = pieceMoving.PieceType;
            SrcPosition = pieceMoving.SrcPosition;
            DstPosition = pieceMoving.DstPosition;
            Moved = pieceMoving.Moved;
        }

        public PieceMoving(ChessPieceType pieceType)
        {
            PieceType = pieceType;
            PieceColor = ChessPieceColor.White;
            SrcPosition = 0;
            DstPosition = 0;
            Moved = false;
        }
    }

    internal struct PieceTaken
    {
        internal bool Moved;
        internal ChessPieceColor PieceColor;
        internal ChessPieceType PieceType;
        internal byte Position;

        internal PieceTaken(ChessPieceColor pieceColor, ChessPieceType pieceType, bool moved,
                          byte position)
        {
            PieceColor = pieceColor;
            PieceType = pieceType;
            Position = position;
            Moved = moved;
        }

        internal PieceTaken(ChessPieceType pieceType)
        {
            PieceColor = ChessPieceColor.White;
            PieceType = pieceType;
            Position = 0;
            Moved = false;
        }
    }

    internal class MoveContent
    {
        internal bool EnPassantOccured;
        internal PieceMoving MovingPiecePrimary;
        internal PieceMoving MovingPieceSecondary;
        internal bool PawnPromoted;
        internal PieceTaken TakenPiece;

        public MoveContent()
        {
            MovingPiecePrimary = new PieceMoving(ChessPieceType.None);
            MovingPieceSecondary = new PieceMoving(ChessPieceType.None);
            TakenPiece = new PieceTaken(ChessPieceType.None);
        }

        public MoveContent(MoveContent moveContent)
        {
            MovingPiecePrimary = new PieceMoving(moveContent.MovingPiecePrimary);
            MovingPieceSecondary = new PieceMoving(moveContent.MovingPieceSecondary);

            TakenPiece = new PieceTaken(moveContent.TakenPiece.PieceColor,
                                        moveContent.TakenPiece.PieceType,
                                        moveContent.TakenPiece.Moved,
                                        moveContent.TakenPiece.Position);

            EnPassantOccured = moveContent.EnPassantOccured;
            PawnPromoted = moveContent.PawnPromoted;
        }
    }
}
