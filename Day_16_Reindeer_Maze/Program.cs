using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<List<Tile>> map = new();
int height = inputList.Count;
int width = inputList[0].Length;
Node? startNode = null;
HashSet<Node> endNodes = new();
for (int y = 0; y < height; y++)
{
    List<Tile> row = new();
    map.Add(row);
    for (int x = 0; x < width; x++)
    {
        switch (inputList[y][x])
        {
            case '#': row.Add(Tile.Wall); break;
            default: row.Add(Tile.Space); break;
        }

        if (inputList[y][x] == 'S')
            startNode = Node.GetOrCreateNode(x, y, 1);
        if (inputList[y][x] == 'E')
        {
            endNodes.Add(Node.GetOrCreateNode(x, y, 0));
            endNodes.Add(Node.GetOrCreateNode(x, y, 1));
            endNodes.Add(Node.GetOrCreateNode(x, y, 2));
            endNodes.Add(Node.GetOrCreateNode(x, y, 3));
        }
    }
}

Debug.Assert(startNode is not null);
Debug.Assert(endNodes.Count == 4);

void P12()
{
    startNode.Cost = 0;
    PriorityQueue<Node, Int64> remainingNodes = new();
    remainingNodes.Enqueue(startNode, 0);

    Node? destNode = null;

    while (remainingNodes.Count > 0)
    {
        Node currentNode = remainingNodes.Dequeue();
        if (!currentNode.Visited)
        {
            if (endNodes.Contains(currentNode))
            {
                destNode = currentNode;
                break;
            }

            currentNode.ProcessNeighbours(map);
            foreach ((Node neighbourNode, int cost) in currentNode.Neighbours.Where(t => !t.node.Visited))
            {
                Int64 newCost = currentNode.Cost + cost;
                if (newCost < neighbourNode.Cost)
                {
                    neighbourNode.Cost = newCost;
                    neighbourNode.BestPathPreviousNodes = new List<Node> { currentNode };
                    //List<Node> newPath = new(currentNode.Path);
                    //newPath.Add(currentNode);
                    //neighbourNode.Path = newPath;
                    remainingNodes.Enqueue(neighbourNode, newCost);
                }
                else if (newCost == neighbourNode.Cost)
                    neighbourNode.BestPathPreviousNodes.Add(currentNode);
            }

            currentNode.Visited = true;
        }
    }

    Debug.Assert(destNode is not null);

    Console.WriteLine(destNode.Cost);
    Console.ReadLine();

    HashSet<Node> bestNodes = new();
    Queue<Node> q = new();
    q.Enqueue(destNode);
    while (q.Count > 0)
    {
        Node node = q.Dequeue();
        bestNodes.Add(node);
        node.BestPathPreviousNodes.ForEach(n => q.Enqueue(n));
    }
    var bestTiles = bestNodes.DistinctBy(n => (n.X, n.Y)).ToList();

    Console.WriteLine(bestTiles.Count);
    Console.ReadLine();
}

P12();

public enum Tile
{
    Space = '.',
    Wall = '#',
}

public class Node
{
    public static Dictionary<string, Node> NodeLibrary = new();

    public static Node GetOrCreateNode(int x, int y, int direction)
    {
        Node trialNode = new(x, y, direction);

        if (NodeLibrary.TryGetValue(trialNode._cacheKey, out Node? node))
            return node;
        else
        {
            NodeLibrary[trialNode._cacheKey] = trialNode;
            return trialNode;
        }
    }

    // ##########################################

    public int X;
    public int Y;
    public int Direction; // N0, E1, S2, W3

    private string _cacheKey { get; }

    public List<(Node node, int cost)> Neighbours = new();
    public Int64 Cost = Int64.MaxValue;
    public bool Visited = false;
    //public List<Node> Path = new();
    public List<Node> BestPathPreviousNodes = new();

    public Node(int x, int y, int direction)
    {
        X = x;
        Y = y;
        Direction = direction;

        _cacheKey = $"{x},{y}{direction}";
    }

    public void ProcessNeighbours(List<List<Tile>> map)
    {
        // Rotation neighbours
        int anticlockwise = Direction - 1;
        if (anticlockwise < 0)
            anticlockwise = 3;
        int clockwise = (Direction + 1) % 4;

        Neighbours.Add((Node.GetOrCreateNode(X, Y, anticlockwise), 1000));
        Neighbours.Add((Node.GetOrCreateNode(X, Y, clockwise), 1000));

        // Position neighbours
        (int x, int y) d;
        switch (Direction)
        {
            case 0: d = (0, -1); break;
            case 1: d = (1, 0); break;
            case 2: d = (0, 1); break;
            case 3: d = (-1, 0); break;
            default: throw new NotImplementedException();
        }

        (int x, int y) proposedLocation = (X + d.x, Y + d.y);
        if (map[proposedLocation.y][proposedLocation.x] == Tile.Space)
            Neighbours.Add((Node.GetOrCreateNode(proposedLocation.x, proposedLocation.y, Direction), 1));
    }

    public void Print(List<List<Tile>> map)
    {
        string s = "";
        int height = map.Count;
        int width = map[0].Count;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y == Y && x == X)
                {
                    switch (Direction)
                    {
                        case 0: s += '^'; break;
                        case 1: s += '>'; break;
                        case 2: s += 'v'; break;
                        case 3: s += '<'; break;
                    }
                }
                else
                {
                    s += (char)map[y][x];
                }
            }
            s += Environment.NewLine;
        }
        Console.WriteLine(s);
    }
}
