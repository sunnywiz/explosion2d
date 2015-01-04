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
                "...... ....",
                "..@@..  ...",
                "..@@.. ....",
                "...... ....",
                ".....@ ....",
                "....@@ @...",
                "...... ....",
                "..@@.......",
                "...........",
                "...........",
            };
                var space = new TwoDSpace<char>();
                space.LoadFromString(pattern1,null,null,x=>x);

                var forceMap = new TwoDSpace<double>();
                forceMap.Set(0, 0, 6); 


                CircleMapper.VisitFromCenter(6, (p) =>
                {
                    // calculate available force
                    if (p.Parents.Count == 0) return;
                    var sum = p.Parents.Sum(x => x.Ratio);
                    var force = p.Parents.Sum(x => (forceMap.Get(x.X, x.Y) * x.Ratio) / sum);
                    force = force - 1;
                    if (force < 0) force = 0; 
                    forceMap.Set(p.X, p.Y, force); 
                }); 

                forceMap.ConsoleWriteLine(Console.Out, (c) => c.ToString("c"));

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
