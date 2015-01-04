using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explosion
{

    class Program
    {
        static void Main(string[] args)
        {
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
