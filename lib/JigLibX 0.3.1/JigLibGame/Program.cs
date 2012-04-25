using System;

namespace JiggleGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (JiggleGame game = new JiggleGame())
            {
                game.Run();
            }
        }
    }
}

