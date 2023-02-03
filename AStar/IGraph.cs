namespace NS.AStar;

public interface IGraph<TNode>
{
	// Cost to move from a to the given neighbor of a
	int MoveCost(TNode a, TNode aNeighbor);

	// IEnumerable with the neighbors of the node
	IEnumerable<TNode> GetNeighbors(TNode a);

	// Returns the end node
	TNode End { get; }

	// A heuristic for telling if the given node is "close"
	// to the end node. Must not be negative
	int HeuristicToEnd(TNode a);
}