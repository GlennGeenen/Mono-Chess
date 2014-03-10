using System;
using System.Drawing;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Mac
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		ChessGame game;
		
		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			/*
			using (game = new ChessGame())
			{
				game.Run ();
			}
			*/

			game = new ChessGame();
			game.Run();
		}
		
		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}

