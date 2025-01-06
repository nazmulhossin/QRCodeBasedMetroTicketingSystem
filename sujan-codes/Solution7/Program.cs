using System;

public class Program
{
    public static void Main(string[] args)
    {
        int N = int.Parse(Console.ReadLine());
        int[] H = new int[N];
        string[] parts = Console.ReadLine().Split();
        for (int i = 0; i < N; i++)
        {
            H[i] = int.Parse(parts[i]);
        }
        int[,] dp = new int[N, N];
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                dp[i, j] = 1;
            }
        }

        int ans = 1;

        for (int d = 1; d < N; d++)
        {
            for (int start = 0; start < N; start++)
            {
                for (int i = start + d; i < N; i += d)
                {
                    if (H[i] == H[i - d])
                    {
                        dp[i,d] = dp[i - d,d] + 1;
                        ans = Math.Max(ans, dp[i, d]);
                    }
                }
            }
        }
        Console.WriteLine(ans);
    }
}