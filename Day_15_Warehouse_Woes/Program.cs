using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;


List<string> inputList = AoC.GetInputLines();

List<List<Tile>> map = new();
List<List<Tile>> mapP2 = new();

int y;
int initialRobotX = -1;
int initialRobotY = -1;
int initialRobotXP2 = -1;
for (y = 0; inputList[y] != ""; y++)
{
    List<Tile> row = new();
    map.Add(row);
    List<Tile> rowP2 = new();
    mapP2.Add(rowP2);
    for (int x = 0; x < inputList[y].Length; x++)
    {
        switch (inputList[y][x])
        {
            case '@':
            case '.':
                row.Add(Tile.Nothing);
                rowP2.Add(Tile.Nothing);
                rowP2.Add(Tile.Nothing);
                break;
            case '#':
                row.Add(Tile.Wall);
                rowP2.Add(Tile.Wall);
                rowP2.Add(Tile.Wall);
                break;
            case 'O':
                row.Add(Tile.Box);
                rowP2.Add(Tile.BoxLeft);
                rowP2.Add(Tile.BoxRight);
                break;
            default: throw new NotImplementedException();
        }

        if (inputList[y][x] == '@')
        {
            initialRobotX = x;
            initialRobotY = y;
            initialRobotXP2 = x * 2;
        }
    }
}
int width = map[0].Count;
int height = map.Count;
int widthP2 = width * 2;
Debug.Assert(initialRobotX >= 0 && initialRobotY >= 0);
Debug.Assert(initialRobotXP2 >= 0);

string instructionString = "";
y++;
for (; y < inputList.Count; y++)
{
    instructionString += inputList[y];
}
List<Instruction> instructions = instructionString.Select(c => (Instruction)c).ToList();

void Print(int robotX, int robotY, bool p2 = false)
{
    string s = "";
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < (p2 ? widthP2 : width); x++)
        {
            if (y == robotY && x == robotX)
                s += '@';
            else
                s += p2 ? (char)mapP2[y][x] : (char)map[y][x];
        }
        s += Environment.NewLine;
    }
    Console.WriteLine(s);
}

void P1()
{
    (int x, int y) robotLocation = (initialRobotX, initialRobotY);
    Queue<Instruction> instructionQueue = new(instructions);

    //Print(robotLocation.x, robotLocation.y);

    while (instructionQueue.Count > 0)
    {
        Instruction instruction = instructionQueue.Dequeue();
        (int x, int y) d;
        switch (instruction)
        {
            case Instruction.Up: d = (0, -1); break;
            case Instruction.Down: d = (0, 1); break;
            case Instruction.Left: d = (-1, 0); break;
            case Instruction.Right: d = (1, 0); break;
            default: throw new NotImplementedException();
        }

        bool possibleToMove = false;
        (int x, int y) proposedRobotLocation = (robotLocation.x + d.x, robotLocation.y + d.y);
        (int x, int y) check = proposedRobotLocation;
        List<(int x, int y)> coordsToMove = new();
        while (check.x >= 0 && check.x < width && check.y >= 0 && check.y < height)
        {
            if (map[check.y][check.x] == Tile.Nothing)
            {
                possibleToMove = true;
                break;
            }
            else
            {
                if (map[check.y][check.x] == Tile.Wall)
                    break;
                else
                {
                    coordsToMove.Add(check);
                    check = (check.x + d.x, check.y + d.y);
                }
            }
        }
        if (possibleToMove)
        {
            robotLocation = proposedRobotLocation;
            if (coordsToMove.Count > 0)
            {
                Debug.Assert(coordsToMove[0] == robotLocation);
                Debug.Assert(map[check.y][check.x] == Tile.Nothing);
                Debug.Assert(map[robotLocation.y][robotLocation.x] == Tile.Box);
                map[check.y][check.x] = Tile.Box;
                map[robotLocation.y][robotLocation.x] = Tile.Nothing;
            }
        }
        //Print(robotLocation.x, robotLocation.y);
    }

    Int64 result = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            if (map[y][x] == Tile.Box)
            {
                result += (y * 100) + x;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    (int x, int y) robotLocation = (initialRobotXP2, initialRobotY);
    Queue<Instruction> instructionQueue = new(instructions);

    //Print(robotLocation.x, robotLocation.y, true);

    while (instructionQueue.Count > 0)
    {
        Instruction instruction = instructionQueue.Dequeue();
        (int x, int y) d;
        switch (instruction)
        {
            case Instruction.Up: d = (0, -1); break;
            case Instruction.Down: d = (0, 1); break;
            case Instruction.Left: d = (-1, 0); break;
            case Instruction.Right: d = (1, 0); break;
            default: throw new NotImplementedException();
        }

        bool possibleToMove = false;
        (int x, int y) proposedRobotLocation = (robotLocation.x + d.x, robotLocation.y + d.y);
        List<(int x, int y)> coordinatesThatNeedToBeEmpty = new() { proposedRobotLocation };
        List<(int x, int y)> coordsToMove = new();
        while (true)
        {
            if (coordinatesThatNeedToBeEmpty.All(check => mapP2[check.y][check.x] == Tile.Nothing))
            {
                possibleToMove = true;
                break;
            }
            else
            {
                if (coordinatesThatNeedToBeEmpty.Any(check => mapP2[check.y][check.x] == Tile.Wall))
                    break;
                else
                {
                    // Not all the coordinates that need to be empty are nothing, but none of them are walls
                    if (instruction == Instruction.Left || instruction == Instruction.Right)
                    {
                        coordsToMove.AddRange(coordinatesThatNeedToBeEmpty);
                        coordinatesThatNeedToBeEmpty = coordinatesThatNeedToBeEmpty.Select(check => (check.x + d.x, check.y + d.y)).ToList();
                    }
                    else
                    {

                        // Need to account for alignment, yuck
                        List<(int x, int y)> newCoordinatesThatNeedToBeEmpty = new();
                        foreach (var coordinateThatNeedsToBeEmpty in coordinatesThatNeedToBeEmpty)
                        {
                            if (mapP2[coordinateThatNeedsToBeEmpty.y][coordinateThatNeedsToBeEmpty.x] == Tile.BoxRight)
                            {
                                coordsToMove.Add(coordinateThatNeedsToBeEmpty);
                                coordsToMove.Add((coordinateThatNeedsToBeEmpty.x - 1, coordinateThatNeedsToBeEmpty.y));

                                newCoordinatesThatNeedToBeEmpty.Add((coordinateThatNeedsToBeEmpty.x + d.x, coordinateThatNeedsToBeEmpty.y + d.y));
                                newCoordinatesThatNeedToBeEmpty.Add((coordinateThatNeedsToBeEmpty.x + d.x - 1, coordinateThatNeedsToBeEmpty.y + d.y));
                            }
                            else if (mapP2[coordinateThatNeedsToBeEmpty.y][coordinateThatNeedsToBeEmpty.x] == Tile.BoxLeft)
                            {
                                coordsToMove.Add(coordinateThatNeedsToBeEmpty);
                                coordsToMove.Add((coordinateThatNeedsToBeEmpty.x + 1, coordinateThatNeedsToBeEmpty.y));

                                newCoordinatesThatNeedToBeEmpty.Add((coordinateThatNeedsToBeEmpty.x + d.x, coordinateThatNeedsToBeEmpty.y + d.y));
                                newCoordinatesThatNeedToBeEmpty.Add((coordinateThatNeedsToBeEmpty.x + d.x + 1, coordinateThatNeedsToBeEmpty.y + d.y));
                            }
                            else
                            {
                                // If a space in front of part of a box is empty, great, no need to run any further checks from that spot
                            }
                        }
                        // Need to do Distinct as the above code can cause duplicates to be added
                        coordinatesThatNeedToBeEmpty = newCoordinatesThatNeedToBeEmpty.Distinct().ToList();
                    }
                }
            }
        }
        if (possibleToMove)
        {
            robotLocation = proposedRobotLocation;
            if (coordsToMove.Count > 0)
            {
                // Need to do Distinct as the above code can cause duplicates to be added
                coordsToMove = coordsToMove.Distinct().ToList();
                coordsToMove.Reverse();
                foreach (var coordToMove in coordsToMove)
                {
                    mapP2[coordToMove.y + d.y][coordToMove.x + d.x] = mapP2[coordToMove.y][coordToMove.x];
                    mapP2[coordToMove.y][coordToMove.x] = Tile.Nothing;
                }

            }
        }
        //Print(robotLocation.x, robotLocation.y, true);
    }

    Int64 result = 0;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < widthP2; x++)
        {
            if (mapP2[y][x] == Tile.BoxLeft)
            {
                result += (y * 100) + x;
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public enum Tile
{
    Nothing = '.',
    Box = 'O',
    Wall = '#',
    BoxLeft = '[',
    BoxRight = ']',
}

public enum Instruction
{
    Up = '^', Down = 'v', Left = '<', Right = '>'
}

