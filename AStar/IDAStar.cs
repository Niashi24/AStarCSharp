namespace NS.AStar;


public static class IDAStar
{
    private const int FOUND = -1;
    private const int INF = int.MaxValue;
    
    public static (List<TNode>, int) IDAStarSearch<TNode>(
		IGraph<TNode> graph, 
		IEqualityComparer<TNode> nodeComparer, 
		TNode start)
    {
        // int bound = graph.Heuristic(start);
        Stack<TNode> path = new();
        HashSet<TNode> pathSet = new(nodeComparer);
        TNode current = start;

        int bound = graph.HeuristicToEnd(current);
        path.Push(start);
        while (true)
        {
            int t = Search(graph, nodeComparer, path, pathSet, 0, bound);
            if (t == FOUND) return (path.ToList(), bound);
            if (t == INF) throw new Exception("IDAStar could not find a path");
            bound = t;
        }
    }

    private static int Search<TNode>(
		IGraph<TNode> graph, 
		IEqualityComparer<TNode> nodeComparer,
        Stack<TNode> path,
        HashSet<TNode> pathSet,
        int g, 
        int bound)
    {
        TNode curr = path.Peek();
        int f = g + graph.HeuristicToEnd(curr);
        if (f > bound) return f;
        if (nodeComparer.Equals(curr, graph.End)) return FOUND;  // FOUND
        int min = INF;
        foreach (var succ in graph.GetNeighbors(curr).OrderBy(graph.HeuristicToEnd))
        {
            if (!pathSet.Contains(succ))
            {
                path.Push(succ);
                pathSet.Add(succ);
                int t = Search(graph, nodeComparer, path, pathSet,g + graph.MoveCost(curr, succ), bound);
                if (t == FOUND) return FOUND;
                if (t < min) min = t;
                pathSet.Remove(succ);
                path.Pop();
            }
        }
        return min;
    }
}
