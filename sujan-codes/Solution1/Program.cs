using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        string[] inputs = Console.ReadLine().Split();
        int n = int.Parse(inputs[0]);
        int m = int.Parse(inputs[1]);

        var v = new List<(int X, int Y, char C)>();
        for (int i = 0; i < m; i++)
        {
            string[] cell = Console.ReadLine().Split();
            int x = int.Parse(cell[0]);
            int y = int.Parse(cell[1]);
            char c = cell[2][0];
            v.Add((x, y, c));
        }

        v.Sort((a, b) => a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y));

        var set = new SortedSet<int>();
        bool ok = true;

        foreach (var t in v)
        {
            int x = t.X;
            int y = t.Y;
            char c = t.C;

            if (c == 'B')
            {
                var it = set.GetViewBetween(int.MinValue, y).Reverse().FirstOrDefault();
                if (it != 0)
                {
                    ok = false;
                    break;
                }
            }
            else
            {
                set.Add(y);
            }
        }

        Console.WriteLine(ok ? "Yes" : "No");
    }
}
