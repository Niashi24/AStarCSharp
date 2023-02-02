namespace NS.AStar
{

	public interface Graph<TNode>
	{
		// Cost to move from a to the given neighbor of a
		int MoveCost(TNode a, TNode aNeighbor);
		// IEnumerable with the neighbors of the node
		IEnumerable<TNode> GetNeighbors(TNode a);
		// Returns the end node
		TNode End {get;}
		// A heuristic for telling if the given node is "close"
		// to the end node. Must not be negative
		int HeuristicToEnd(TNode a);
	}
	
	public static class AStar
	{
		public static (List<TNode>, int) AStarSearch<TNode>(
			Graph<TNode> graph, 
			IEqualityComparer<TNode> nodeComparer, 
			TNode start)
				where TNode : notnull
		{
			Dictionary<TNode, int> G = new(nodeComparer) {{start, 0}};
			Dictionary<TNode, int> F = new(nodeComparer) {
				{start, graph.HeuristicToEnd(start)}};

			HashSet<TNode> closedVertices = new(nodeComparer);
			
			PriorityQueue<TNode, int> openVertPQueue = new();
			openVertPQueue.Enqueue(start, F[start]);
			
			Dictionary<TNode, TNode> cameFrom = new(nodeComparer);

			while (openVertPQueue.Count > 0)
			{
				// Get the vertex in the open list with the lowest F score
				TNode current = openVertPQueue.Dequeue();

				if (closedVertices.Contains(current)) 
					continue;

				// Mark the current node as closed
				closedVertices.Add(current);

				// Check if we have reached the goal
				if (nodeComparer.Equals(current, graph.End))
				{
					// Retrace route backwards
					List<TNode> path = new() {current};
					while (cameFrom.ContainsKey(current))
					{
						current = cameFrom[current];
						path.Add(current);
					}
					path.Reverse();
					// Console.WriteLine($"Processed {F.Count} nodes.");
					return (path, F[graph.End]);  // Done!
				}

				// Update scores for vertices near the current position
				foreach (var neighbor in graph.GetNeighbors(current))
				{
					if (closedVertices.Contains(neighbor))
						continue;  // We have already processed this node   

					if (!G.ContainsKey(neighbor) || G[current] + graph.MoveCost(current, neighbor) < G[neighbor])
					{
						cameFrom[neighbor] = current;
						G[neighbor] = G[current] + graph.MoveCost(current, neighbor);
						F[neighbor] = G[neighbor] + graph.HeuristicToEnd(neighbor);
						openVertPQueue.Enqueue(neighbor, F[neighbor]);
					}
				}
			}

			// throw new Exception($"A* failed to find a solution after searching {F.Count} nodes");
			return (new(), -1);
		}
	}
	
}