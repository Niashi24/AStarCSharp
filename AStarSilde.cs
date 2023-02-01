using System.Diagnostics.CodeAnalysis;
using NS.AStar;

namespace NS.AStar.Tests
{
	public class AStarSlide2
	{
        public class SlideNodeEqualityComparer : IEqualityComparer<(char[], int)>
        {
            public bool Equals((char[], int) x, (char[], int) y)
            {
				// Check just if the position of the 0's match, may be faster
				if (x.Item2 != y.Item2) return false;
                for (int i = 0; i < x.Item1.Length; i++)
					if (x.Item1[i] != y.Item1[i]) return false;
				return true;
            }

            public int GetHashCode([DisallowNull] (char[], int) obj)
            {
                return 2 * ((System.Collections.IStructuralEquatable)obj.Item1)
					.GetHashCode(EqualityComparer<char>.Default);
            }
        }

		public class SlideGraph : Graph<(char[], int)>
		{
			public int s {get; private set;}

			public SlideGraph(int s)
			{
				this.s = s;
			}

			public (int, int) OneDToTwoD(int i, int s)
			{
				return (i % s, i / s);
			}

			public (char[], int) Swap0((char[], int) state, int i)
			{
				var output = new char[state.Item1.Length];
				state.Item1.CopyTo(output, 0);

				char temp = output[i];
				output[i] = output[state.Item2];
				output[state.Item2] = temp;

				return (output, i);
			}

            public int MoveCost((char[], int) a, (char[], int) aNeighbor)
            {
                return 1;
            }

            public IEnumerable<(char[], int)> GetNeighbors((char[], int) a)
            {
				int i = a.Item2;
                var (x, y) = OneDToTwoD(i, s);
				if (x != 0)
					yield return Swap0(a, i - 1);
				if (x != s - 1)
					yield return Swap0(a, i + 1);
				if (y != 0)
					yield return Swap0(a, i - s);
				if (y != s - 1)
					yield return Swap0(a, i + s);
            }

			public int GetCharPos(char[] a, char c)
			{
				for (int i = 0; i < a.Length; i++)
					if (a[i] == c) return i;
				return -1;
			}

            public int Heuristic((char[], int) a, (char[], int) end)
            {
                int heuristic = 0;

				for (int i = 0; i < a.Item1.Length; i++)
				{
					int cI = GetCharPos(a.Item1, end.Item1[i]);
					var (cX, cY) = OneDToTwoD(cI, s);
					var (eX, eY) = OneDToTwoD(i, s);
					heuristic += Math.Abs(eX - cX) + Math.Abs(eY - cY);
				}

				return heuristic;
            }

			public string StateToString(char[] state)
			{
				string output = "";

				for (int i = 0; i < state.Length; i++)
				{
					output += state[i];
					if ((i+1) % s == 0) output += "\n";
				}

				return output;
			}
        }

        public static void Main()
		{
			TestSlides(new string[]
			{
				"271543860",
				"472861350",
				"271384650",
				"462817350",
				"267185340",
				"315642870",
				"467813250",
				"354278160"
			}, 3);

			TestSlides(new string[]
			{
				"3DBC42A9156F78E0",
				"4C8E2F5617D3AB90",
				"D85A21ECF936B740"
			}, 4);
		}

		public static void TestSlides(string[] tests, int s)
		{
			if (tests.Length == 0) return;

			(char[], int) end = default;
			if (s == 4)
				end = ("123456789ABCDEF0".ToCharArray(), 15);
			else if (s == 3)
				end = ("123456780".ToCharArray(), 8);
			else
				return;

			(char[], int)[] testNodes = tests
				.Select(x => (x.ToCharArray(), x.IndexOf('0')))
				.ToArray();

			SlideGraph graph = new(s);
			SlideNodeEqualityComparer nodeComparer = new();

            System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Start();
			int sumMoves = 0;
			for (int i = 0; i < tests.Length; i++)
			{
				var (path, cost) = IDAStar.IDAStarSearch(graph, nodeComparer, testNodes[i], end);
				sumMoves += cost;
			}
			stopwatch.Stop();
			Console.WriteLine($"Found path in {s}x{s} tests with an average time of {stopwatch.ElapsedMilliseconds/(double)tests.Length}ms and cost of {sumMoves/(double)tests.Length}");
		}
	}
}