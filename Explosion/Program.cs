using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explosion
{

    /// <summary>
    /// THis program was inspired by version 1.15e of FortressCraft.  The explosions could use a bit of tweaking
    /// so my brain started obsessing about it, so i wrote this code while on vacation. 
    /// </summary>
    class Program
    {



        static void Main(string[] args)
        {
            // TODO: 
            // - make this 3D instead
            // - make the power of the explosion drop down log/exp instead of linear
            // - make it so that CircleNav gets a bool back of "keep going or not". If a shell gets all false, stops running
            // - make it so that circleNav does not need a max radius to do its work.  (by JIT-expanding the lookup table as needed)
            // -note that this is a static thingy, calculate once use forever.

            try
            {
                var pattern1 = new string[] {
                "@@@@@@     ",
                "@@@@@@     ",
                "@@@@@@     ",
                "@@@@@@     ",
                "@@@@@@     ",
                "...........",
                "...........",
                "...........",
                "...........",
                "...........",
            };
                var space = new TwoDSpace<char>();
                space.LoadFromString(pattern1,null,null,x=>x);

                var forceMap = new TwoDSpace<double>();
                forceMap.Set(0, 0, 15); 


                CircleMapper.VisitFromCenter(10, (p) =>
                {
                    // calculate available force
                    if (p.Parents.Count == 0) return;
                    var sum = p.Parents.Sum(x => x.Ratio);
                    var force = p.Parents.Sum(x => (forceMap.Get(x.X, x.Y) * x.Ratio) / sum);

                    var ratio = 0.9;
                    var ch = space.Get(p.X, p.Y);
                    switch (ch)
                    {
                        case '.': ratio = 0.9; break;
                        case '@': ratio = 0.5; break;
                        case ' ': ratio = 0.2; break; 
                    }
                    
                    force = force * ratio;
                    if (force < 1) force = 0; 
                    forceMap.Set(p.X, p.Y, force); 
                }); 

                forceMap.ConsoleWriteLine(Console.Out, (f) => String.Format(" {0:00}",f));

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.Message);
            }
            finally
            {
                Console.WriteLine("Done");
            }
        }
    }
}
