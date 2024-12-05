using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<List<int>> grid = new();
for (int y = 0; y < inputList.Count; y++)
{
    List<int> row = new();
    for (int x = 0; x < inputList[y].Length; x++)
    {
        row.Add(inputList[y][x]);
    }
    grid.Add(row);
}

void P1()
{


    int result = 0;

    for (int y = 0; y < inputList.Count; y++)
    {
        for (int x = 0; x < inputList[y].Length; x++)
        {
            List<(int, int)> directions = new() { (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1) };
            foreach ((int dx, int dy) in directions)
            {
                const string pattern = "XMAS";
                int furthestX = x + ((pattern.Length - 1) * dx);
                int furthestY = y + ((pattern.Length - 1) * dy);
                if (furthestX >= 0 && furthestX < inputList[y].Length &&
                    furthestY >= 0 && furthestY < inputList.Count)
                {
                    bool matches = true;
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        int iterX = x + (i * dx);
                        int iterY = y + (i * dy);
                        if (grid[iterY][iterX] != pattern[i])
                        {
                            matches = false;
                            break;
                        }
                    }
                    if (matches)
                    {
                        result++;
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
    int result = 0;
    for (int y = 1; y < inputList.Count - 1; y++)
    {
        for (int x = 1; x < inputList[y].Length - 1; x++)
        {
            if (grid[y][x] == 'A')
            {
                if ((grid[y - 1][x - 1] == 'M' && grid[y + 1][x + 1] == 'S') ||
                    (grid[y - 1][x - 1] == 'S' && grid[y + 1][x + 1] == 'M'))
                {
                    if ((grid[y - 1][x + 1] == 'M' && grid[y + 1][x - 1] == 'S') ||
                    (grid[y - 1][x + 1] == 'S' && grid[y + 1][x - 1] == 'M'))
                    {
                        result++;
                    }
                }
            }
        }
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
