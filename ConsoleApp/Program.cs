using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Algorithm;
using Chess;
using Games;

namespace ConsoleApp;

class Program
{
    static void Main(string[] args)
    {
        int n1 = 5;
        int k1 = 1;
        int[] bindings1 = { 0, 1, 2, 3 };
        string specials1 = "11100";
        Console.WriteLine(Olympics.SpecialWalk(n1, k1, bindings1, specials1));

        int n2 = 6;
        int k2 = 2;
        int[] bindings2 = { 0, 0, 0, 2, 2 };
        string specials2 = "000001";
        Console.WriteLine(Olympics.SpecialWalk(n2, k2, bindings2, specials2));

        int n3 = 9;
        int k3 = 4;
        int[] bindings3 = { 0, 0, 2, 2, 2, 4, 4, 5 };
        string specials3 = "000110010";
        Console.WriteLine(Olympics.SpecialWalk(n3, k3, bindings3, specials3));

        int n4 = 7;
        int k4 = 2;
        int[] bindings4 = { 0, 1, 0, 3, 1, 3 };
        string specials4 = "0000011";
        Console.WriteLine(Olympics.SpecialWalk(n4, k4, bindings4, specials4));

        int n5 = 10;
        int k5 = 3;
        int[] bindings5 = { 0, 1, 2, 3, 4, 4, 2, 7, 0 };
        string specials5 = "0000010100";
        Console.WriteLine(Olympics.SpecialWalk(n5, k5, bindings5, specials5));
    }
}
