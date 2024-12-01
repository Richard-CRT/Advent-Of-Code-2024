using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<int> List1 = new();
List<int> List2 = new();
foreach (string s in inputList)
{
    var split = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    List1.Add(int.Parse(split[0]));
    List2.Add(int.Parse(split[1]));
}

void P1()
{
    List1.Sort();
    List2.Sort();

    int sum = 0;
    for (int i = 0; i < List1.Count; i++)
        sum += Math.Abs(List2[i] - List1[i]);

    Console.WriteLine(sum);
    Console.ReadLine();
}

void P2()
{
    Int64 similarity = 0;
    for (int i = 0; i < List1.Count; i++)
        similarity += List1[i] * List2.Count(v => v == (List1[i]));

    Console.WriteLine(similarity);
    Console.ReadLine();
}

P1();
P2();
