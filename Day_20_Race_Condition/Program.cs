using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<List<Node>> map = new();
int width = inputList[0].Length;
int height = inputList.Count;
Node? startNode = null;
Node? endNode = null;
for (int y = 0; y < height; y++)
{
    List<Node> row = new();
    map.Add(row);
    for (int x = 0; x < width; x++)
    {
        Node newNode = new(x, y);
        if (inputList[y][x] == '#')
            newNode.Wall = true;
        else
        {
            if (inputList[y][x] == 'E')
                endNode = newNode;
            else if (inputList[y][x] == 'S')
                startNode = newNode;
        }
        row.Add(newNode);
    }
}
Debug.Assert(startNode is not null);
Debug.Assert(endNode is not null);

Int64 normalFastest = Int64.MaxValue;

startNode.Cost = 0;
PriorityQueue<Node, Int64> remainingNodes = new();
remainingNodes.Enqueue(startNode, 0);

while (remainingNodes.Count > 0)
{
    Node currentNode = remainingNodes.Dequeue();
    if (!currentNode.Visited)
    {
        currentNode.Visited = true;

        if (currentNode == endNode)
            break;

        currentNode.ProcessNeighbours(map);
        foreach (Node neighbourNode in currentNode.Neighbours.Where(node => !node.Visited))
        {
            Int64 newCost = currentNode.Cost + 1;
            if (newCost < neighbourNode.Cost)
            {
                neighbourNode.Cost = newCost;
                remainingNodes.Enqueue(neighbourNode, newCost);
            }
        }
    }
}

normalFastest = endNode.Cost;

#pragma warning disable 8321
void Print(List<List<Node>> map)
{
    string s = "";

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (map[y][x].Wall)
                s += '#';
            else
                s += '.';
        }
        s += Environment.NewLine;
    }
    Console.WriteLine(s);
}
#pragma warning restore 8321

void P1()
{
    Int64 result = 0;

    // Don't need to test or reset the outside edge given the walls
    for (int y1 = 1; y1 < height - 1; y1++)
    {
        for (int x1 = 1; x1 < width - 1; x1++)
        {
            for (int y2 = y1; y2 <= y1 + 2 && y2 < height - 1; y2++)
            {
                for (int x2 = x1; x2 <= x1 + 2 && x2 < width - 1; x2++)
                {
                    if (!map[y1][x1].Wall && !map[y2][x2].Wall)
                    {
                        // Looking for 2 cells with manhatten distance = 2 where the cost difference is >= 100
                        int manhattenDistance = Math.Abs(y2 - y1) + Math.Abs(x2 - x1);
                        if (manhattenDistance == 2)
                        {
                            Int64 costDiff = Math.Abs(map[y1][x1].Cost - map[y2][x2].Cost);
                            if (costDiff - manhattenDistance >= 100)
                            {
                                //Console.WriteLine($"{x1},{y1}");
                                result++;
                            }
                        }
                    }
                }
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}


void P2()
{
    Int64 result = 0;

    const int cheatLength = 20;

    // This solution worked, but I don't understand why, because the code doesn't check
    // that all the cells on the way were a wall, so this might hop over a genuine path

    // Don't need to test or reset the outside edge given the walls
    for (int y1 = 1; y1 < height - 1; y1++)
    {
        for (int x1 = 1; x1 < width - 1; x1++)
        {
            for (int y2 = Math.Max(1, y1 - cheatLength); y2 <= y1 + cheatLength && y2 < height - 1; y2++)
            {
                for (int x2 = Math.Max(1, x1 - cheatLength); x2 <= x1 + cheatLength && x2 < width - 1; x2++)
                {
                    if (!map[y1][x1].Wall && !map[y2][x2].Wall)
                    {
                        // Looking for 2 cells with manhatten distance is between inclusive 2 & cheatLength where the cost difference is >= 100
                        int manhattenDistance = Math.Abs(y2 - y1) + Math.Abs(x2 - x1);
                        if (manhattenDistance >= 2 && manhattenDistance <= cheatLength)
                        {
                            Int64 costDiff = Math.Abs(map[y1][x1].Cost - map[y2][x2].Cost);
                            if (costDiff - manhattenDistance >= 100)
                            {
                                //Console.WriteLine($"({x1},{y1}),({x2},{y2})");
                                result++;
                            }
                        }
                    }
                }
            }
        }
    }

    // Divide by 2 to eliminate the duplicate cheats found by the x - and +
    Console.WriteLine(result / 2);
    Console.ReadLine();

    /*
    // Never got this to work, but this solution makes more sense to me than my working one...
    result = 0;
    HashSet<((int, int), (int, int))> foundCheats = new();
    // Don't need to test or reset the outside edge given the walls
    for (int y1 = 1; y1 < height - 1; y1++)
    {
        for (int x1 = 1; x1 < width - 1; x1++)
        {
            if (!map[y1][x1].Wall)
            {
                // Explore cheatLength away from x1,y1 in E and S direction

                Queue<Node> wallNodesWithinCheatLength = new();
                wallNodesWithinCheatLength.Enqueue(map[y1][x1]);
                HashSet<Node> wallNodesWithinCheatLengthHashSet = new();
                Dictionary<Node,int> nonWallNodesWithinCheatLength = new() { };
                int length = 0;

                while (wallNodesWithinCheatLength.Count > 0)
                {
                    Queue<Node> nextWallNodesWithinCheatLength = new();
                    while (length < cheatLength && wallNodesWithinCheatLength.Count > 0)
                    {
                        Node currentNode = wallNodesWithinCheatLength.Dequeue();

                        foreach (var d in new List<(int x, int y)> { (0, -1), (1, 0), (0, 1), (-1, 0) })
                        {
                            (int x, int y) potentialCoord = (currentNode.X + d.x, currentNode.Y + d.y);

                            if (potentialCoord.y >= 0 && potentialCoord.y < height &&
                                potentialCoord.x >= 0 && potentialCoord.x < width)
                            {
                                Node potentialNode = map[potentialCoord.y][potentialCoord.x];

                                if (potentialNode.Wall)
                                {
                                    if (wallNodesWithinCheatLengthHashSet.Add(potentialNode))
                                        nextWallNodesWithinCheatLength.Enqueue(potentialNode);
                                }
                                else if (length > 0)
                                {
                                    if (!nonWallNodesWithinCheatLength.TryGetValue(potentialNode, out int existingLength) || existingLength > length)
                                        nonWallNodesWithinCheatLength[potentialNode] = length;
                                }
                            }
                        }
                    }
                    length++;
                    wallNodesWithinCheatLength = nextWallNodesWithinCheatLength;
                }
                nonWallNodesWithinCheatLength.Remove(map[y1][x1]);

                foreach (var kvp in nonWallNodesWithinCheatLength)
                {
                    var nonWallNodeWithinCheatLength = kvp.Key;

                    // Looking for 2 cells with manhatten distance is between inclusive 2 & cheatLength where the cost difference is >= 100
                    int manhattenDistance = Math.Abs(nonWallNodeWithinCheatLength.Y - y1) + Math.Abs(nonWallNodeWithinCheatLength.X - x1);
                    if (manhattenDistance >= 2 && manhattenDistance <= cheatLength)
                    {
                        Int64 costDiff = Math.Abs(map[y1][x1].Cost - map[nonWallNodeWithinCheatLength.Y][nonWallNodeWithinCheatLength.X].Cost);
                        if (costDiff - manhattenDistance >= 100)
                        {
                            //if (foundCheats)
                            //Console.WriteLine($"({x1},{y1}),({nonWallNodeWithinCheatLength.X},{nonWallNodeWithinCheatLength.Y})");
                            result++;
                        }
                    }
                }
            }
        }
    }

    Console.WriteLine(result / 2);
    Console.ReadLine();
    */
}

P1();
P2();

public class Node
{
    public int X;
    public int Y;

    public bool Wall = false;

    public List<Node> Neighbours = new();
    public Int64 Cost = Int64.MaxValue;
    public bool Visited = false;

    public Node(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void ProcessNeighbours(List<List<Node>> map)
    {
        if (X > 0 && map[Y][X - 1] is Node n1 && !n1.Wall) Neighbours.Add(n1);
        if (X < map[0].Count - 1 && map[Y][X + 1] is Node n2 && !n2.Wall) Neighbours.Add(n2);
        if (Y > 0 && map[Y - 1][X] is Node n3 && !n3.Wall) Neighbours.Add(n3);
        if (Y < map.Count - 1 && map[Y + 1][X] is Node n4 && !n4.Wall) Neighbours.Add(n4);
    }

    public override string ToString()
    {
        return $"{X},{Y}";
    }
}
