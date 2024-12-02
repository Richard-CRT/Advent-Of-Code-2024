using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<int[]> reports = inputList.Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)).ToArray()).ToList();

void P1()
{
    int result = 0;

    foreach (int[] report in reports)
    {
        int initialDiff = report[1] - report[0];
        int direction = 0;
        if (initialDiff > 0)
            direction = 1;
        else if (initialDiff < 0)
            direction = -1;
        if (direction != 0)
        {
            bool success = true;
            for (int i = 1; i < report.Length; i++)
            {
                int diff = (report[i] - report[i - 1]) * direction;
                if (diff < 1 || diff > 3)
                {
                    success = false;
                    break;
                }
            }
            if (success)
                result++;
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int result = 0;

    foreach (int[] report in reports)
    {
        bool success;
        for (int j = 0; j < report.Length; j++)
        {
            List<int> editedReport = report.ToList();
            editedReport.RemoveAt(j);

            int initialDiff = editedReport[1] - editedReport[0];
            int direction = 0;
            if (initialDiff > 0)
                direction = 1;
            else if (initialDiff < 0)
                direction = -1;
            if (direction != 0)
            {
                success = true;
                for (int i = 1; i < editedReport.Count; i++)
                {
                    int diff = (editedReport[i] - editedReport[i - 1]) * direction;
                    if (diff < 1 || diff > 3)
                    {
                        success = false;
                        break;
                    }
                }
                if (success)
                {
                    result++;
                    break;
                }
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
