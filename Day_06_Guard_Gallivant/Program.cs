using AdventOfCodeUtilities;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

HashSet<(int, int)> obstructions = new(); ;

(int, int)? startLocation = null;
for (int y = 0; y < inputList.Count; y++)
{
    for (int x = 0; x < inputList[y].Length; x++)
    {
        if (inputList[y][x] == '#')
            obstructions.Add((x, y));
        if (inputList[y][x] == '^')
            startLocation = (x, y);
    }
}
int height = inputList.Count;
int width = inputList[0].Count();
Debug.Assert(startLocation is not null);
HashSet<(int, int)> unobstructedHistory = new();

void P1()
{
    int direction = 0;
    (int, int)? currentLocation = startLocation;
    while (currentLocation is not null)
    {
        (int newX,int newY) = currentLocation.Value;
        if (direction == 0) newY--;
        else if (direction == 1) newX++;
        else if (direction == 2) newY++;
        else if (direction == 3) newX--;
        if (newX < 0 || newX >= width || newY < 0 || newY >= height)
            currentLocation = null;
        else
        {
            (int, int) testNewLocation = (newX, newY);
            if (!obstructions.Contains(testNewLocation))
            {
                currentLocation = testNewLocation;
                unobstructedHistory.Add(currentLocation.Value);
            }
            else
            {
                direction = (direction + 1) % 4;
            }
        }
    }

    Console.WriteLine(unobstructedHistory.Count);
    Console.ReadLine();
}

void P2()
{
    int loops = 0;
    foreach ((int,int) extraObsLocation in unobstructedHistory)
    {
        if (extraObsLocation != startLocation)
        {
            HashSet<(int, int)> history = new();
            HashSet<((int, int), int)> historyWithDirection = new();
            int direction = 0;
            (int, int)? currentLocation = startLocation;
            while (currentLocation is not null)
            {
                (int newX, int newY) = currentLocation.Value;
                if (direction == 0) newY--;
                else if (direction == 1) newX++;
                else if (direction == 2) newY++;
                else if (direction == 3) newX--;
                if (newX < 0 || newX >= width || newY < 0 || newY >= height)
                    currentLocation = null;
                else
                {
                    (int, int) testNewLocation = (newX, newY);
                    if (!obstructions.Contains(testNewLocation) && testNewLocation != extraObsLocation)
                    {
                        currentLocation = testNewLocation;
                        ((int, int), int) testNewLocationWithDirection = (currentLocation.Value, direction);
                        if (!historyWithDirection.Add(testNewLocationWithDirection))
                        {
                            loops++;
                            break;
                        }
                    }
                    else direction = (direction + 1) % 4;
                }
            }
        }
    }
    
    Console.WriteLine(loops);
    Console.ReadLine();
}

P1();
P2();
