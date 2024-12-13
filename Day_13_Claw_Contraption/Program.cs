using AdventOfCodeUtilities;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();
List<Game> games = new();

for (int i = 0; i < inputList.Count; i += 4)
{
    var split = inputList[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    (int, int) buttonADelta = (int.Parse(split[2][2..^1]), int.Parse(split[3][2..]));
    split = inputList[i + 1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    (int, int) buttonBDelta = (int.Parse(split[2][2..^1]), int.Parse(split[3][2..]));
    split = inputList[i + 2].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    (int, int) prizeLocation = (int.Parse(split[1][2..^1]), int.Parse(split[2][2..]));
    games.Add(new(buttonADelta, buttonBDelta, prizeLocation));
}

void P1()
{
    Int64 result = 0;
    const int aCost = 3;
    const int bCost = 1;
    foreach (Game game in games)
    {
        bool possibleWin = false;
        Int64 minCost = Int64.MaxValue;
        for (int aCount = 0; aCount <= 100; aCount++)
        {
            for (int bCount = 0; bCount <= 100; bCount++)
            {
                (Int64 x, Int64 y) resultLocation = (
                    (game.ButtonADelta.x * (Int64)aCount) + (game.ButtonBDelta.x * (Int64)bCount),
                    (game.ButtonADelta.y * (Int64)aCount) + (game.ButtonBDelta.y * (Int64)bCount)
                    );
                if (resultLocation == game.PrizeLocation)
                {
                    minCost = Math.Min(((Int64)aCount * aCost) + ((Int64)bCount * bCost), minCost);
                    possibleWin = true;
                }
            }
        }
        if (possibleWin)
            result += minCost;
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

void P2()
{
    Int64 result = 0;
    const int aCost = 3;
    const int bCost = 1;
    foreach (Game game in games)
    {
        (Int64 x, Int64 y) target = (game.PrizeLocation.x + 10000000000000, game.PrizeLocation.y + 10000000000000);
        //(Int64 x, Int64 y) target = (game.PrizeLocation.x, game.PrizeLocation.y);

        if (game.ButtonADelta.x > game.ButtonBDelta.x && game.ButtonADelta.y > game.ButtonBDelta.y)
        {
            if (game.ButtonADelta.x % game.ButtonBDelta.x == 0 && game.ButtonADelta.y % game.ButtonBDelta.y == 0)
            {
                if (game.ButtonADelta.x / game.ButtonBDelta.x == game.ButtonADelta.y / game.ButtonBDelta.y)
                {
                    // Lines are the same
                    // Have tested the input doesn't actually do this, so I can't be bothered to figure it out
                    throw new NotImplementedException();
                }
            }
        }
        else if (game.ButtonBDelta.x > game.ButtonADelta.x && game.ButtonBDelta.y > game.ButtonADelta.y)
        {
            if (game.ButtonBDelta.x % game.ButtonADelta.x == 0 && game.ButtonBDelta.y % game.ButtonADelta.y == 0)
            {
                if (game.ButtonBDelta.x / game.ButtonADelta.x == game.ButtonBDelta.y / game.ButtonADelta.y)
                {
                    // Lines are the same
                    // Have tested the input doesn't actually do this, so I can't be bothered to figure it out
                    throw new NotImplementedException();
                }
            }
        }
        else if (game.ButtonADelta.x == game.ButtonBDelta.x
            && game.ButtonADelta.y == game.ButtonBDelta.y)
        {
            // Lines are the same
            // Have tested the input doesn't actually do this, so I can't be bothered to figure it out
            throw new NotImplementedException();
        }

        // I've rearranged this on paper externally
        // We have 2 simulataneously equations with 2 unknowns
        // After checking above that the lines aren't the same, there's only one solution for aCount and bCount
        // Need to remember this is advent of code, so integers only!
        Int64 bCountNumerator = (game.ButtonADelta.x * target.y) - (game.ButtonADelta.y * target.x);
        Int64 bCountDenominator = (game.ButtonADelta.x * game.ButtonBDelta.y) - (game.ButtonADelta.y * game.ButtonBDelta.x);

        if (bCountNumerator % bCountDenominator == 0)
        {
            Int64 bCount = bCountNumerator / bCountDenominator;
            (Int64 x, Int64 y) targetLessB = (target.x - (bCount * game.ButtonBDelta.x), target.y - (bCount * game.ButtonBDelta.y));
            // I was missing this stupid check to make sure it divides evenly to find aCount
            // So I was getting false positives where integer division was 'helpfully' rounding
            // Particularly annoyingly, I wasn't missing this check on the previous implementation attempts
            if (targetLessB.x % game.ButtonADelta.x == 0 && targetLessB.y % game.ButtonADelta.y == 0)
            {
                Int64 aCountX = targetLessB.x / game.ButtonADelta.x;
                Int64 aCountY = targetLessB.y / game.ButtonADelta.y;
                if (aCountX == aCountY)
                {
                    Int64 cost = ((Int64)aCountX * aCost) + ((Int64)bCount * bCost);
                    result += cost;
                }
            }
        }
    }

    Console.WriteLine(result);
    Console.ReadLine();
}

P1();
P2();

public class Game
{
    public (int x, int y) ButtonADelta;
    public (int x, int y) ButtonBDelta;
    public (int x, int y) PrizeLocation;

    public Game((int, int) buttonADelta, (int, int) buttonBDelta, (int, int) prizeLocation)
    {
        ButtonADelta = buttonADelta;
        ButtonBDelta = buttonBDelta;
        PrizeLocation = prizeLocation;
    }
}
