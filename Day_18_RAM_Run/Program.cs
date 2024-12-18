using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
const int dim = 70;

List<(int x, int y)> fallingBytes = inputList.Select(s => { var split = s.Split(','); return (int.Parse(split[0]), int.Parse(split[1])); }).ToList();

List<List<Node>> map = new();
for (int y = 0; y <= dim; y++)
{
    List<Node> row = new();
    map.Add(row);
    for (int x = 0; x <= dim; x++)
    {
        row.Add(new(x, y));
    }
}

#pragma warning disable 8321
void Print(List<List<Node>> map)
{
    string s = "";

    for (int y = 0; y <= dim; y++)
    {
        for (int x = 0; x <= dim; x++)
        {
            if (map[y][x].Corrupted)
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
    for (int i = 0; i < 1024; i++)
    {
        var fallingByte = fallingBytes[i];
        map[fallingByte.y][fallingByte.x].Corrupted = true;
    }

    Node startNode = map[0][0];
    startNode.Cost = 0;
    PriorityQueue<Node, Int64> remainingNodes = new();
    remainingNodes.Enqueue(startNode, 0);

    Node destNode = map[dim][dim];

    while (remainingNodes.Count > 0)
    {
        Node currentNode = remainingNodes.Dequeue();
        if (!currentNode.Visited)
        {
            if (currentNode == destNode)
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

            currentNode.Visited = true;
        }
    }

    Console.WriteLine(destNode.Cost);
    Console.ReadLine();
}

void P2()
{
    int numBytesToDrop = 1024;
    for (int i = 0; i < numBytesToDrop; i++)
    {
        var fallingByte = fallingBytes[i];
        map[fallingByte.y][fallingByte.x].Corrupted = true;
    }

    Node startNode = map[0][0];
    Node destNode = map[dim][dim];

    bool possible = true;
    for (; possible; numBytesToDrop++)
    {
        possible = false;

        for (int y = 0; y <= dim; y++)
        {
            for (int x = 0; x <= dim; x++)
            {
                map[y][x].Visited = false;
                map[y][x].Cost = Int64.MaxValue;
                map[y][x].Neighbours = new();
            }
        }

        var fallingByte = fallingBytes[numBytesToDrop];
        map[fallingByte.y][fallingByte.x].Corrupted = true;

        startNode.Cost = 0;
        PriorityQueue<Node, Int64> remainingNodes = new();
        remainingNodes.Enqueue(startNode, 0);

        while (remainingNodes.Count > 0)
        {
            Node currentNode = remainingNodes.Dequeue();
            if (!currentNode.Visited)
            {
                currentNode.Visited = true;

                if (currentNode == destNode)
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

        if (destNode.Visited)
            possible = true;
        else
            numBytesToDrop--;
    }

    var blockingFallingByte = fallingBytes[numBytesToDrop];
    Console.WriteLine($"{blockingFallingByte.x},{blockingFallingByte.y}");
    Console.ReadLine();
}

P1();
P2();

public class Node
{
    public int X;
    public int Y;

    public bool Corrupted = false;

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
        if (X > 0 && map[Y][X - 1] is Node n1 && !n1.Corrupted) Neighbours.Add(n1);
        if (X < map[0].Count - 1 && map[Y][X + 1] is Node n2 && !n2.Corrupted) Neighbours.Add(n2);
        if (Y > 0 && map[Y - 1][X] is Node n3 && !n3.Corrupted) Neighbours.Add(n3);
        if (Y < map.Count - 1 && map[Y + 1][X] is Node n4 && !n4.Corrupted) Neighbours.Add(n4);
    }
}
