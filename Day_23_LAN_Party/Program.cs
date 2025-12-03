using AdventOfCodeUtilities;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

Dictionary<string, Computer> computersMap = new();
List<Computer> computers = new();

foreach (string connection in inputList)
{
    var split = connection.Split('-');
    foreach (string computerString in split)
    {
        if (!computersMap.ContainsKey(computerString))
        {
            computersMap[computerString] = new(computerString);
            computers.Add(computersMap[computerString]);
        }

        foreach (string otherComputerString in split)
        {
            if (!computersMap.ContainsKey(otherComputerString))
            {
                computersMap[otherComputerString] = new(otherComputerString);
                computers.Add(computersMap[otherComputerString]);
            }

            if (computerString != otherComputerString)
                computersMap[computerString].ConnectedComputers.Add(computersMap[otherComputerString]);
        }
    }
}

void P1()
{
    List<HashSet<Computer>> networks = new();

    for (int computer1Index = 0; computer1Index < computers.Count; computer1Index++)
    {
        Computer computer1 = computers[computer1Index];
        for (int computer2Index = computer1Index + 1; computer2Index < computers.Count; computer2Index++)
        {
            Computer computer2 = computers[computer2Index];
            for (int computer3Index = computer2Index + 1; computer3Index < computers.Count; computer3Index++)
            {
                Computer computer3 = computers[computer3Index];
                if (computer1.ConnectedComputers.Contains(computer2) && computer1.ConnectedComputers.Contains(computer3) &&
                    computer2.ConnectedComputers.Contains(computer1) && computer2.ConnectedComputers.Contains(computer3) &&
                    computer3.ConnectedComputers.Contains(computer1) && computer3.ConnectedComputers.Contains(computer2))
                {
                    HashSet<Computer> network = new() { computer1, computer2, computer3 };
                    networks.Add(network);
                }
            }
        }
    }

    int result = networks.Where(n => n.Any(c => c.Name.StartsWith('t'))).Count();

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    void recurse(HashSet<string> allNetworks, List<Computer> network)
    {
        string key = string.Join(',', network.OrderBy(c => c.Name));
        if (allNetworks.Contains(key))
            return;
        allNetworks.Add(key);

        HashSet<Computer> commonComputers = network[0].ConnectedComputers.Except(network).ToHashSet();
        for (int i = 1; i < network.Count; i++)
            commonComputers = commonComputers.Intersect(network[i].ConnectedComputers).ToHashSet();

        foreach (var commonComputer in commonComputers)
        {
            List<Computer> newNetwork = new(network);
            newNetwork.Add(commonComputer);
            recurse(allNetworks, newNetwork);
        }
    }

    HashSet<string> allNetworks = new();
    foreach (Computer computer in computers)
        recurse(allNetworks, new() { computer });

    Console.WriteLine(allNetworks.MaxBy(s => s.Split(',').Length));
    Console.ReadLine();
}

P1();
P2();

public class Computer
{
    public string Name;
    public HashSet<Computer> ConnectedComputers = new();

    public Computer(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}
