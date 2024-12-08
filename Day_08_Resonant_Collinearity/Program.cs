using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

HashSet<char> allFrequencies = new();
Dictionary<char, List<(int, int)>> antennasByFrequency = new();
List<List<HashSet<char>>> antennasByCoord = new();
int width = inputList[0].Length;
int height = inputList.Count;
for (int y = 0; y < height; y++)
{
    List<HashSet<char>> row = new();
    antennasByCoord.Add(row);
    for (int x = 0; x < width; x++)
    {
        HashSet<char> antennas = new();
        row.Add(antennas);
        if (inputList[y][x] != '.')
        {
            char freq = inputList[y][x];
            antennas.Add(freq);
            allFrequencies.Add(freq);
            if (antennasByFrequency.TryGetValue(freq, out List<(int, int)>? coords))
            {
                coords.Add((x, y));
            }
            else
            {
                antennasByFrequency[freq] = new List<(int, int)> { (x, y) };
            }
        }
    }
}

void P1()
{
    int result = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            bool antinode = false;

            foreach ((char frequency, List<(int, int)> antennas) in antennasByFrequency)
            {
                foreach ((int antenna1X, int antenna1Y) in antennas)
                {
                    int dx = antenna1X - x;
                    int dy = antenna1Y - y;

                    if (dx != 0 || dy != 0)
                    {
                        int targetX = x + (dx * 2);
                        int targetY = y + (dy * 2);
                        if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                        {
                            if (antennasByCoord[targetY][targetX].Contains(frequency))
                            {
                                antinode = true;
                                break;
                            }
                        }
                    }
                }
                if (antinode) break;
            }

            if (antinode)
                result++;
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    HashSet<(int, int)> antinodes = new();

    foreach ((char frequency, List<(int, int)> antennas) in antennasByFrequency)
    {
        for (int i = 0; i < antennas.Count; i++)
        {
            for (int j = i + 1; j < antennas.Count; j++)
            {
                (int antenna1X, int antenna1Y) = antennas[i];
                (int antenna2X, int antenna2Y) = antennas[j];

                int dx = antenna2X - antenna1X;
                int dy = antenna2Y - antenna1Y;

                // The input has been crafted so they're coprime, but we'll do this anyway just for completeness
                int gcf = (int)AoC.GCF(dx, dy);
                dx = dx / gcf;
                dy = dy / gcf;

                int mulToExitLeft = (int)Math.Ceiling((antenna1X - 0) / ((double)Math.Abs(dx)));
                int mulToExitRight = (int)Math.Ceiling(((width - 1) - antenna1X) / ((double)Math.Abs(dx)));
                int mulToExitTop = (int)Math.Ceiling((antenna1Y - 0) / ((double)Math.Abs(dy)));
                int mulToExitBottom = (int)Math.Ceiling(((height - 1) - antenna1Y) / ((double)Math.Abs(dy)));

                int mulLimit = (new List<int>() { mulToExitLeft, mulToExitRight, mulToExitTop, mulToExitBottom }).Max();

                for (int mul = -mulLimit; mul <= mulLimit; mul++)
                {
                    int targetX = antenna1X + (dx * mul);
                    int targetY = antenna1Y + (dy * mul);
                    if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                    {
                        antinodes.Add((targetX, targetY));
                    }
                }
            }
        }
    }

    Console.WriteLine(antinodes.Count);
    Console.ReadLine();
}

P1();
P2();
