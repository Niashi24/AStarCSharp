using System.Diagnostics.CodeAnalysis;

namespace NS.AStar.Tests
{
    public class AdventOfCode12
    {
        public class HillGraph : Graph<(int, int)>
        {
            public Dictionary<(int, int), char> posToHeightDict {get; private set;}
            public int w {get; private set;}
            public int h {get; private set;}

            public HillGraph(Dictionary<(int, int), char> posToHeightDict, int w, int h)
            {
                this.posToHeightDict = posToHeightDict;
                this.w = w;
                this.h = h;
            }

            public IEnumerable<(int, int)> GetNeighbors((int, int) a)
            {
                var (x, y) = a;
                int height = posToHeightDict[a];

                if (x != 0 && posToHeightDict[(x - 1, y)] <= height + 1)  // left
                    yield return (x - 1, y);
                if (x != w - 1 && posToHeightDict[(x + 1, y)] <= height + 1)
                    yield return (x + 1, y);
                if (y != 0 && posToHeightDict[(x, y - 1)] <= height + 1)
                    yield return (x, y - 1);
                if (y != h - 1 && posToHeightDict[(x, y + 1)] <= height + 1)
                    yield return (x, y + 1);
            }

            public int Heuristic((int, int) a, (int, int) end)
            {
                int square(int x) => x * x;
                int score = 0;

                score += square(a.Item1 - end.Item1);
                score += square(a.Item2 - end.Item2);

                int h1 = posToHeightDict[a];
                int h2 = posToHeightDict[end];
                if (h1 < h2)
                    score = (int)Math.Sqrt(score + square(h2 - h1));
                else
                    score = (int)Math.Sqrt(score) + 1;

                return score;
            }

            public int MoveCost((int, int) a, (int, int) aNeighbor)
            {
                return 1;
            }
        }

        public class IntIntTupleComparer : IEqualityComparer<(int, int)>
        {
            public bool Equals((int, int) x, (int, int) y)
            {
                return (x.Item1 == y.Item1) && (x.Item2 == y.Item2);
            }

            public int GetHashCode([DisallowNull] (int, int) obj)
            {
                return HashCode.Combine(obj.Item1, obj.Item2);
            }
        }

        public static void Main(string[] args)
        {
            Dictionary<(int, int), char> map = new();

            string[] fileText = File.ReadAllLines("Day12e.txt");
            if (fileText.Length == 0) return;

            int h = fileText.Length;
            int w = fileText[0].Length;

            (int, int) s = default, e = default;
            List<(int, int)> aLocations = new();
            
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    char height = fileText[y][x];
                    if (height == 'S')
                    {
                        height = 'a';
                        s = (x, y);
                    }
                    else if (height == 'E')
                    {
                        height = 'z';
                        e = (x, y);
                    }
                    else if (height == 'a')
                    {
                        aLocations.Add((x, y));
                    }

                    map.Add((x, y), height);
                }
            }

            HillGraph graph = new(map, w, h);
            IntIntTupleComparer nodeComparer = new();

            // Part 1:
            var (path, cost) = AStar.AStarSearch(graph, nodeComparer, s, e);

            Console.WriteLine($"Part 1: {path.Count - 1}");

            int minDistance = aLocations
                .Select(x => AStar.AStarSearch(graph, nodeComparer, x, e)) // get path and distance
                .Select(x => x.Item2) // get distance only
                .Where(x => x != -1)
                .Min() - 1; // get smallest and subtract one

            Console.WriteLine($"Part 2: {minDistance}");
        }
    }
}