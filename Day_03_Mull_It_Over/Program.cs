using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

string input = AoC.GetInput();

void P1()
{
    Int64 result = 0;
    var matches = AoC.RegexMatch(input, "mul\\((\\d{1,3}),(\\d{1,3})\\)");
    int a;
    int b;
    foreach (Match match in matches)
    {
        a = int.Parse(match.Groups[1].Value);
        b = int.Parse(match.Groups[2].Value);
        result += (Int64)a * (Int64)b;
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;
    var matches = AoC.RegexMatch(input, "mul\\((\\d{1,3}),(\\d{1,3})\\)|do\\(\\)|don't\\(\\)");
    int a;
    int b;
    bool enabled = true;
    foreach (Match match in matches)
    {
        if (match.Groups[0].Value == "do()") enabled = true;
        else if (match.Groups[0].Value == "don't()") enabled = false;
        else if (enabled)
        {
            a = int.Parse(match.Groups[1].Value);
            b = int.Parse(match.Groups[2].Value);
            result += (Int64)a * (Int64)b;
        }
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
