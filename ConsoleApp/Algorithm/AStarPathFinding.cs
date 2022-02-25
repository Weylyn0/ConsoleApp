using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Algorithm;

public class AStarPathFinding
{
    private readonly int Width;
    private readonly int Height;
    private readonly Node[,] Nodes;
    private readonly List<Node> OpenNodes;
    private readonly HashSet<Node> ClosedNodes;
    private readonly Stopwatch Stopwatch;

    private bool Found;
    private Node StartNode;
    private Node EndNode;

    public AStarPathFinding(params string[] rows)
    {
        if (rows.Length == 0)
            throw new ArgumentException($"{nameof(rows)} must contain at least 1 row");

        for (int i = 1; i < rows.Length; i++)
            if (rows[i].Length != rows[0].Length)
                throw new ArgumentException($"{nameof(i)}. row's length does not match with other rows");

        foreach (var row in rows)
            foreach (var c in row)
                if (c != '1' && c != '0')
                    throw new ArgumentException($"{nameof(rows)} can only contain 1 and 0");

        Width = rows[0].Length;
        Height = rows.Length;
        Nodes = new Node[Width, Height];
        OpenNodes = new List<Node>();
        ClosedNodes = new HashSet<Node>();
        Stopwatch = new Stopwatch();

        Found = false;
        StartNode = null;
        EndNode = null;

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Nodes[x, y] = new Node(x, y, rows[y][x] == '0');
    }

    public void FindPath(int startX, int startY, int endX, int endY)
    {
        OpenNodes.Clear();
        ClosedNodes.Clear();

        foreach (var node in Nodes)
            node.Reset();

        Found = false;
        StartNode = null;
        EndNode = null;

        if (startX < 0 || startX > Width - 1 || endX < 0 || endX > Width - 1)
            return;

        if (startY < 0 || startY > Height - 1 || endY < 0 || endY > Height - 1)
            return;

        Stopwatch.Restart();

        StartNode = Nodes[startX, startY];
        EndNode = Nodes[endX, endY];

        OpenNodes.Add(StartNode);

        while (OpenNodes.Count > 0)
        {
            var currentNode = OpenNodes[0];
            for (int i = 1; i < OpenNodes.Count; i++)
                if (OpenNodes[i].FCost < currentNode.FCost || (OpenNodes[i].FCost == currentNode.FCost && OpenNodes[i].HCost < currentNode.HCost))
                    currentNode = OpenNodes[i];

            OpenNodes.Remove(currentNode);
            ClosedNodes.Add(currentNode);

            if (currentNode == EndNode)
            {
                Stopwatch.Stop();
                Found = true;
                return;
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int newX = currentNode.X + x;
                    int newY = currentNode.Y + y;

                    if (newX < 0 || newX > Width - 1)
                        continue;

                    if (newY < 0 || newY > Height - 1)
                        continue;

                    var newNode = Nodes[newX, newY];
                    if (!newNode.Walkable || ClosedNodes.Contains(newNode))
                        continue;

                    int newCost = currentNode.GCost + GetDistance(currentNode, newNode);
                    if (newCost < newNode.GCost || !OpenNodes.Contains(newNode))
                    {
                        newNode.GCost = newCost;
                        newNode.HCost = GetDistance(newNode, EndNode);
                        newNode.Parent = currentNode;

                        if (!OpenNodes.Contains(newNode))
                            OpenNodes.Add(newNode);
                    }
                }
            }
        }
        Stopwatch.Reset();
    }

    public string GetPath()
    {
        if (!Found)
            return "No paths found to end";

        int move = 1;
        var path = $"[ {EndNode.X}, {EndNode.Y} ] END";
        var node = EndNode.Parent;
        while (node != StartNode)
        {
            path = $"[ {node.X}, {node.Y} ]\n{path}";
            node = node.Parent;
            move++;
        }
        path = $"[ {StartNode.X}, {StartNode.Y} ] START\n{path}\n\n  > {move} Moves\n  > Calculated in {Stopwatch.ElapsedMilliseconds} MS ({Stopwatch.ElapsedTicks} Ticks)";
        return path;
    }

    private int GetDistance(Node A, Node B)
    {
        int distanceX = Math.Abs(A.X - B.X);
        int distanceY = Math.Abs(A.Y - B.Y);
        return 14 * Math.Min(distanceX, distanceY) + 10 * Math.Abs(distanceX - distanceY);
    }
}

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Walkable { get; set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get => GCost + HCost; }
    public Node Parent { get; set; }

    public Node(int x, int y, bool walkable)
    {
        X = x;
        Y = y;
        Walkable = walkable;
    }

    public void Reset()
    {
        GCost = 0;
        HCost = 0;
        Parent = null;
    }
}
