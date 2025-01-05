using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        string[] inputs = Console.ReadLine().Split();
        int H = int.Parse(inputs[0]);
        int W = int.Parse(inputs[1]);
        int X = int.Parse(inputs[2]) - 1; 
        int Y = int.Parse(inputs[3]) - 1;

        List<string> S = new List<string>();
        for (int i = 0; i < H; i++) {
            S.Add(Console.ReadLine());
        }

        string T = Console.ReadLine();

        bool[,] vis = new bool[H, W];
        int cnt = 0;

        foreach (char c in T) {
            int a = X, b = Y;
            if (c == 'D') a++;
            else if (c == 'U') a--;
            else if (c == 'L') b--;
            else if (c == 'R') b++;

            if (a >= 0 && a < H && b >= 0 && b < W && S[a][b] != '#') {
                X = a;
                Y = b;
                if (!vis[a, b] && S[a][b] == '@') {
                    cnt++;
                    vis[a, b] = true;
                }
            }
        }

        Console.WriteLine($"{X + 1} {Y + 1} {cnt}");
    }
}
