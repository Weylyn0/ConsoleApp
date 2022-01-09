using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Algorithm;
using Chess;
using Games;

namespace ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        var rows = new string[]
        {
            "00000100000000",
            "00110101000100",
            "01100101011100",
            "11001101000110",
            "00000001000010",
        };
        var p = new PathFinding(rows);
        p.FindPath(0, 0, 11, 4);
        Console.WriteLine(p.GetPath());
    }
}
