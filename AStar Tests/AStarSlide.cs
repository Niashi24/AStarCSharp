using System.Diagnostics.CodeAnalysis;
using NS.AStar;

namespace NS.AStar.Tests;

public class AStarSlide2
{
    public class SlideNodeEqualityComparer : IEqualityComparer<(byte[], int)>
    {
        public bool Equals((byte[], int) x, (byte[], int) y)
        {
            // Check just if the position of the 0's match, may be faster
            if (x.Item2 != y.Item2) return false;
            for (var i = 0; i < x.Item1.Length; i++)
                if (x.Item1[i] != y.Item1[i])
                    return false;
            return true;
        }

            public int GetHashCode([DisallowNull] (byte[], int) obj)
            {
	            var hash = new HashCode();
	            hash.AddBytes(obj.Item1);
	            return hash.ToHashCode();
            }
        }

    public class SlideGraph : IGraph<(byte[], int)>
    {
        public int s { get; private set; }

        public (int, int)[] oneDToTwoDArr { get; set; }

        public (byte[], int) End { get; private set; }

        public int[] endCharArr { get; private set; }

        private int[] indexToHeuristic;

        public SlideGraph(int s, (byte[], int) End)
        {
            this.s = s;
            this.End = End;

            oneDToTwoDArr = Enumerable.Range(0, s * s)
                .Select(i => OneDToTwoD(i, s))
                .ToArray();

            endCharArr = new int[s * s];

            for (var i = 0; i < End.Item1.Length; i++) endCharArr[End.Item1[i]] = i;

            indexToHeuristic = new int[s * s * s * s];
            for (var i = 0; i < s * s; i++)
            for (var j = 0; j < s * s; j++)
            {
                var (x1, y1) = oneDToTwoDArr[i];
                var (x2, y2) = oneDToTwoDArr[j];
                indexToHeuristic[i + j * s * s] = Math.Abs(x2 - x1) + Math.Abs(y2 - y1);
            }
        }

        public (int, int) OneDToTwoD(int i, int s)
        {
            var div = Math.DivRem(i, s, out var mod);
            return (mod, div);
        }

        public (byte[], int) Swap0((byte[], int) state, int i)
        {
            var output = new byte[state.Item1.Length];

            state.Item1.CopyTo(output, 0);

            output[i] = state.Item1[state.Item2];
            output[state.Item2] = state.Item1[i];

            return (output, i);
        }

        public int MoveCost((byte[], int) a, (byte[], int) aNeighbor)
        {
            return 1;
        }

        public IEnumerable<(byte[], int)> GetNeighbors((byte[], int) a)
        {
            var i = a.Item2;
            var (x, y) = oneDToTwoDArr[i];
            if (x != 0)
                yield return Swap0(a, i - 1);
            if (x != s - 1)
                yield return Swap0(a, i + 1);
            if (y != 0)
                yield return Swap0(a, i - s);
            if (y != s - 1)
                yield return Swap0(a, i + s);
        }

        public int HeuristicToEnd((byte[], int) a)
        {
            var heuristic = 0;

            for (var i = 0; i < a.Item1.Length; i++)
            {
                var cI = endCharArr[a.Item1[i]];
                heuristic += indexToHeuristic[i + cI * s * s];
            }

            return heuristic;
        }

        public string StateToString(byte[] state)
        {
            var output = "";

            for (var i = 0; i < state.Length; i++)
            {
                output += state[i];
                if ((i + 1) % s == 0) output += "\n";
            }

            return output;
        }
    }

    public static void Main(string[] args)
    {
        TestSlides(new byte[][]
        {
            new byte[] { 2, 7, 1, 5, 4, 3, 8, 6, 0 },
            new byte[] { 4, 7, 2, 8, 6, 1, 3, 5, 0 },
            new byte[] { 2, 7, 1, 3, 8, 4, 6, 5, 0 },
            new byte[] { 4, 6, 2, 8, 1, 7, 3, 5, 0 },
            new byte[] { 2, 6, 7, 1, 8, 5, 3, 4, 0 },
            new byte[] { 3, 1, 5, 6, 4, 2, 8, 7, 0 },
            new byte[] { 4, 6, 7, 8, 1, 3, 2, 5, 0 },
            new byte[] { 3, 5, 4, 2, 7, 8, 1, 6, 0 }
        }, 3);
        // Average time for 3x3: ~20ms

        TestSlides(new byte[][]
        {
            // new byte[] {12,7,1,10,6,4,5,8,11,3,13,15,14,9,2,0},
            // new byte[] {2,15,1,13,5,10,11,6,7,14,12,3,9,8,4,0},
            new byte[] { 10, 11, 2, 5, 9, 1, 14, 8, 4, 7, 15, 3, 0, 12, 13, 6 }
        }, 4);
        // Average time for 4x4: ~60s
        // Number of possible nodes grows factorialy (very high)
    }

    public static void TestSlides(byte[][] tests, int s)
    {
        if (tests.Length == 0) return;

        (byte[], int) end = default;
        if (s == 4)
            end = (new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 0 }, 15);
        else if (s == 3)
            end = (new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 0 }, 8);
        else
            return;

        (byte[], int)[] testNodes = tests
            .Select(x => (x, Array.IndexOf(x, (byte)0)))
            .ToArray();

        SlideGraph graph = new(s, end);
        SlideNodeEqualityComparer nodeComparer = new();

        System.Diagnostics.Stopwatch stopwatch = new();
        stopwatch.Start();
        var sumMoves = 0;
        for (var i = 0; i < tests.Length; i++)
        {
            var (path, cost) = IDAStar.IDAStarSearch(graph, nodeComparer, testNodes[i]);
            sumMoves += cost;
        }

        stopwatch.Stop();
        Console.WriteLine(
            $"Found path in {s}x{s} tests with avg time of {stopwatch.ElapsedMilliseconds / (double)tests.Length}ms " +
            $"and cost of {sumMoves / (double)tests.Length}");
    }
}