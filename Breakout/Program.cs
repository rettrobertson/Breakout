using System;

namespace Breakout
{
    public static class Program 
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Assignment())
            {
                game.Run();
            }
        }
    }
}
