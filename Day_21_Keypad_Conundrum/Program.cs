using AdventOfCodeUtilities;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading.Channels;

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

// I don't really understand why it's better to do left, then vertical, then right (unless not allowed by the gap)

Dictionary<(char, char), string> numericalKeyPadInputsToInstructions = new();
foreach ((char c1, var coord1) in keyPadInputToCoord)
{
    foreach ((char c2, var coord2) in keyPadInputToCoord)
    {
        /*
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
            instructions.Add(horizontalDifference + verticalDifference + 'A');
        }
        else if (coord1.x >= 1 && coord1.y == 3 && coord2.x == 0 && coord2.y <= 2)
        {
            instructions.Add(verticalDifference + horizontalDifference + 'A');
        }
        else
        {
            instructions.Add(verticalDifference + horizontalDifference + 'A');
            if (horizontalDifference != "" && verticalDifference != "")
                instructions.Add(horizontalDifference + verticalDifference + 'A');
        }
        numericalKeyPadInputsToInstructions[(c1, c2)] = instructions;
        */
        /*
        if (coord2.x < coord1.x)
        {
            if (coord2.y > coord1.y)
                instructions.Add(new string('<', coord1.x - coord2.x) + new string('v', coord2.y - coord1.y) + 'A');
            else if (coord2.y < coord1.y)
                instructions.Add(new string('<', coord1.x - coord2.x) + new string('^', coord1.y - coord2.y) + 'A');
        }
        else if (coord2.x > coord1.x)
        {
            if (coord2.y > coord1.y)
                instructions.Add(new string('v', coord2.y - coord1.y) + new string('>', coord2.x - coord1.x) + 'A');
            else if (coord2.y < coord1.y)
                instructions.Add(new string('^', coord1.y - coord2.y) + new string('>', coord2.x - coord1.x) + 'A');
        }
        */
        string leftInstructions = new string('<', Math.Max(0, coord1.x - coord2.x));
        string rightInstructions = new string('>', Math.Max(0, coord2.x - coord1.x));
        string upInstructions = new string('^', Math.Max(0, coord1.y - coord2.y));
        string downInstructions = new string('v', Math.Max(0, coord2.y - coord1.y));
        string instructions = leftInstructions + downInstructions + upInstructions + rightInstructions;
        if ((coord2.x == 0 && coord1.y == 3) || (coord1.x == 0 && coord2.y == 3))
        {
            char[] chars = instructions.ToCharArray();
            Array.Reverse(chars);
            instructions = new string(chars);
        }
        instructions = instructions + 'A';
        numericalKeyPadInputsToInstructions[(c1, c2)] = instructions;
    }
}

/*
Dictionary<(char, char), List<string>> directionalKeyPadInputsToInstructions = new()
{

    { ('<', '<'), new List<string>() { "A" } },
    { ('<', 'v'), new List<string>() { ">A" } },
    { ('<', '>'), new List<string>() { ">>A" } },
    { ('<', '^'), new List<string>() { ">^A" } },
    { ('<', 'A'), new List<string>() { ">>^A" } }, // Don't bother with sequences that involve changing direction in middle, they won't be ideal

    { ('v', 'v'), new List<string>() { "A" } },
    { ('v', '<'), new List<string>() { "<A" } },
    { ('v', '>'), new List<string>() { ">A" } },
    { ('v', '^'), new List<string>() { "^A" } },
    { ('v', 'A'), new List<string>() { "^>A", ">^A" } },

    { ('>', '>'), new List<string>() { "A" } },
    { ('>', '<'), new List<string>() { "<<A" } },
    { ('>', 'v'), new List<string>() { "<A" } },
    { ('>', '^'), new List<string>() { "<^A", "^<A" } },
    { ('>', 'A'), new List<string>() { "^A" } },

    { ('^', '^'), new List<string>() { "A" } },
    { ('^', '<'), new List<string>() { "v<A" } },
    { ('^', 'v'), new List<string>() { "vA" } },
    { ('^', '>'), new List<string>() { "v>A", ">vA" } },
    { ('^', 'A'), new List<string>() { ">A" } },

    { ('A', 'A'), new List<string>() { "A" } },
    { ('A', '<'), new List<string>() { "v<<A" } }, // Don't bother with sequences that involve changing direction in middle, they won't be ideal
    { ('A', 'v'), new List<string>() { "v<A", "<vA" } },
    { ('A', '>'), new List<string>() { "vA" } },
    { ('A', '^'), new List<string>() { "<A" } },
};
*/

Dictionary<(char, char), string> directionalKeyPadInputsToInstructions = new();
foreach ((char c1, var coord1) in directionalKeyPadInputToCoord)
{
    foreach ((char c2, var coord2) in directionalKeyPadInputToCoord)
    {
        /*
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
        */

        string leftInstructions = new string('<', Math.Max(0, coord1.x - coord2.x));
        string rightInstructions = new string('>', Math.Max(0, coord2.x - coord1.x));
        string upInstructions = new string('^', Math.Max(0, coord1.y - coord2.y));
        string downInstructions = new string('v', Math.Max(0, coord2.y - coord1.y));
        string instructions = leftInstructions + downInstructions + upInstructions + rightInstructions;
        if ((coord2.x == 0 && coord1.y == 0) || (coord1.x == 0 && coord2.y == 0))
        {
            char[] chars = instructions.ToCharArray();
            Array.Reverse(chars);
            instructions = new string(chars);
        }
        instructions = instructions + 'A';
        directionalKeyPadInputsToInstructions[(c1, c2)] = instructions;
    }
}

List<string> recurse(List<string> inputs, char[] robotLocs, int depth = 0)
{
    if (depth == robotLocs.Length)
    {
        return inputs;
    }
    else
    {
        Dictionary<(char, char), string> inputsToInstructionsDictionaryToUse;
        if (depth == 0)
            inputsToInstructionsDictionaryToUse = numericalKeyPadInputsToInstructions;
        else
            inputsToInstructionsDictionaryToUse = directionalKeyPadInputsToInstructions;

        //Int64 minLengthOfNewInputs = Int64.MaxValue;
        //Int64 minChangesOfNewInputs = Int64.MaxValue;
        List<string> newInputs = new();
        foreach (string input in inputs)
        {
            List<string> newInputsFromThisInput = new() { "" };
            foreach (char charInCode in input)
            {
                List<string> possibleInstructions = new() { inputsToInstructionsDictionaryToUse[(robotLocs[depth], charInCode)] };
                if (possibleInstructions.Count == 1)
                {
                    string instructions = possibleInstructions[0];
                    for (int i = 0; i < newInputsFromThisInput.Count; i++)
                        newInputsFromThisInput[i] += instructions;
                }
                else
                {
                    int initialCount = newInputsFromThisInput.Count;
                    for (int i = 0; i < initialCount; i++)
                    {
                        string originalNewInput = newInputsFromThisInput[i];
                        newInputsFromThisInput[i] = originalNewInput + possibleInstructions[0];
                        // Add new ones for the others
                        for (int j = 1; j < possibleInstructions.Count; j++)
                        {
                            newInputsFromThisInput.Add(originalNewInput + possibleInstructions[j]);
                        }
                    }
                }
                robotLocs[depth] = charInCode;
            }

            /*
            Int64 minChangesOfNewInputsFromThisInput = Int64.MaxValue;
            foreach (string newInputFromThisInput in newInputsFromThisInput)
            {
                Int64 changes = 0;
                for (int i = 0; i < newInputFromThisInput.Length - 1; i++)
                {
                    if (newInputFromThisInput[i] != newInputFromThisInput[i + 1])
                        changes++;
                }
                if (changes < minChangesOfNewInputsFromThisInput) minChangesOfNewInputsFromThisInput = changes;
            }
            if (minChangesOfNewInputsFromThisInput < minChangesOfNewInputs)
            {
                minChangesOfNewInputs = minChangesOfNewInputsFromThisInput;
                newInputs = newInputsFromThisInput;
            }
            else if (minChangesOfNewInputsFromThisInput == minChangesOfNewInputs)
            {
                newInputs.AddRange(newInputsFromThisInput);
            }
            else
            {

            }
            */


            /*
            int lengthOfNewInputsFromThisInput = newInputsFromThisInput[0].Length;
            if (lengthOfNewInputsFromThisInput < minLengthOfNewInputs)
            {
                minLengthOfNewInputs = lengthOfNewInputsFromThisInput;
                newInputs = newInputsFromThisInput;
            }
            else if (lengthOfNewInputsFromThisInput == minLengthOfNewInputs)
            {
                newInputs.AddRange(newInputsFromThisInput);
            }
            else
            {

            }
            */

            newInputs.AddRange(newInputsFromThisInput);
        }
        return recurse(newInputs, robotLocs, depth + 1);
    }
}

Dictionary<(string, int), Int64> cache = new();
Int64 recurse2(string sequence, int robotsCount, int depth = 0)
{
    if (depth == robotsCount)
        return sequence.Length;
    else if (cache.TryGetValue((sequence, depth), out Int64 length))
        return length;
    else
    {
        Dictionary<(char, char), string> inputsToInstructionsDictionaryToUse;
        if (depth == 0)
            inputsToInstructionsDictionaryToUse = numericalKeyPadInputsToInstructions;
        else
            inputsToInstructionsDictionaryToUse = directionalKeyPadInputsToInstructions;

        Int64 totalLength = 0;
        for (int i = 0; i < sequence.Length; i++)
        {
            (char, char) pair;
            if (i == 0)
                pair = ('A', sequence[i]);
            else
                pair = (sequence[i - 1], sequence[i]);
            totalLength += recurse2(inputsToInstructionsDictionaryToUse[pair], robotsCount, depth + 1);
        }
        cache[(sequence, depth)] = totalLength;
        return totalLength;
    }
}

void P1()
{
    int result = 0;

    char[] robotLocs = new char[3];
    for (int i = 0; i < robotLocs.Length; i++)
        robotLocs[i] = 'A';

    foreach (string code in codes)
    {
        List<string> inputs = recurse(new() { code }, robotLocs);

        int parse = int.Parse(code.Trim('A'));
        int length = inputs.MinBy(s => s.Length)!.Length;
        result += parse * length;
        //Console.WriteLine($"{length}, {parse}");
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;

    char[] robotLocs = new char[3];
    for (int i = 0; i < robotLocs.Length; i++)
        robotLocs[i] = 'A';

    foreach (string code in codes)
    {
        Int64 length = recurse2(code, 26);

        int parse = int.Parse(code.Trim('A'));
        result += parse * length;
        //Console.WriteLine($"{length}, {parse}");
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();
