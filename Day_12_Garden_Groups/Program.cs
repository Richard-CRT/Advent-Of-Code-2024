using AdventOfCodeUtilities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

List<string> inputList = AoC.GetInputLines();

List<Region> regions = new List<Region>();
Dictionary<(int, int), Region> locationToRegionMap = new();

int height = inputList.Count;
int width = inputList[0].Length;

void P12()
{
    List<(int, int)> deltas = new List<(int, int)>() { (0, -1), (1, 0), (0, 1), (-1, 0) };
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            (int x, int y) initialLocation = (x, y);
            if (!locationToRegionMap.ContainsKey(initialLocation))
            {
                Region region = new();
                regions.Add(region);

                char plant = inputList[y][x];

                Queue<(int x, int y)> locationsInRegion = new();

                locationsInRegion.Enqueue(initialLocation);
                region.Locations.Add(initialLocation);

                while (locationsInRegion.Count > 0)
                {
                    var locationInRegion = locationsInRegion.Dequeue();
                    locationToRegionMap[locationInRegion] = region;

                    foreach ((int x, int y) d in deltas)
                    {
                        (int x, int y) targetLocation = (locationInRegion.x + d.x, locationInRegion.y + d.y);
                        if (targetLocation.x >= 0 && targetLocation.x < width && targetLocation.y >= 0 && targetLocation.y < height)
                        {
                            if (inputList[targetLocation.y][targetLocation.x] == plant)
                            {
                                if (!region.Locations.Contains(targetLocation))
                                {
                                    region.Locations.Add(targetLocation);
                                    locationsInRegion.Enqueue(targetLocation);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    Console.WriteLine(regions.Sum(r => r.Area * r.Perimeter));
    Console.ReadLine();

    Console.WriteLine(regions.Sum(r => r.Area * r.Sides));
    Console.ReadLine();
}

P12();

public class Region
{
    public HashSet<(int x, int y)> Locations = new();

    public int Area
    {
        get { return Locations.Count; }
    }

    public int Perimeter
    {
        get
        {
            int perimeter = 0;
            foreach (var location in Locations)
            {
                if (!Locations.Contains((location.x, location.y - 1))) perimeter++;
                if (!Locations.Contains((location.x + 1, location.y))) perimeter++;
                if (!Locations.Contains((location.x, location.y + 1))) perimeter++;
                if (!Locations.Contains((location.x - 1, location.y))) perimeter++;
            }
            return perimeter;
        }
    }

    public int Sides
    {
        get
        {
            List<((int x, int y), Edge)> edges = new();
            foreach (var location in Locations)
            {
                if (!Locations.Contains((location.x, location.y - 1))) edges.Add((location,Edge.up));
                if (!Locations.Contains((location.x + 1, location.y))) edges.Add((location, Edge.right));
                if (!Locations.Contains((location.x, location.y + 1))) edges.Add((location, Edge.down));
                if (!Locations.Contains((location.x - 1, location.y))) edges.Add((location, Edge.left));
            }

            List<List<((int x, int y), Edge)>> sides = new();
            foreach (var (edgeLoc, edge) in edges)
            {
                HashSet<List<((int x, int y), Edge)>> matchingSides = new();
                foreach (var side in sides)
                {
                    foreach (var (edgeInSideLoc, edgeInSideEdge) in side)
                    {
                        if ((edgeInSideEdge == Edge.up && edge == Edge.up) ||
                            (edgeInSideEdge == Edge.down && edge == Edge.down))
                        {
                            if (edgeInSideLoc.y == edgeLoc.y && (edgeInSideLoc.x - 1 == edgeLoc.x || edgeInSideLoc.x + 1 == edgeLoc.x))
                            {
                                matchingSides.Add(side);
                            }
                        }
                        else if ((edgeInSideEdge == Edge.left && edge == Edge.left) ||
                            (edgeInSideEdge == Edge.right && edge == Edge.right))
                        {
                            if (edgeInSideLoc.x == edgeLoc.x && (edgeInSideLoc.y - 1 == edgeLoc.y || edgeInSideLoc.y + 1 == edgeLoc.y))
                            {
                                matchingSides.Add(side);
                            }
                        }
                    }
                }

                if (matchingSides.Count == 0)
                {
                    List<((int x, int y), Edge)> newSide = new() { (edgeLoc, edge) };
                    sides.Add(newSide);
                }
                else if (matchingSides.Count == 1)
                {
                    matchingSides.First().Add((edgeLoc, edge));
                }
                else
                {
                    // If we pick up an edge between 2 other edges that have previously been given a side, we need to combine the sides
                    Debug.Assert(matchingSides.Count == 2);
                    var matchingSide = matchingSides.First();
                    matchingSide.Add((edgeLoc, edge));
                    var matchingSideToBeRemoved = matchingSides.Last();
                    foreach (var edgeDefToBeMoved in matchingSideToBeRemoved)
                    {
                        matchingSide.Add(edgeDefToBeMoved);
                    }
                    sides.Remove(matchingSideToBeRemoved);
                }
            }

            return sides.Count;
        }
    }
}

public enum Edge
{
    left, right, up, down
}
