using System.Collections.Generic;

namespace MonoChess
{
    internal struct MoveArrays
    {
        internal static byte[][] BlackPawnMoves;
        internal static byte[] BlackPawnTotalMoves;

        internal static byte[][] WhitePawnMoves;
        internal static byte[] WhitePawnTotalMoves;

        internal static byte[][] KnightMoves;
        internal static byte[] KnightTotalMoves;

        internal static byte[][] BishopMoves1;
        internal static byte[] BishopTotalMoves1;
        internal static byte[][] BishopMoves2;
        internal static byte[] BishopTotalMoves2;
        internal static byte[][] BishopMoves3;
        internal static byte[] BishopTotalMoves3;
        internal static byte[][] BishopMoves4;
        internal static byte[] BishopTotalMoves4;

        internal static byte[][] RookMoves1;
        internal static byte[] RookTotalMoves1;
        internal static byte[][] RookMoves2;
        internal static byte[] RookTotalMoves2;
        internal static byte[][] RookMoves3;
        internal static byte[] RookTotalMoves3;
        internal static byte[][] RookMoves4;
        internal static byte[] RookTotalMoves4;

        internal static byte[][] KingMoves;
        internal static byte[] KingTotalMoves;
    }

    internal static class PieceMoves
    {
        private static byte Position(byte col, byte row)
        {
            return (byte)(col + (row * 8));
        }

        #region IntitiateMotionMethods

        internal static void InitiateChessPieceMotion()
        {
            MoveArrays.WhitePawnTotalMoves = new byte[64];
            MoveArrays.BlackPawnTotalMoves = new byte[64];

            MoveArrays.KnightTotalMoves = new byte[64];

            MoveArrays.BishopTotalMoves1 = new byte[64];
            MoveArrays.BishopTotalMoves2 = new byte[64];
            MoveArrays.BishopTotalMoves3 = new byte[64];
            MoveArrays.BishopTotalMoves4 = new byte[64];

            MoveArrays.RookTotalMoves1 = new byte[64];
            MoveArrays.RookTotalMoves2 = new byte[64];
            MoveArrays.RookTotalMoves3 = new byte[64];
            MoveArrays.RookTotalMoves4 = new byte[64];

            MoveArrays.KingTotalMoves = new byte[64];
            
            SetMovesWhitePawn();
            SetMovesBlackPawn();
            SetMovesKnight();
            SetMovesBishop();
            SetMovesRook();
            SetMovesKing();
        }

        private static void SetMovesBlackPawn()
        {
            byte[][] arr = new byte[64][];
            for (byte index = 8; index <= 55; ++index)
            {
                var moveset = new List<byte>();
                
                byte x = (byte)(index % 8);
                byte y = (byte)(index / 8);
                
                //Diagonal Kill
                if (y < 7 && x < 7)
                {
                    moveset.Add((byte)(index + 8 + 1));
                    MoveArrays.BlackPawnTotalMoves[index]++;
                }
                if (x > 0 && y < 7)
                {
                    moveset.Add((byte)(index + 8 - 1));
                    MoveArrays.BlackPawnTotalMoves[index]++;
                }
                
                //One Forward
                moveset.Add((byte)(index + 8));
                MoveArrays.BlackPawnTotalMoves[index]++;

                //Starting Position we can jump 2
                if (y == 1)
                {
                    moveset.Add((byte)(index + 16));
                    MoveArrays.BlackPawnTotalMoves[index]++;
                }

                arr[index] = moveset.ToArray();
            }
            MoveArrays.BlackPawnMoves = arr;
        }

        private static void SetMovesWhitePawn()
        {
            byte[][] arr = new byte[64][];
            for (byte index = 8; index <= 55; ++index)
            {
                byte x = (byte)(index % 8);
                byte y = (byte)(index / 8);

                var moveset = new List<byte>();
               
                //Diagonal Kill
                if (x < 7 && y > 0)
                {
                    moveset.Add((byte)(index - 8 + 1));
                    MoveArrays.WhitePawnTotalMoves[index]++;
                }
                if (x > 0 && y > 0)
                {
                    moveset.Add((byte)(index - 8 - 1));
                    MoveArrays.WhitePawnTotalMoves[index]++;
                }

                //One Forward
                moveset.Add((byte)(index - 8));
                MoveArrays.WhitePawnTotalMoves[index]++;

                //Starting Position we can jump 2
                if (y == 6)
                {
                    moveset.Add((byte)(index - 16));
                    MoveArrays.WhitePawnTotalMoves[index]++;
                }

                arr[index] = moveset.ToArray();
            }
            MoveArrays.WhitePawnMoves = arr;
        }

        private static void SetMovesKnight()
        {
            byte[][] arr = new byte[64][];

            for (byte y = 0; y < 8; ++y)
            {
                for (byte x = 0; x < 8; ++x)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveset = new List<byte>();
                    
                    byte move;

                    if (y < 6 && x > 0)
                    {
                        move = Position((byte)(y + 2), (byte)(x - 1));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 1 && x < 7)
                    {
                        move = Position((byte)(y - 2), (byte)(x + 1));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 1 && x > 0)
                    {
                        move = Position((byte)(y - 2), (byte)(x - 1));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y < 6 && x < 7)
                    {
                        move = Position((byte)(y + 2), (byte)(x + 1));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 0 && x < 6)
                    {
                        move = Position((byte)(y - 1), (byte)(x + 2));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y < 7 && x > 1)
                    {
                        move = Position((byte)(y + 1), (byte)(x - 2));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    if (y > 0 && x > 1)
                    {
                        move = Position((byte)(y - 1), (byte)(x - 2));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }
                    
                    if (y < 7 && x < 6)
                    {
                        move = Position((byte)(y + 1), (byte)(x + 2));

                        if (move < 64)
                        {
                            moveset.Add(move);
                            MoveArrays.KnightTotalMoves[index]++;
                        }
                    }

                    
                    arr[index] = moveset.ToArray();
                }
            }

            MoveArrays.KnightMoves = arr;
        }

        private static void SetMovesBishop()
        {

            byte[][] arrOne = new byte[64][];
            byte[][] arrTwo = new byte[64][];
            byte[][] arrThree = new byte[64][];
            byte[][] arrFour = new byte[64][];

            for (byte y = 0; y < 8; ++y)
            {
                for (byte x = 0; x < 8; ++x)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveset = new List<byte>();
                    byte move;

                    byte row = x;
                    byte col = y;

                    while (row < 7 && col < 7)
                    {
                        ++row;
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.BishopTotalMoves1[index]++;
                    }

                    arrOne[index] = moveset.ToArray();

                    moveset = new List<byte>();

                    row = x;
                    col = y;

                    while (row < 7 && col > 0)
                    {
                        ++row;
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.BishopTotalMoves2[index]++;
                    }

                    arrTwo[index] = moveset.ToArray();

                    moveset = new List<byte>();

                    row = x;
                    col = y;

                    while (row > 0 && col < 7)
                    {
                        --row;
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.BishopTotalMoves3[index]++;
                    }

                    arrThree[index] = moveset.ToArray();

                    moveset = new List<byte>();

                    row = x;
                    col = y;

                    while (row > 0 && col > 0)
                    {
                        --row;
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.BishopTotalMoves4[index]++;
                    }

                    arrFour[index] = moveset.ToArray();
                }
            }

            MoveArrays.BishopMoves1 = arrOne;
            MoveArrays.BishopMoves2 = arrTwo;
            MoveArrays.BishopMoves3 = arrThree;
            MoveArrays.BishopMoves4 = arrFour;

        }

        private static void SetMovesRook()
        {
            byte[][] arrOne = new byte[64][];
            byte[][] arrTwo = new byte[64][];
            byte[][] arrThree = new byte[64][];
            byte[][] arrFour = new byte[64][];

            for (byte y = 0; y < 8; ++y)
            {
                for (byte x = 0; x < 8; ++x)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveset = new List<byte>();
                    byte move;

                    byte row = x;
                    byte col = y;

                    while (row < 7)
                    {
                        ++row;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.RookTotalMoves1[index]++;
                    }

                    arrOne[index] = moveset.ToArray();

                    moveset = new List<byte>();
                    row = x;
                    col = y;

                    while (row > 0)
                    {
                        --row;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.RookTotalMoves2[index]++;
                    }

                    arrTwo[index] = moveset.ToArray();

                    moveset = new List<byte>();
                    row = x;
                    col = y;

                    while (col > 0)
                    {
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.RookTotalMoves3[index]++;
                    }

                    arrThree[index] = moveset.ToArray();

                    moveset = new List<byte>();
                    row = x;
                    col = y;

                    while (col < 7)
                    {
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.RookTotalMoves4[index]++;
                    }

                    arrFour[index] = moveset.ToArray();
                }
            }

            MoveArrays.RookMoves1 = arrOne;
            MoveArrays.RookMoves2 = arrTwo;
            MoveArrays.RookMoves3 = arrThree;
            MoveArrays.RookMoves4 = arrFour;
        }

        private static void SetMovesKing()
        {
            byte[][] arr = new byte[64][];

            for (byte y = 0; y < 8; ++y)
            {
                for (byte x = 0; x < 8; ++x)
                {
                    byte index = (byte)(y + (x * 8));

                    var moveset = new List<byte>();
                    byte move;

                    byte row = x;
                    byte col = y;

                    if (row < 7)
                    {
                        ++row;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row > 0)
                    {
                        --row;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (col > 0)
                    {
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (col < 7)
                    {
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row < 7 && col < 7)
                    {
                        ++row;
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row < 7 && col > 0)
                    {
                        ++row;
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row > 0 && col < 7)
                    {
                        --row;
                        ++col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    row = x;
                    col = y;

                    if (row > 0 && col > 0)
                    {
                        --row;
                        --col;

                        move = Position(col, row);
                        moveset.Add(move);
                        MoveArrays.KingTotalMoves[index]++;
                    }

                    arr[index] = moveset.ToArray();
                }
            }
            MoveArrays.KingMoves = arr;
        }

        #endregion
    }
}
