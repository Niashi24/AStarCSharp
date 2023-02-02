
namespace NS.AStar
{
    public static class IDAStar
    {
        private const int FOUND = -1;
        private const int INF = int.MaxValue;
        
        public static (List<TNode>, int) IDAStarSearch<TNode>(
			Graph<TNode> graph, 
			IEqualityComparer<TNode> nodeComparer, 
			TNode start, TNode end)
        {
            // int bound = graph.Heuristic(start);
            Stack<TNode> path = new();
            TNode current = start;

            int bound = graph.Heuristic(current, end);
            path.Push(start);
            while (true)
            {
                int t = Search(graph, nodeComparer, path, end, 0, bound);
                if (t == FOUND) return (path.ToList(), bound);
                if (t == INF) throw new Exception("IDAStar could not find a path");
                bound = t;
            }
        }

        private static int Search<TNode>(
			Graph<TNode> graph, 
			IEqualityComparer<TNode> nodeComparer,
            Stack<TNode> path,
            TNode end, 
            int g, 
            int bound)
        {
            TNode curr = path.Peek();
            int f = g + graph.Heuristic(curr, end);
            if (f > bound) return f;
            if (nodeComparer.Equals(curr, end)) return FOUND;  // FOUND
            int min = INF;
            foreach (var succ in graph.GetNeighbors(curr).OrderBy(x => graph.Heuristic(x, end)))
            {
                if (!path.Contains(succ, nodeComparer))
                {
                    path.Push(succ);
                    int t = Search(graph, nodeComparer, path, end, g + graph.MoveCost(curr, succ), bound);
                    if (t == FOUND) return FOUND;
                    if (t < min) min = t;
                    path.Pop();
                }
            }
            return min;
        }
    }
}