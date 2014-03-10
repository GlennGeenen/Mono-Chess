using System;

namespace SharpChess
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ChessGame game = new ChessGame())
            {
                game.Run();
            }
        }
    }
#endif
}

