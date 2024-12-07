using AdventOfCodeUtilities;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Equation> equations = inputList.Select(s => new Equation(s)).ToList();

void P1()
{
    Int64 result = 0;
    foreach (Equation equation in equations)
    {
        if (equation.Solve()) result += equation.Result;
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;
    foreach (Equation equation in equations)
    {
        if (equation.Solve(part2: true)) result += equation.Result;
    }
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Equation
{
    public enum Operation
    {
        Add,
        Multiply,
        Combine,
    }

    public Int64 Result;
    public List<Int64> Parts = new();

    public Equation(string s)
    {
        var split = s.Split(':', StringSplitOptions.RemoveEmptyEntries);
        Result = Int64.Parse(split[0]);
        Parts = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(s => Int64.Parse(s)).ToList();
    }

    public bool Solve(List<Operation>? operations = null, bool part2 = false)
    {
        if (operations == null) operations = new();
        if (operations.Count < Parts.Count - 1)
        {
            IEnumerable<Operation> operationList = part2 ? new List<Operation>() { Operation.Add, Operation.Multiply, Operation.Combine } : new List<Operation>() { Operation.Add, Operation.Multiply };
            foreach (Operation operation in operationList)
            {
                List<Operation> newOperations = new(operations);
                newOperations.Add(operation);
                if (Solve(newOperations, part2: part2)) return true;
            }
        }
        else
        {
            Int64 trialResult = Parts[0];
            for (int i = 0; i < operations.Count; i++)
            {
                switch (operations[i])
                {
                    case Operation.Add:
                        trialResult = trialResult + Parts[i + 1];
                        break;
                    case Operation.Multiply:
                        trialResult = trialResult * Parts[i + 1];
                        break;
                    case Operation.Combine:
                        trialResult = Int64.Parse($"{trialResult}{Parts[i + 1]}");
                        break;
                }
            }
            if (trialResult == Result)
            {
                return true;
            }
        }
        return false;
    }
}
