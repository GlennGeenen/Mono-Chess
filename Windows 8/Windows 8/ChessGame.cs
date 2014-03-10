using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoChess;

namespace Windows_8
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ChessGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch spriteBatch;
        private Texture2D dummyTexture;

        //User Interface variables
        private bool blackIsAI = true;
        private byte selectedIndex = 99;

        //Chess variables
        private ChessBoard boardInfo;
        private Texture2D[] textureArray;

        public ChessGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Assets";
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            textureArray = new Texture2D[12];
            boardInfo = new ChessBoard();
            boardInfo.WhoseMove = ChessPieceColor.White;
            boardInfo.pieces = new ChessPiece[]
            {
                new ChessPiece(ChessPieceType.Rook,0,true),
                new ChessPiece(ChessPieceType.Knight,0,true),
                new ChessPiece(ChessPieceType.Bishop,0,true),
                new ChessPiece(ChessPieceType.Queen,0,true),
                new ChessPiece(ChessPieceType.King,0,true),
                new ChessPiece(ChessPieceType.Bishop,1,true),
                new ChessPiece(ChessPieceType.Knight,1,true),
                new ChessPiece(ChessPieceType.Rook,1,true),
                new ChessPiece(ChessPieceType.Pawn,0,true),
                new ChessPiece(ChessPieceType.Pawn,1,true),
                new ChessPiece(ChessPieceType.Pawn,2,true),
                new ChessPiece(ChessPieceType.Pawn,3,true),
                new ChessPiece(ChessPieceType.Pawn,4,true),
                new ChessPiece(ChessPieceType.Pawn,5,true),
                new ChessPiece(ChessPieceType.Pawn,6,true),
                new ChessPiece(ChessPieceType.Pawn,7,true),
                null,null,null,null,null,null,null,null,
                null,null,null,null,null,null,null,null,
                null,null,null,null,null,null,null,null,
                null,null,null,null,null,null,null,null,
                new ChessPiece(ChessPieceType.Pawn,0,false),
                new ChessPiece(ChessPieceType.Pawn,1,false),
                new ChessPiece(ChessPieceType.Pawn,2,false),
                new ChessPiece(ChessPieceType.Pawn,3,false),
                new ChessPiece(ChessPieceType.Pawn,4,false),
                new ChessPiece(ChessPieceType.Pawn,5,false),
                new ChessPiece(ChessPieceType.Pawn,6,false),
                new ChessPiece(ChessPieceType.Pawn,7,false),
                new ChessPiece(ChessPieceType.Rook,0,false),
                new ChessPiece(ChessPieceType.Knight,0,false),
                new ChessPiece(ChessPieceType.Bishop,0,false),
                new ChessPiece(ChessPieceType.Queen,0,false),
                new ChessPiece(ChessPieceType.King,0,false),
                new ChessPiece(ChessPieceType.Bishop,1,false),
                new ChessPiece(ChessPieceType.Knight,1,false),
                new ChessPiece(ChessPieceType.Rook,1,false),
            };

            PieceMoves.InitiateChessPieceMotion();
            PieceValidMoves.GenerateValidMoves(boardInfo);
            Evaluation.EvaluateBoardScore(boardInfo);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            spriteBatch = new SpriteBatch(GraphicsDevice);
            dummyTexture = new Texture2D(GraphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.White });
            LoadTextures();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (boardInfo.WhoseMove == ChessPieceColor.Black && blackIsAI)
            {
                ChessEngine.EngineMove(boardInfo);
            }
            else
            {
                MouseState mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    int xPos = (int)(mouseState.X / Constants.SquareSize);
                    int yPos = (int)(mouseState.Y / Constants.SquareSize);

                    if (xPos < Constants.NumberOfFiles && yPos < Constants.NumberOfRanks)
                    {
                        if (selectedIndex > boardInfo.pieces.Length)
                        {
                            byte index = (byte)(yPos * Constants.NumberOfFiles + xPos);
                            if (boardInfo.pieces[index] != null && boardInfo.pieces[index].PieceColor == boardInfo.WhoseMove)
                            {
                                selectedIndex = index;
                                boardInfo.pieces[index].isSelected = true;
                            }
                        }
                        else
                        {
                            byte index = (byte)(yPos * Constants.NumberOfFiles + xPos);
                            if (index != selectedIndex)
                            {
                                //SourceIndex has no piece
                                if (boardInfo.pieces[selectedIndex] == null)
                                {
                                    return;
                                }

                                //Select new piece if same color
                                if (boardInfo.pieces[index] != null && boardInfo.pieces[index].PieceColor == boardInfo.WhoseMove)
                                {
                                    boardInfo.pieces[selectedIndex].isSelected = false;
                                    boardInfo.pieces[index].isSelected = true;
                                    selectedIndex = index;
                                }

                                //Check if this is infact a valid move
                                if (!ChessEngine.IsValidMove(boardInfo, selectedIndex, index))
                                {
                                    return;
                                }

                                ChessEngine.MovePiece(boardInfo, selectedIndex, index);
                                selectedIndex = 99;
                            }
                            else
                            {
                                boardInfo.pieces[selectedIndex].isSelected = false;
                                selectedIndex = 99;
                            }
                        }
                    }
                    else if (selectedIndex < boardInfo.pieces.Length)
                    {
                        boardInfo.pieces[selectedIndex].isSelected = false;
                        selectedIndex = 99;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw Squares and Pieces
            Color squareColor = Color.White;

            for (byte i = 0; i < boardInfo.pieces.Length; ++i)
            {
                if ((int)(i / Constants.NumberOfRanks) % 2 == 0)
                {
                    if (i % 2 == 0)
                    {
                        squareColor = Color.White;
                    }
                    else
                    {
                        squareColor = Color.Gray;
                    }
                }
                else
                {
                    if (i % 2 == 0)
                    {
                        squareColor = Color.Gray;
                    }
                    else
                    {
                        squareColor = Color.White;
                    }
                }

                ChessPiece piece = boardInfo.pieces[i];

                if (piece != null)
                {
                    if (piece.isSelected)
                    {
                        squareColor = Color.Blue;
                    }

                    spriteBatch.Draw(dummyTexture, new Rectangle((i % Constants.NumberOfFiles) * Constants.SquareSize, (int)(i / Constants.NumberOfRanks) * Constants.SquareSize, Constants.SquareSize, Constants.SquareSize), squareColor);

                    Texture2D textureToDraw;
                    if (piece.PieceColor == ChessPieceColor.White)
                    {
                        textureToDraw = textureArray[(int)piece.Identifier];
                    }
                    else
                    {
                        textureToDraw = textureArray[(int)ChessPieceType.None + (int)piece.Identifier];
                    }

                    spriteBatch.Draw(textureToDraw, new Rectangle((i % Constants.NumberOfFiles) * Constants.SquareSize, (int)(i / Constants.NumberOfRanks) * Constants.SquareSize, Constants.SquareSize, Constants.SquareSize), Color.White);

                }
                else
                {
                    spriteBatch.Draw(dummyTexture, new Rectangle((i % Constants.NumberOfFiles) * Constants.SquareSize, (int)(i / Constants.NumberOfRanks) * Constants.SquareSize, Constants.SquareSize, Constants.SquareSize), squareColor);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void LoadTextures()
        {
            textureArray[5] = Content.Load<Texture2D>("Pawn_White");
            textureArray[4] = Content.Load<Texture2D>("Knight_White");
            textureArray[3] = Content.Load<Texture2D>("Bishop_White");
            textureArray[2] = Content.Load<Texture2D>("Rook_White");
            textureArray[1] = Content.Load<Texture2D>("Queen_White");
            textureArray[0] = Content.Load<Texture2D>("King_White");

            textureArray[11] = Content.Load<Texture2D>("Pawn_Black");
            textureArray[10] = Content.Load<Texture2D>("Knight_Black");
            textureArray[9] = Content.Load<Texture2D>("Bishop_Black");
            textureArray[8] = Content.Load<Texture2D>("Rook_Black");
            textureArray[7] = Content.Load<Texture2D>("Queen_Black");
            textureArray[6] = Content.Load<Texture2D>("King_Black");
        }
    }
}
