using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Robot> robots = inputList.Select(s => new Robot(s)).ToList();

const int width = 101;
const int height = 103;

List<List<int>> countByCoord = new List<List<int>>();
for (int y = 0; y < height; y++)
{
    List<int> row = new();
    countByCoord.Add(row);
    for (int x = 0; x < width; x++)
    {
        int count = 0;
        foreach (Robot robot in robots)
        {
            if (robot.PosX == x && robot.PosY == y)
                count++;
        }
        row.Add(count);
    }
}

#pragma warning disable 8321
void Print()
{
    string s = "";
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            int count = countByCoord[y][x];
            if (count == 0)
                s += " ";
            else
                s += count;

        }
        s += Environment.NewLine;
    }
    Console.WriteLine(s);
}
#pragma warning restore 8321

void P1()
{
    //Print();
    for (int i = 0; i < 100; i++)
    {
        foreach (Robot robot in robots)
        {
            robot.Tick(width, height, countByCoord);
        }
        //Print();
    }

    Int64 quadrantCountTL = 0;
    Int64 quadrantCountTR = 0;
    Int64 quadrantCountBL = 0;
    Int64 quadrantCountBR = 0;
    const int midX = width / 2;
    const int midY = height / 2;
    foreach (Robot robot in robots)
    {
        if (robot.PosX < midX)
        {
            if (robot.PosY < midY)
            {
                quadrantCountTL++;
            }
            else if (robot.PosY > midY)
            {
                quadrantCountBL++;
            }
        }
        else if (robot.PosX > midX)
        {
            if (robot.PosY < midY)
            {
                quadrantCountTR++;
            }
            else if (robot.PosY > midY)
            {
                quadrantCountBR++;
            }
        }
    }

    Console.WriteLine(quadrantCountTL * quadrantCountTR * quadrantCountBL * quadrantCountBR);
    Console.ReadLine();
}

void P2()
{
    int result = 0;

    for (int i = 100; true; i++)
    {
        foreach (Robot robot in robots)
        {
            robot.Tick(width, height, countByCoord);
        }

        bool candidate = false;
        for (int y = 0; y < height; y++)
        {
            int inARow = 0;
            for (int x = 0; x < width; x++)
            {
                if (countByCoord[y][x] > 0)
                    inARow++;
                else
                    inARow = 0;

                if (inARow >= 20)
                {
                    candidate = true;
                    break;
                }
            }
            if (candidate)
                break;
        }

        if (candidate)
        {
            //Print();
            result = i + 1;
            break;
            //Console.WriteLine(i + 1);
            //Console.ReadLine();
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Robot
{
    public int PosX;
    public int PosY;
    public int VelX;
    public int VelY;

    public Robot(string s)
    {
        var split = s.Split(' ');
        var split2 = split[0][2..].Split(',');
        PosX = int.Parse(split2[0]);
        PosY = int.Parse(split2[1]);
        split2 = split[1][2..].Split(',');
        VelX = int.Parse(split2[0]);
        VelY = int.Parse(split2[1]);

    }

    public void Tick(int width, int height, List<List<int>> countByCoord)
    {
        countByCoord[PosY][PosX]--;
        int newX = PosX + VelX;
        int newY = PosY + VelY;
        if (newX < 0) newX = width + (newX % width);
        if (newX >= width) newX = newX % width;
        if (newY < 0) newY = height + (newY % height);
        if (newY >= height) newY = newY % height;
        PosX = newX;
        PosY = newY;
        countByCoord[PosY][PosX]++;
    }
}