using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

List<string> inputList = AoC.GetInputLines();

Dictionary<string, bool> startState = new();
Dictionary<string, Gate> gateByOutputKey = new();
List<Gate> gates = new();

int i;
for (i = 0; inputList[i] != ""; i++)
{
    var split = inputList[i].Split(':', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
    startState[split[0]] = split[1] == "1";
}
i++;
for (; i < inputList.Count; i++)
{
    Gate gate = new(inputList[i]);
    gates.Add(gate);
    gateByOutputKey[gate.OutputKey] = gate;
}

foreach (Gate gate in gates)
{
    gateByOutputKey.TryGetValue(gate.Input1Key, out gate.Input1Gate);
    gateByOutputKey.TryGetValue(gate.Input2Key, out gate.Input2Gate);

    if (startState.TryGetValue(gate.Input1Key, out bool value1)) gate.Input1Literal = value1;
    if (startState.TryGetValue(gate.Input2Key, out bool value2)) gate.Input2Literal = value2;
}

bool anyNotComputed = true;
while (anyNotComputed)
{
    anyNotComputed = false;
    foreach (Gate gate in gates)
    {
        if (!gate.CheckCompute())
            anyNotComputed = true;
    }
}

void P1()
{
    Int64 result = 0;

    var outputGates = gates.Where(g => g.OutputKey.StartsWith('z')).OrderByDescending(g => g.OutputKey).ToList();

    string outputBinary = new string(outputGates.Select(g => g.Output!.Value ? '1' : '0').ToArray());
    result = Convert.ToInt64(outputBinary, 2);

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    int inputBitcount = startState.Where(kvp => kvp.Key.StartsWith('x')).OrderByDescending(kvp => kvp.Key).Select(kvp => kvp.Value ? '1' : '0').Count();

    // Try to find the fulladder

    // Gate 1: A ^ B = tmp1
    // Gate 2: A & B = tmp2
    // Gate 3: Cin ^ tmp1 = S
    // Gate 4: Cin & tmp1 = tmp3
    // Gate 5: tmp2 + tmp3 = Cout
    // tmp1, tmp2, tmp3 are entirely internal, so we can simplify it all down quite a bit

    List<FullAdder> fullAdders = new();

    List<Gate> unaccountedGates = new(gates);

    for (int bitNum = 1; bitNum < inputBitcount; bitNum++)
    {
        // x is always in first input key
        // y is always in second input key
        var gate1 = gates.FirstOrDefault(g => g!.Input1Key == $"x{bitNum:00}" && g.Input2Key == $"y{bitNum:00}" && g.Type == GateType.XOR, null);
        if (gate1 is not null)
        {
            string a = gate1.Input1Key;
            string b = gate1.Input1Key;
            string tmp1 = gate1.OutputKey;
            var gate2 = gates.FirstOrDefault(g => g!.Input1Key == $"x{bitNum:00}" && g.Input2Key == $"y{bitNum:00}" && g.Type == GateType.AND, null);
            if (gate2 is not null)
            {
                string tmp2 = gate2.OutputKey;
                var gate3 = gates.FirstOrDefault(g => g!.OutputKey == $"z{bitNum:00}" && (g.Input1Key == tmp1 || g.Input2Key == tmp1) && g.Type == GateType.XOR, null);
                if (gate3 is not null)
                {
                    string s = gate3.OutputKey;
                    string cin = gate3.Input1Key == tmp1 ? gate3.Input2Key : gate3.Input1Key;
                    var gate4 = gates.FirstOrDefault(g => ((g!.Input1Key == cin && g.Input2Key == tmp1) || (g.Input1Key == tmp1 && g.Input2Key == cin)) && g.Type == GateType.AND, null);
                    if (gate4 is not null)
                    {
                        string tmp3 = gate4.OutputKey;
                        var gate5 = gates.FirstOrDefault(g => ((g!.Input1Key == tmp2 && g.Input2Key == tmp3) || (g.Input1Key == tmp3 && g.Input2Key == tmp2)) && g.Type == GateType.OR, null);
                        if (gate5 is not null)
                        {
                            string cout = gate5.OutputKey;
                            FullAdder fullAdder = new(gate1, gate2, gate3, gate4, gate5, a, b, s, cin, cout, tmp1, tmp2, tmp3);
                            fullAdders.Add(fullAdder);
                            unaccountedGates.Remove(gate1);
                            unaccountedGates.Remove(gate2);
                            unaccountedGates.Remove(gate3);
                            unaccountedGates.Remove(gate4);
                            unaccountedGates.Remove(gate5);
                            Console.WriteLine(fullAdder);
                            continue;
                        }
                    }
                }
            }
        }
        Console.WriteLine("?");
    }

    Console.WriteLine();
    foreach (var gate in unaccountedGates)
        Console.WriteLine(gate);
    Console.ReadLine();
}

P1();
P2();

public enum GateType
{
    OR, AND, XOR
}

public class Gate
{
    public GateType Type;
    public string Input1Key;
    public string Input2Key;
    public string OutputKey;
    public Gate? Input1Gate = null;
    public Gate? Input2Gate = null;
    public bool? Input1Literal = null;
    public bool? Input2Literal = null;

    public bool? Output = null;

    public Gate(string line)
    {
        var split = line.Split(' ');
        Input1Key = split[0];
        Input2Key = split[2];
        if (Input1Key.StartsWith('y'))
        {
            string tmp = Input1Key;
            Input1Key = Input2Key;
            Input2Key = tmp;
        }
        OutputKey = split[4];
        switch (split[1])
        {
            case "AND": Type = GateType.AND; break;
            case "OR": Type = GateType.OR; break;
            case "XOR": Type = GateType.XOR; break;
            default: throw new FormatException();
        }
    }

    public bool CheckCompute()
    {
        bool? input1 = null;
        bool? input2 = null;

        if (Input1Literal is not null)
            input1 = Input1Literal;
        else if (Input1Gate!.Output is not null)
            input1 = Input1Gate!.Output;

        if (Input2Literal is not null)
            input2 = Input2Literal;
        else if (Input2Gate!.Output is not null)
            input2 = Input2Gate!.Output;

        if (input1 is not null && input2 is not null)
        {
            switch (Type)
            {
                case GateType.AND: Output = input1.Value && input2.Value; break;
                case GateType.OR: Output = input1.Value || input2.Value; break;
                case GateType.XOR: Output = input1.Value ^ input2.Value; break;
            }
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        return $"{Input1Key} {Type} {Input2Key} => {OutputKey}";
    }
}

public class FullAdder
{
    // Gate 1: A ^ B = tmp1
    // Gate 2: A & B = tmp2
    // Gate 3: Cin ^ tmp1 = S
    // Gate 4: Cin & tmp1 = tmp3
    // Gate 5: tmp2 + tmp3 = Cout

    public Gate Gate1;
    public Gate Gate2;
    public Gate Gate3;
    public Gate Gate4;
    public Gate Gate5;
    public string A;
    public string B;
    public string S;
    public string Cin;
    public string Cout;
    public string Tmp1;
    public string Tmp2;
    public string Tmp3;

    public FullAdder(Gate gate1, Gate gate2, Gate gate3, Gate gate4, Gate gate5, string a, string b, string s, string cin, string cout, string tmp1, string tmp2, string tmp3)
    {
        Gate1 = gate1;
        Gate2 = gate2;
        Gate3 = gate3;
        Gate4 = gate4;
        Gate5 = gate5;
        A = a;
        B = b;
        S = s;
        Cin = cin;
        Cout = cout;
        Tmp1 = tmp1;
        Tmp2 = tmp2;
        Tmp3 = tmp3;
    }

    public override string ToString()
    {
        return $"A:{A} B:{B} Cin:{Cin} -> S:{S} Cout:{Cout}";
    }
}
