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
			public int w {get; private set;}
			public int h {get; private set;}

			public SlideGraph(int w, int h)
			{
				this.w = w;
				this.h = h;
			}

			public (int, int) OneDToTwoD(int i, int w, int h)
			{
				return (i % w, i / w);
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
                var (x, y) = OneDToTwoD(i, w, h);
				if (x != 0)
					yield return Swap0(a, i - 1);
				if (x != w - 1)
					yield return Swap0(a, i + 1);
				if (y != 0)
					yield return Swap0(a, i - w);
				if (y != h - 1)
					yield return Swap0(a, i + w);
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
					var (cX, cY) = OneDToTwoD(cI, w, h);
					var (eX, eY) = OneDToTwoD(i, w, h);
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
					if ((i+1) % w == 0) output += "\n";
				}

				return output;
			}
        }

        public static void Main()
		{
			int s = 4;
			SlideGraph graph = new(s, s);
			// (char[], int) start = ("7517BEA24D839FC0".ToCharArray(), s * s - 1);
			(char[], int) end =   ("123456789ABCDEF0".ToCharArray(), s * s - 1);
			string input = "3DBC42A9156F78E0";
			(char[], int) start = (input.ToCharArray(), input.IndexOf('0'));
			// (char[], int) end =   ("123456780".ToCharArray(), s * s - 1);

			var (path, cost) = AStar.AStarSearch(graph, new SlideNodeEqualityComparer(), start, end);

			Console.WriteLine($"Found path using {cost} steps: ");
			Console.WriteLine(graph.StateToString(start.Item1));

			Dictionary<int, char> dirToArrow = new()
			{
				{1, '→'}, {-1, '←'}, {s, '↓'}, {-s, '↑'}
			};

			Console.OutputEncoding = System.Text.Encoding.UTF8;

			for (int i = 1; i < path.Count; i++)
			{
				int dir = path[i].Item2 - path[i-1].Item2;
				Console.Write($"{dirToArrow[dir]}");
			}
			Console.WriteLine();

			Console.WriteLine(graph.StateToString(end.Item1));
			// Main3x3();
			// Main4x4();
		}

		// public static void Main4x4()
		// {
		// 	string[] testInput = new string[]
		// 	{
		// 		"3DBC42A9156F78E0",
		// 		"4C8E2F5617D3AB90",
		// 		"D85A21ECF936B740"
		// 	};

		// 	SlideNode[] tests = testInput.Select(x => new SlideNode(x.ToCharArray())).ToArray();
			
		// 	SlideNode end = new("123456789ABCDEF0".ToCharArray());
		// 	SlideGraph graph = new(4, 4);

		// 	System.Diagnostics.Stopwatch stopwatch = new();
		// 	stopwatch.Start();

		// 	foreach (SlideNode test in tests)
		// 	{
		// 		AStar.AStarSearch(graph, test, end);
		// 	}

		// 	stopwatch.Stop();

		// 	Console.WriteLine($"Solved 4x4 puzzles with an average time of {stopwatch.ElapsedMilliseconds / ((float)tests.Length)}ms");
		// }

		// public static void Main3x3()
		// {
		// 	string[] testInput = new string[]
		// 	{
		// 		"271543860",
		// 		"472861350",
		// 		"271384650",
		// 		"462817350",
		// 		"267185340",
		// 		"315642870",
		// 		"467813250",
		// 		"354278160"
		// 	};

		// 	SlideNode[] tests = testInput.Select(x => new SlideNode(x.ToCharArray())).ToArray();
			
		// 	SlideNode end = new("123456780".ToCharArray());
		// 	SlideGraph graph = new(3, 3);

		// 	System.Diagnostics.Stopwatch stopwatch = new();
		// 	stopwatch.Start();

		// 	foreach (SlideNode test in tests)
		// 	{
		// 		AStar.AStarSearch(graph, test, end);
		// 	}

		// 	stopwatch.Stop();

		// 	Console.WriteLine($"Solved 3x3 puzzles with an average time of {stopwatch.ElapsedMilliseconds / ((float)tests.Length)}ms");
		// }
	}
}