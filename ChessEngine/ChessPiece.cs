using System.Collections.Generic;

namespace MonoChess
{
    public class ChessPiece
    {
        #region Properties

        public ChessPieceType Identifier;
        public ChessPieceColor PieceColor;

        public Stack<byte> ValidMoves;

        public short PieceValue;
        public short PieceActionValue;
        public short AttackedValue;
        public short DefendedValue;

        public int LastValidMoveCount;
        public bool Moved;

        #endregion

        #region Public Methods

        public ChessPiece(ChessPieceType id, byte number, bool isBlack)
        {
            this.Identifier = id;
            if (isBlack) this.PieceColor = ChessPieceColor.Black;
            else this.PieceColor = ChessPieceColor.White;
            this.Moved = false;
            this.LastValidMoveCount = 0;
            this.ValidMoves = new Stack<byte>();

            PieceValue = CalculatePieceValue(id);
            PieceActionValue = CalculatePieceActionValue(id);
        }

        public ChessPiece(ChessPiece piece)
        {
            Identifier = piece.Identifier;
            PieceColor = piece.PieceColor;
            PieceValue = piece.PieceValue;
            PieceActionValue = piece.PieceActionValue;
            Moved = piece.Moved;

            if (piece.ValidMoves != null)
                LastValidMoveCount = piece.ValidMoves.Count;
        }

        #endregion

        #region PrivateMethods

        private static short CalculatePieceValue(ChessPieceType pieceType)
        {
            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    {
                        return 100;

                    }
                case ChessPieceType.Knight:
                    {
                        return 320;
                    }
                case ChessPieceType.Bishop:
                    {
                        return 333;
                    }
                case ChessPieceType.Rook:
                    {
                        return 510;
                    }

                case ChessPieceType.Queen:
                    {
                        return 880;
                    }

                case ChessPieceType.King:
                    {
                        return 32767;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }


        private static short CalculatePieceActionValue(ChessPieceType pieceType)
        {
            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    {
                        return 6;

                    }
                case ChessPieceType.Knight:
                    {
                        return 3;
                    }
                case ChessPieceType.Bishop:
                    {
                        return 3;
                    }
                case ChessPieceType.Rook:
                    {
                        return 2;
                    }

                case ChessPieceType.Queen:
                    {
                        return 1;
                    }

                case ChessPieceType.King:
                    {
                        return 1;
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        #endregion
    }
}
