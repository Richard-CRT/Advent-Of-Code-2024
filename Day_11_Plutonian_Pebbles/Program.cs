using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Int64> initialValues = inputList[0].Split(' ').Select(s => Int64.Parse(s)).ToList();;
List<Stone> stones = initialValues.Select(i => new Stone(i)).ToList();
for (int i = 0; i < stones.Count - 1; i++)
{
    stones[i].NextStone = stones[i + 1];
}
Stone initialStone = stones[0];

/*
void Print(Stone initialStone)
{
    Stone? currentStone = initialStone;
    while (currentStone is not null)
    {
        Console.Write($"{currentStone.Value} ");
        currentStone = currentStone.NextStone;
    }
    Console.WriteLine();
}
*/

// Naive P1 solution
void P1()
{
    //Print(initialStone);

    for (int i = 0; i < 25; i++)
    {
        Stone? currentStone = initialStone;
        while (currentStone is not null)
        {
            if (currentStone.Value == 0)
                currentStone.Value = 1;
            else
            {
                string strRep = currentStone.Value.ToString();
                if (strRep.Length % 2 == 0)
                {
                    int lenDiv2 = strRep.Length / 2;
                    currentStone.Value = Int64.Parse(strRep.Substring(0, lenDiv2));
                    Stone newStone = new(Int64.Parse(strRep.Substring(lenDiv2)));
                    newStone.NextStone = currentStone.NextStone;
                    currentStone.NextStone = newStone;

                    currentStone = newStone;
                }
                else
                    currentStone.Value = currentStone.Value * 2024;
            }

            currentStone = currentStone.NextStone;
        }
    }


    Stone? currentStone2 = initialStone;
    int j;
    for (j = 0; currentStone2 is not null; j++)
    {
        currentStone2 = currentStone2.NextStone;
    }

    Console.WriteLine(j);
    Console.ReadLine();
}

Dictionary<(Int64, int), Int64> CacheIntAndNumBlinksToNumStones = new();
Int64 IntAndNumBlinksToNumStones(Int64 input, int numBlinks)
{
    if (numBlinks == 0)
        return 1;

    if (CacheIntAndNumBlinksToNumStones.TryGetValue((input, numBlinks), out Int64 numStones))
        return numStones;
    else
    {
        if (input == 0)
        {
            Int64 newVal = 1;
            Int64 resultingStoneCount = IntAndNumBlinksToNumStones(newVal, numBlinks - 1);
            //Console.WriteLine($"1 | {input}, {numBlinks} = {resultingStoneCount}");
            CacheIntAndNumBlinksToNumStones[(input, numBlinks)] = resultingStoneCount;
            return resultingStoneCount;
        }
        else
        {
            string strRep = input.ToString();
            if (strRep.Length % 2 == 0)
            {
                int lenDiv2 = strRep.Length / 2;
                Int64 leftVal = Int64.Parse(strRep.Substring(0, lenDiv2));
                Int64 rightVal = Int64.Parse(strRep.Substring(lenDiv2));
                Int64 resultingStoneCount1 = IntAndNumBlinksToNumStones(leftVal, numBlinks - 1);
                Int64 resultingStoneCount2 = IntAndNumBlinksToNumStones(rightVal, numBlinks - 1);
                Int64 resultingStoneCount = resultingStoneCount1 + resultingStoneCount2;
                //Console.WriteLine($"2 | {input}, {numBlinks} = {resultingStoneCount} = {resultingStoneCount1} + {resultingStoneCount2}");
                CacheIntAndNumBlinksToNumStones[(input, numBlinks)] = resultingStoneCount;
                return resultingStoneCount;
            }
            else
            {
                Int64 newVal = input * 2024;
                Int64 resultingStoneCount = IntAndNumBlinksToNumStones(newVal, numBlinks - 1);
                //Console.WriteLine($"3 | {input}, {numBlinks} = {resultingStoneCount}");
                CacheIntAndNumBlinksToNumStones[(input, numBlinks)] = resultingStoneCount;
                return resultingStoneCount;
            }
        }
    }
}

void P2()
{
    Int64 result = 0;
    foreach (Int64 initialValue in initialValues)
        result += IntAndNumBlinksToNumStones(initialValue, 75);

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Stone
{
    public Stone? NextStone = null;
    public Int64 Value;

    public Stone(Int64 value)
    {
        Value = value;
    }
}
