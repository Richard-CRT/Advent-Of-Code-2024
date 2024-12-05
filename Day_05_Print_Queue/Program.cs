using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
int blankLineIndex = inputList.IndexOf("");
List<int[]> updates = new();
for (int i = blankLineIndex + 1; i < inputList.Count; i++)
{
    updates.Add(inputList[i].Split(',').Select(s => int.Parse(s)).ToArray());
}
List<(int, int)> rules = new();
for (int i = 0; i < blankLineIndex; i++)
{
    var split = inputList[i].Split('|');
    rules.Add((int.Parse(split[0]), int.Parse(split[1])));
}
List<int[]> wrongOrderUpdates = new();

void P1()
{
    int result = 0;

    foreach (int[] update in updates)
    {
        HashSet<int> pagesSet = new(update);
        bool correctlyOrdered = true;
        foreach ((int prePage, int postPage) in rules)
        {
            if (pagesSet.Contains(prePage) && pagesSet.Contains(postPage))
            {
                if (Array.IndexOf(update, prePage) >= Array.IndexOf(update, postPage))
                {
                    wrongOrderUpdates.Add(update);
                    correctlyOrdered = false;
                    break;
                }
            }
        }
        if (correctlyOrdered)
        {
            Debug.Assert(update.Length % 2 == 1);
            result += update[(update.Length - 1) / 2];
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int result = 0;

    foreach (int[] update in wrongOrderUpdates)
    {
        bool changeMade = true;
        while (changeMade)
        {
            changeMade = false;
            foreach ((int prePage, int postPage) in rules)
            {
                if (update.Contains(prePage) && update.Contains(postPage))
                {
                    int a = Array.IndexOf(update, prePage);
                    int b = Array.IndexOf(update, postPage);
                    if (a >= b)
                    {
                        changeMade = true;
                        update[b] = prePage;
                        update[a] = postPage;
                        break;
                    }
                }
            }
        }
        Debug.Assert(update.Length % 2 == 1);
        result += update[(update.Length - 1) / 2];
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
