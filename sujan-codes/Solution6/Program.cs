using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    public static void Main() 
    {
        string[] inputs = Console.ReadLine().Split();
        int a = int.Parse(inputs[0]);
        int b = int.Parse(inputs[1]);
        int c = int.Parse(inputs[2]);
        int d = int.Parse(inputs[3]);
        int e = int.Parse(inputs[4]);

        char[] s = { 'A', 'B', 'C', 'D', 'E' };
        List<(string name, int score)> scores = new List<(string, int)>();

        for (int i = 1; i < (1 << s.Length); i++) {
            string name = "";
            int score = 0;

            for (int j = 0; j < s.Length; j++) {
                if ((i & (1 << j)) != 0) {
                    name += s[j];
                    if (s[j] == 'A') score += a;
                    else if (s[j] == 'B') score += b;
                    else if (s[j] == 'C') score += c;
                    else if (s[j] == 'D') score += d;
                    else if (s[j] == 'E') score += e;
                }
            }

            scores.Add((name, score));
        }

        scores = scores
            .OrderByDescending(x => x.score)
            .ThenBy(x => x.name)
            .ToList();

        foreach (var (name, score) in scores) {
            Console.WriteLine($"{name}");
        }
    }
}
