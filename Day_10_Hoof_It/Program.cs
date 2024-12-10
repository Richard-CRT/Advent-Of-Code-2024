using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<List<int>> map = new();
int height = inputList.Count;
int width = inputList[0].Count();
List<(int, int)> trailHeads = new();
for (int y = 0; y < height; y++)
{
    List<int> row = new();
    map.Add(row);
    for (int x = 0; x < width; x++)
    {
        int locationHeight = int.Parse(inputList[y][x].ToString());
        row.Add(locationHeight);
        if (locationHeight == 0)
            trailHeads.Add((x, y));
    }
}

void recurse(List<HashSet<(int, int)>> paths, HashSet<(int, int)> path, (int x, int y) newLocation)
{
    if (map[newLocation.y][newLocation.x] == 9)
    {
        path.Add(newLocation);
        paths.Add(path);
    }
    else
    {
        List<(int x, int y)> possibleSteps = new List<(int, int)> { (-1, 0), (0, -1), (1, 0), (0, 1) };
        foreach (var d in possibleSteps)
        {
            (int x, int y) targetLocation = (newLocation.x + d.x, newLocation.y + d.y);

            if (targetLocation.x >= 0 && targetLocation.x < width && targetLocation.y >= 0 && targetLocation.y < height)
            {
                if (!path.Contains(targetLocation))
                {
                    if (map[targetLocation.y][targetLocation.x] == map[newLocation.y][newLocation.x] + 1)
                    {
                        HashSet<(int, int)> newPath = new(path);
                        newPath.Add(newLocation);
                        recurse(paths, newPath, targetLocation);
                    }
                }
            }
        }
    }
}


void P1()
{
    int result = 0;

    foreach (var trailHead in trailHeads)
    {
        List<HashSet<(int x, int y)>> paths = new();
        recurse(paths, new(), trailHead);

        HashSet<(int, int)> unique9s = new();
        foreach (var path in paths)
        {
            foreach (var loc9 in path.Where(loc => map[loc.y][loc.x] == 9))
                unique9s.Add(loc9);
        }
        result += unique9s.Count;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int result = 0;

    foreach (var trailHead in trailHeads)
    {
        List<HashSet<(int, int)>> paths = new();
        recurse(paths, new(), trailHead);
        result += paths.Count;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
