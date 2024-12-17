using AdventOfCodeUtilities;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
Int64 initialRegA = Int64.Parse(inputList[0].Split(' ').Last());
Int64 initialRegB = Int64.Parse(inputList[1].Split(' ').Last());
Int64 initialRegC = Int64.Parse(inputList[2].Split(' ').Last());

List<(byte, byte)> instructions = new();
var instructionSplit = inputList[4].Split(' ').Last().Split(',').Select(s => byte.Parse(s)).ToArray();
for (int i = 0; i < instructionSplit.Length; i += 2)
{
    instructions.Add((instructionSplit[i], instructionSplit[i + 1]));
}

void P1()
{
    string output;
    Int64 regA;
    Int64 regB;
    Int64 regC;

    regA = initialRegA;
    regB = initialRegB;
    regC = initialRegC;

    output = "";

    for (int pc = 0; pc < instructions.Count; pc++)
    {
        (byte opcode, byte operand) instruction = instructions[pc];

        Int64 comboValue;

        if (instruction.operand == 4)
            comboValue = regA;
        else if (instruction.operand == 5)
            comboValue = regB;
        else if (instruction.operand == 6)
            comboValue = regC;
        else
            comboValue = instruction.operand;

        switch (instruction.opcode)
        {
            case 0: Debug.Assert(comboValue <= int.MaxValue); regA = (Int64)(regA >> (int)comboValue); break;
            case 1: regB = regB ^ instruction.operand; break;
            case 2: regB = (comboValue & 0b111); break;
            case 3: if (regA != 0) { pc = instruction.operand; pc--; } break;
            case 4: regB = regB ^ regC; break;
            case 5: byte outputVal = (byte)(comboValue & 0b111); output += $"{outputVal},"; break;
            case 6: Debug.Assert(comboValue <= int.MaxValue); regB = (Int64)(regA >> (int)comboValue); break;
            case 7: Debug.Assert(comboValue <= int.MaxValue); regC = (Int64)(regA >> (int)comboValue); break;
        }
    }

    Console.WriteLine(output[..^1]);
    Console.ReadLine();
}

Int64 recurse(List<byte> outputs, int depth = 0, Int64 providedA = 0)
{
    var outputsRequired = outputs.GetRange(outputs.Count - 1 - depth, depth + 1);

    for (Int64 additionalA = 0; additionalA <= 0b111; additionalA++)
    {
        // For a given regA, simulate the full machine code to get the outputs
        // Could use our decompiled code here, but it runs fast enough with the generic interpreter
        // Still relies on the assumptions explained in P2() though

        List<byte> outputsAchieved = new();

        Int64 regA = providedA + additionalA;
        Int64 regB = initialRegB;
        Int64 regC = initialRegC;

        for (int pc = 0; pc < instructions.Count; pc++)
        {
            (byte opcode, byte operand) instruction = instructions[pc];

            Int64 comboValue;

            if (instruction.operand == 4)
                comboValue = regA;
            else if (instruction.operand == 5)
                comboValue = regB;
            else if (instruction.operand == 6)
                comboValue = regC;
            else
                comboValue = instruction.operand;

            switch (instruction.opcode)
            {
                case 0: Debug.Assert(comboValue <= int.MaxValue); regA = (Int64)(regA >> (int)comboValue); break;
                case 1: regB = regB ^ instruction.operand; break;
                case 2: regB = (comboValue & 0b111); break;
                case 3: if (regA != 0) { pc = instruction.operand; pc--; } break;
                case 4: regB = regB ^ regC; break;
                case 5: byte outputVal = (byte)(comboValue & 0b111); outputsAchieved.Add(outputVal); break;
                case 6: Debug.Assert(comboValue <= int.MaxValue); regB = (Int64)(regA >> (int)comboValue); break;
                case 7: Debug.Assert(comboValue <= int.MaxValue); regC = (Int64)(regA >> (int)comboValue); break;
            }
        }

        if (outputsRequired.SequenceEqual(outputsAchieved))
        {
            if (depth == outputs.Count - 1)
                // Found a solution!
                return providedA + additionalA;
            else
            {
                if (recurse(outputs, depth + 1, (providedA + additionalA) << 3) is Int64 v && v != -1) return v;
            }
        }
    }

    return -1;
}

void P2()
{
    // Decompiled input
    /*
    while (regA > 0)
    {
        regB = regA & 0b111;
        regB = regB ^ 4;
        // regB is at most 0b111 at this point
        regC = regA >> (int)regB;
        regB = regB ^ regC;
        regB = regB ^ 4;
        output += $"{regB & 0b111},";
        regA = regA >> 3;
    }
    */

    // Observations
    // regA changes by going right 3 each iteration
    // regB and regC depend entirely on regA for a given iteration
    // output depends on more than just the last 3 bits of regA

    // We can't quite solve 1 output at a time, because the output depends on more than just 3 bits
    // But we can do DFS with a solution for the last output, then try the 7 possibilities for the second last output,
    // if none found, go back to the last output, then try the 7 possibilities for the second last output
    // Result is that we can recursively go through, only trying 3 bit possibilities (7 possibilities) at once

    Int64 solution = recurse(instructionSplit.ToList());
    Console.WriteLine(solution);
    Console.ReadLine();
}

P1();
P2();
