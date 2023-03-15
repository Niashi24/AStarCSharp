namespace NS.AStar;

public interface IGraph<TNode>
{
	// Cost to move from a to the given neighbor of a
	int MoveCost(TNode a, TNode aNeighbor);

	// IEnumerable with the neighbors of the node
	// Using IEnumerable allows us to use the yield return (generator) syntax
	IEnumerable<TNode> GetNeighbors(TNode a);

	// Returns the end node
	TNode End { get; }

	// A heuristic for telling if the given node is "close"
	// to the end node. Must not be negative.
	// Must be admissible (must not overshoot the actual cost to reach the end)
	// to always find solution with lowest cost
	int HeuristicToEnd(TNode a);
}