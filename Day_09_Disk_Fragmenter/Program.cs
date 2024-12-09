using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<int> blocks = new();
bool file = true;
int fileId = 0;
Dictionary<int, int> fileIdToLength = new();
Dictionary<int, int> fileIdToOriginalLocation = new();
foreach (char c in inputList[0])
{
    if (file)
    {
        int length = int.Parse(c.ToString());
        fileIdToLength[fileId] = length;
        fileIdToOriginalLocation[fileId] = blocks.Count;
        for (int i = 0; i < length; i++)
            blocks.Add(fileId);
        fileId++;
    }
    else
        for (int i = 0; i < int.Parse(c.ToString()); i++)
            blocks.Add(-1);
    file = !file;
}
int maxFileId = fileId - 1;

void P1()
{
    List<int> blocksP1 = new(blocks);

    int firstFreeIndex = 0;
    int lastFilledIndex = blocksP1.Count - 1;

    while (true)
    {
        for (; blocksP1[firstFreeIndex] != -1; firstFreeIndex++) ;
        for (; blocksP1[lastFilledIndex] == -1; lastFilledIndex--) ;

        if (lastFilledIndex <= firstFreeIndex)
            break;

        blocksP1[firstFreeIndex] = blocksP1[lastFilledIndex];
        blocksP1[lastFilledIndex] = -1;
    }

    Int64 result = 0;

    for (int i = 1; i <= lastFilledIndex; i++)
    {
        result += (Int64)i * (Int64)blocksP1[i];
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    List<int> blocksP2 = new(blocks);

    for (int fileId = maxFileId; fileId >= 0; fileId--)
    {
        int originalLocation = fileIdToOriginalLocation[fileId];
        int length = fileIdToLength[fileId];
        // Find first gap of length length
        for (int destLocation = 0; destLocation < originalLocation && destLocation < blocksP2.Count - (length - 1); destLocation++)
        {
            bool longEnoughEmpty = true;
            for (int i = 0; i < length; i++)
            {
                if (blocksP2[destLocation + i] != -1)
                {
                    longEnoughEmpty = false;
                    break;
                }
            }
            if (longEnoughEmpty)
            {
                for (int i = 0; i < length; i++)
                {
                    blocksP2[destLocation + i] = blocksP2[originalLocation + i];
                    blocksP2[originalLocation + i] = -1;
                }
                break;
            }
        }
    }

    Int64 result = 0;

    for (int i = 1; i < blocksP2.Count; i++)
    {
        if (blocksP2[i] != -1)
            result += (Int64)i * (Int64)blocksP2[i];
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
