namespace NS.AStar;

public static class IDAStar
{
    private const int FOUND = -1;
    private const int INF = int.MaxValue;

    public static (List<TNode>, int) IDAStarSearch<TNode>(
        Graph<TNode> graph,
        IEqualityComparer<TNode> nodeComparer,
        TNode start)
    {
        // int bound = graph.Heuristic(start);
        Stack<TNode> path = new();
        var current = start;

        var bound = graph.HeuristicToEnd(current);
        path.Push(start);
        while (true)
        {
            var t = Search(graph, nodeComparer, path, 0, bound);
            if (t == FOUND) return (path.ToList(), bound);
            if (t == INF) throw new Exception("IDAStar could not find a path");
            bound = t;
        }
    }

    private static int Search<TNode>(
        Graph<TNode> graph,
        IEqualityComparer<TNode> nodeComparer,
        Stack<TNode> path,
        int g,
        int bound)
    {
        var curr = path.Peek();
        var f = g + graph.HeuristicToEnd(curr);
        if (f > bound) return f;
        if (nodeComparer.Equals(curr, graph.End)) return FOUND; // FOUND
        var min = INF;
        foreach (var succ in graph.GetNeighbors(curr).OrderBy(graph.HeuristicToEnd))
            if (!path.Contains(succ, nodeComparer))
            {
                path.Push(succ);
                var t = Search(graph, nodeComparer, path, g + graph.MoveCost(curr, succ), bound);
                if (t == FOUND) return FOUND;
                if (t < min) min = t;
                path.Pop();
            }

        return min;
    }
}