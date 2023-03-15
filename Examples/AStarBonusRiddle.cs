using System.Collections;

namespace NS.AStar.Examples;

// This graph is used to solve the bonus riddle found in this video from TED-Ed:
// https://youtu.be/YeMVoJKn1Tg
// Your mission: get from 11 to 25
// Operations available: x2, -3
public class AStarBonusRiddle
{
    public class BonusRiddleGraph : IGraph<int>
    {
        public int Start { get; private set; }

        public int End { get; private set; }
        
        public BonusRiddleGraph(int Start, int End)
        {
            this.Start = Start;
            this.End = End;
        }
        
        public int MoveCost(int a, int aNeighbor)
        {
            return 1;
        }

        public IEnumerable<int> GetNeighbors(int a)
        {
            yield return a * 2;
            yield return a - 3;
        }
        
        public int HeuristicToEnd(int a)
        {
            if (a == End) 
                return 0;
            if (a > End) 
                return Math.Abs((End - a) / 3);
            // if (a < End)
            return (int)Math.Log2(End / Math.Max(a, 1));
        }
    }

    public static void MainRiddle(string[] args)
    {
        BonusRiddleGraph graph = new(11, 26);
        var (moves, cost) = IDAStar.IDAStarSearch(graph, EqualityComparer<int>.Default, graph.Start);
        Console.Write($"{cost} Moves: ");
        foreach (var move in moves)
        {
            Console.Write($"{move} ");
        }
        Console.WriteLine();
    }
}