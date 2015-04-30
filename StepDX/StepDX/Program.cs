using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace StepDX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           // Application.Run(new Game());
            Game game = new Game();
            game.Show();
            do
            {
                //game.Advance();
                
                if(game.game_over != true)
                {

                    game.Advance();
                 //   break;
                }
                game.Render();
                Application.DoEvents();
            } while (game.Created);
            //System.Threading.Thread.Sleep(2000);
            game.Dispose();

        }
    }
}
