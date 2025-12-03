using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<int[]> keys = new();
List<int[]> locks = new();

int i = 0;
while (i < inputList.Count)
{
    bool key;
    if (inputList[i] == "#####")
        key = false;
    else
        key = true;
    i++;

    if (key)
    {
        int[] heights = new int[5] { 0, 0, 0, 0, 0 };
        for (int j = 0; j < 5; j++, i++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (inputList[i][x] == '#' && heights[x] == 0)
                    heights[x] = 5 - j;
            }
        }
        keys.Add(heights);
    }
    else
    {
        int[] heights = new int[5] { 5, 5, 5, 5, 5 };
        for (int j = 0; j < 5; j++, i++)
        {
            for (int x = 0; x < 5; x++)
            {
                if (inputList[i][x] == '.' && heights[x] == 5)
                    heights[x] = j;
            }
        }
        locks.Add(heights);
    }
    i++;
    i++;
}

void P1()
{
    int result = 0;

    for (int lockIndex = 0; lockIndex  < locks.Count; lockIndex++)
    {
        for (int keyIndex = 0;  keyIndex < keys.Count; keyIndex++)
        {
            bool fits = true;
            for (int x = 0; x < 5; x++)
            {
                if (locks[lockIndex][x] + keys[keyIndex][x] > 5)
                {
                    fits = false;
                    break;
                }
            }
            if (fits)
                result++;
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
