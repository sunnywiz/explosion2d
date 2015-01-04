using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explosion
{
    public class TwoDSpace<T>  
    {
        public int XMin { get; private set; }
        public int XMax { get; private set; }
        public int YMin { get; private set; }
        public int YMax { get; private set; } 

        private Dictionary<int, Dictionary<int, T>> Storage { get; set; }

        public TwoDSpace()
        {
            Storage = new Dictionary<int, Dictionary<int, T>>();
            XMin = 0;
            YMin = 0;
            XMax = 0;
            YMax = 0; 
        }

        public T Get(int x, int y)
        {
            if (x < XMin || x > XMax || y < YMin || y > YMax) return default(T); 
            Dictionary<int, T> row;
            T ch = default(T); 
            if (!Storage.TryGetValue(y, out row)) return ch;
            if (!row.TryGetValue(x, out ch)) return ch;
            return ch;
        }

        public void Set(int x, int y, T ch)
        {
            Dictionary<int, T> row;
            if (!Storage.TryGetValue(y, out row))
            {
                row = new Dictionary<int, T>();
                Storage[y] = row;
                if (y < YMin) YMin = y;
                if (y > YMax) YMax = y;
            }
            row[x] = ch;
            if (x < XMin) XMin = x;
            if (x > XMax) XMax = x;
        }

        public void LoadFromString(string[] pattern, int? cx, int? cy, Func<char, T> convert)
        {
            if (cy == null) cy = (pattern.Length / 2);
            if (cx == null) cx = pattern.Max(x => x.Length) / 2;

            for (int y = 0; y < pattern.Length; y++)
            {
                var row = pattern[y];
                for (int x = 0; x < row.Length; x++)
                {
                    var ch = row[x];
                    Set(x - cx.Value, y - cy.Value, convert(ch));  
                }
            }
        }

       

        public void ConsoleWriteLine(TextWriter o, Func<T,string> action)
        {
            for (int y = YMin; y <= YMax; y++)
            {
                for (int x = XMin; x <= XMax; x++)
                {
                    o.Write(action(Get(x, y)));
                }
                o.WriteLine(); 
            }
        }
    }

    public class CircleMapper
    {
        public class VisitFromCenterParams
        {
            public VisitFromCenterParams()
            {
                Parents = new List<Parent>();
            }

            public class Parent
            {
                public int X { get; set; }
                public int Y { get; set; }
                public int Ratio { get; set; }
            }

            public int X { get; set; }
            public int Y { get; set; }
            public long RadiusSquared { get; set; }

            public List<Parent> Parents { get; set; }
        }

        /// <summary>
        /// Visits points in a space starting from center 0,0 in increasing distance from center
        /// Each visit passes in:  x,y of the point being visited; x,y of a "parent" point that has
        /// already been visited, rsq = radius (Squared) of how far out you are
        /// <param name="visit">function to invoke each visit. </param>
        /// </summary>
        public static void VisitFromCenter(int maxRadius, Action<VisitFromCenterParams> visit)
        {
            long rdsq = (long)maxRadius * maxRadius;
            var DtoXy = new Dictionary<long, List<Tuple<int, int>>>();
            for (int y = -maxRadius; y <= maxRadius; y++)
            {
                long ysq = (long)y * y;
                for (int x = -maxRadius; x < maxRadius; x++)
                {
                    var tuple = new Tuple<int, int>(x, y);
                    var distance = (long)x * x + ysq;
                    if (distance > rdsq) continue;

                    var foo = new List<Tuple<int, int>>();
                    if (!DtoXy.TryGetValue(distance, out foo))
                    {
                        foo = new List<Tuple<int, int>>();
                        DtoXy[distance] = foo;
                    }
                    foo.Add(tuple);
                }
            }
            foreach (var a in DtoXy.OrderBy(x => x.Key))
            {
                foreach (var b in a.Value)
                {
                    var p = new VisitFromCenterParams()
                    {
                        X = b.Item1,
                        Y = b.Item2,
                        RadiusSquared = a.Key
                    };

                    // figure out parents.  9 zones.  
                    if (p.X < 0)
                    {
                        p.Parents.Add(new VisitFromCenterParams.Parent
                        {
                            X = p.X + 1,
                            Y = p.Y,
                            Ratio = -p.X
                        });
                    }
                    if (p.X > 0)
                    {
                        p.Parents.Add(new VisitFromCenterParams.Parent
                        {
                            X = p.X - 1,
                            Y = p.Y,
                            Ratio = p.X
                        });
                    }
                    if (p.Y < 0)
                    {
                        p.Parents.Add(new VisitFromCenterParams.Parent
                        {
                            X = p.X,
                            Y = p.Y + 1,
                            Ratio = -p.Y
                        });
                    }
                    if (p.Y > 0)
                    {
                        p.Parents.Add(new VisitFromCenterParams.Parent
                        {
                            X = p.X,
                            Y = p.Y - 1,
                            Ratio = p.Y
                        });
                    }

                    visit(p);
                }
            }
        }
    }
}
