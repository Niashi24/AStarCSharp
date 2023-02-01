using NS.AStar;

namespace NS.AStar.Tests
{
	public class AStarSlide
	{
		public class SlideNode : Node<SlideNode>
		{
			public char[] state {get; private set;}

			public SlideNode(char[] state)
			{
				this.state = state;
			}

			public override bool EqualsNode(SlideNode other)
			{
				for (int i = 0; i < state.Length; i++)
					if (state[i] != other.state[i]) return false;
				return true;
			}

			public override int GetHashCode()
			{
				return ((System.Collections.IStructuralEquatable)this.state).GetHashCode(EqualityComparer<char>.Default);
			}

			public int GetCharPos(char c)
			{
				for (int i = 0; i < state.Length; i++)
					if (state[i] == c) return i;
				return -1;
			}

			public char[] Swap(int i, int j)
			{
				var output = (char[])state.Clone();

				char temp = output[i];
				output[i] = output[j];
				output[j] = temp;

				return output;
			}

			public string ToString(int w, int h)
			{
				string output = "";

				for (int i = 0; i < state.Length; i++)
				{
					output += state[i];
					if ((i+1) % w == 0) output += "\n";
				}

				return output;
			}

			public override string ToString()
			{
				return string.Join("", state);
			}
		}

		public class SlideGraph : Graph<SlideNode>
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

			public IEnumerable<SlideNode> GetNeighbors(SlideNode a)
			{
				int i = a.GetCharPos('0');

				var (x, y) = OneDToTwoD(i, w, h);
				if (x != 0)
					yield return new(a.Swap(i, i - 1));
				if (x != w - 1)
					yield return new(a.Swap(i, i + 1));
				if (y != 0)
					yield return new(a.Swap(i, i - w));
				if (y != h - 1)
					yield return new(a.Swap(i, i + w));
			}

			public int Heuristic(SlideNode a, SlideNode end)
			{
				int heuristic = 0;

				for (int i = 0; i < a.state.Length; i++)
				{
					int cI = a.GetCharPos(end.state[i]);
					var (cX, cY) = OneDToTwoD(cI, w, h);
					var (eX, eY) = OneDToTwoD(i, w, h);
					heuristic += Math.Abs(eX - cX) + Math.Abs(eY - cY);
				}

				return heuristic;
			}

			public int MoveCost(SlideNode a, SlideNode aNeighbor)
			{
				return 1;
			}
		}

		public static void Main1()
		{
			Main3x3();
			Main4x4();
		}

		public static void Main4x4()
		{
			string[] testInput = new string[]
			{
				"3DBC42A9156F78E0",
				"4C8E2F5617D3AB90",
				"D85A21ECF936B740"
			};

			SlideNode[] tests = testInput.Select(x => new SlideNode(x.ToCharArray())).ToArray();
			
			SlideNode end = new("123456789ABCDEF0".ToCharArray());
			SlideGraph graph = new(4, 4);

			System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Start();

			foreach (SlideNode test in tests)
			{
				AStar.AStarSearch(graph, test, end);
			}

			stopwatch.Stop();

			Console.WriteLine($"Solved 4x4 puzzles with an average time of {stopwatch.ElapsedMilliseconds / ((float)tests.Length)}ms");
		}

		public static void Main3x3()
		{
			string[] testInput = new string[]
			{
				"271543860",
				"472861350",
				"271384650",
				"462817350",
				"267185340",
				"315642870",
				"467813250",
				"354278160"
			};

			SlideNode[] tests = testInput.Select(x => new SlideNode(x.ToCharArray())).ToArray();
			
			SlideNode end = new("123456780".ToCharArray());
			SlideGraph graph = new(3, 3);

			System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Start();

			foreach (SlideNode test in tests)
			{
				AStar.AStarSearch(graph, test, end);
			}

			stopwatch.Stop();

			Console.WriteLine($"Solved 3x3 puzzles with an average time of {stopwatch.ElapsedMilliseconds / ((float)tests.Length)}ms");
		}
	}
}