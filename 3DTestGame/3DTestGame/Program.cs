using System;

namespace _3DTestGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ISTestGame game = new ISTestGame())
            {
                game.Run();
            }
        }
    }
#endif
}

