using AdventOfCodeUtilities;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

List<string> codes = AoC.GetInputLines();

Dictionary<char, (int x, int y)> keyPadInputToCoord = new()
{
    { '9', (2,0) },
    { '8', (1,0) },
    { '7', (0,0) },
    { '6', (2,1) },
    { '5', (1,1) },
    { '4', (0,1) },
    { '3', (2,2) },
    { '2', (1,2) },
    { '1', (0,2) },
    { '0', (1,3) },
    { 'A', (2,3) },
};
Dictionary<char, (int x, int y)> directionalKeyPadInputToCoord = new()
{
    { 'A', (2,0) },
    { '^', (1,0) },
    { '>', (2,1) },
    { 'v', (1,1) },
    { '<', (0,1) },
};

Dictionary<(char, char), List<string>> keyPadInputsToInstructions = new();
foreach ((char c1, var coord1) in keyPadInputToCoord)
{
    foreach ((char c2, var coord2) in keyPadInputToCoord)
    {
        List<string> instructions = new();

        string horizontalDifference = "";
        if (coord2.x > coord1.x)
            horizontalDifference = new string('>', coord2.x - coord1.x);
        else if (coord2.x < coord1.x)
            horizontalDifference = new string('<', coord1.x - coord2.x);
        string verticalDifference = "";
        if (coord2.y > coord1.y)
            verticalDifference = new string('v', coord2.y - coord1.y);
        else if (coord2.y < coord1.y)
            verticalDifference = new string('^', coord1.y - coord2.y);

        // Special cases to avoid the gap
        if (coord1.x == 0 && coord1.y <= 2 && coord2.x >= 1 && coord2.y == 3)
        {
            instructions.Add(horizontalDifference + verticalDifference);
        }
        else if (coord1.x >= 1 && coord1.y == 3 && coord2.x == 0 && coord2.y <= 2)
        {
            instructions.Add(verticalDifference + horizontalDifference);
        }
        else
        {
            instructions.Add(horizontalDifference + verticalDifference);
            if (horizontalDifference != "" && verticalDifference != "")
                instructions.Add(verticalDifference + horizontalDifference);
        }
        keyPadInputsToInstructions[(c1, c2)] = instructions;
    }
}

Dictionary<(char, char), List<string>> directionalKeyPadInputsToInstructions = new()
{

    { ('<', '<'), new List<string>() { "" } },
    { ('<', 'v'), new List<string>() { ">" } },
    { ('<', '>'), new List<string>() { ">>" } },
    { ('<', '^'), new List<string>() { ">^" } },
    { ('<', 'A'), new List<string>() { ">>^" } }, // Don't bother with sequences that involve changing direction in middle, they won't be ideal

    { ('v', 'v'), new List<string>() { "" } },
    { ('v', '<'), new List<string>() { "<" } },
    { ('v', '>'), new List<string>() { ">" } },
    { ('v', '^'), new List<string>() { "^" } },
    { ('v', 'A'), new List<string>() { "^>", ">^" } },

    { ('>', '>'), new List<string>() { "" } },
    { ('>', '<'), new List<string>() { "<<" } },
    { ('>', 'v'), new List<string>() { "<" } },
    { ('>', '^'), new List<string>() { "<^", "^<" } },
    { ('>', 'A'), new List<string>() { "^" } },

    { ('^', '^'), new List<string>() { "" } },
    { ('^', '<'), new List<string>() { "v<" } },
    { ('^', 'v'), new List<string>() { "v" } },
    { ('^', '>'), new List<string>() { "v>", ">v" } },
    { ('^', 'A'), new List<string>() { ">" } },

    { ('A', 'A'), new List<string>() { "" } },
    { ('A', '<'), new List<string>() { "v<<" } }, // Don't bother with sequences that involve changing direction in middle, they won't be ideal
    { ('A', 'v'), new List<string>() { "v<", "<v" } },
    { ('A', '>'), new List<string>() { "v" } },
    { ('A', '^'), new List<string>() { "<" } },
};

/*
Dictionary<(char, char), string> directionalKeyPadInputsToInstructions = new();
foreach ((char c1, var coord1) in directionalKeyPadInputToCoord)
{
    foreach ((char c2, var coord2) in directionalKeyPadInputToCoord)
    {
        string instructions = "";
        string horizontalDifference = "";
        if (coord2.x > coord1.x)
            horizontalDifference = new string('>', coord2.x - coord1.x);
        else if (coord2.x < coord1.x)
            horizontalDifference = new string('<', coord1.x - coord2.x);
        string verticalDifference = "";
        if (coord2.y > coord1.y)
            verticalDifference = new string('v', coord2.y - coord1.y);
        else if (coord2.y < coord1.y)
            verticalDifference = new string('^', coord1.y - coord2.y);

        // Special cases to avoid the gap
        if (coord1.x == 0 && coord1.y == 1 && coord2.x >= 1 && coord2.y == 0)
        {
            instructions = horizontalDifference + verticalDifference;
        }
        else if (coord1.x >= 1 && coord1.y == 0 && coord2.x == 0 && coord2.y == 1)
        {
            instructions = verticalDifference + horizontalDifference;
        }
        else
        {
            instructions = horizontalDifference + verticalDifference;
        }
        directionalKeyPadInputsToInstructions[(c1, c2)] = instructions;
    }
}
*/


void P1()
{
    int result = 0;

    char[] robotLocs = new char[3];
    for (int i = 0; i < robotLocs.Length; i++)
        robotLocs[i] = 'A';

    foreach (string code in codes)
    {
        List<string> keyPadInstructionsInputs = new() { code };
        for (int depth = 0; depth < robotLocs.Length; depth++)
        {
            Dictionary<(char, char), List<string>> dictionaryToUse;
            if (depth == 0)
                dictionaryToUse = keyPadInputsToInstructions;
            else
                dictionaryToUse = directionalKeyPadInputsToInstructions;

            List<string> keyPadInstructions = new() { };
            foreach (string keyPadInstructionsInput in keyPadInstructionsInputs)
            {
                List<string> keyPadInstructionsForThisInput = new() { "" };
                foreach (char charInCode in keyPadInstructionsInput)
                {
                    List<string> possibleInstructions = dictionaryToUse[(robotLocs[depth], charInCode)];
                    if (possibleInstructions.Count == 1)
                    {
                        for (int i = 0; i < keyPadInstructionsForThisInput.Count; i++)
                            keyPadInstructionsForThisInput[i] += possibleInstructions[0] + 'A';
                    }
                    else
                    {
                        var oldDirectionalKeyPadInstructionsForThisInput = keyPadInstructionsForThisInput;
                        keyPadInstructionsForThisInput = new();
                        foreach (string s in oldDirectionalKeyPadInstructionsForThisInput)
                        {
                            foreach (var possibleInstruction in possibleInstructions)
                                keyPadInstructionsForThisInput.Add(s + possibleInstruction + 'A');
                        }
                    }
                    robotLocs[depth] = charInCode;
                }
                keyPadInstructions.AddRange(keyPadInstructionsForThisInput);
            }

            keyPadInstructionsInputs = keyPadInstructions;
        }

        int parse = int.Parse(code.Trim('A'));
        int length = keyPadInstructionsInputs.MinBy(s => s.Length)!.Length;
        result += parse * length;
        Console.WriteLine($"{length}, {parse}");
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int result = 0;
    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
