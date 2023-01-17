using System;

namespace StrangeSuits
{
static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Menu game = new Menu())
            {
                game.Run();
            }
        }
    }
}

