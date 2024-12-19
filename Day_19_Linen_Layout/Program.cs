using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<string> availableDesigns = inputList[0].Split(',').Select(s => s.Trim()).ToList();
List<string> displayDesigns = inputList.Skip(2).ToList();

Dictionary<string, bool> cache;
bool recurse(List<string> availableDesigns, string remainingDesignToMake)
{
    if (remainingDesignToMake.Length == 0)
        return true;
    if (cache.TryGetValue(remainingDesignToMake, out bool result))
        return result;
    foreach (string availableDesign in availableDesigns)
    {
        if (remainingDesignToMake.StartsWith(availableDesign))
        {
            if (recurse(availableDesigns, remainingDesignToMake.Substring(availableDesign.Length)))
            {
                cache[remainingDesignToMake] = true;
                return true;
            }
        }
    }
    cache[remainingDesignToMake] = false;
    return false;
}

Dictionary<string, Int64> cache2;
Int64 recurse2(List<string> availableDesigns, string remainingDesignToMake)
{
    if (remainingDesignToMake.Length == 0)
        return 1;
    if (cache2.TryGetValue(remainingDesignToMake, out Int64 result))
        return result;
    result = 0;
    foreach (string availableDesign in availableDesigns)
    {
        if (remainingDesignToMake.StartsWith(availableDesign))
        {
            result += recurse2(availableDesigns, remainingDesignToMake.Substring(availableDesign.Length));
        }
    }
    cache2[remainingDesignToMake] = result;
    return result;
}

void P1()
{
    cache = new();

    int result = 0;
    foreach (var displayDesign in displayDesigns)
    {
        if (recurse(availableDesigns, displayDesign))
            result++;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    cache2 = new();

    Int64 result = 0;
    foreach (var displayDesign in displayDesigns)
    {
        Int64 numWays = recurse2(availableDesigns, displayDesign);
        result += numWays;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
