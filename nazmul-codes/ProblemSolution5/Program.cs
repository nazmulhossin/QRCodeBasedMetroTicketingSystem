using System;
using System.Linq;

public class Program
{
    static void Main()
    {
        int n = Convert.ToInt32(Console.ReadLine());
        int[] arr = Console.ReadLine().Split().Select(int.Parse).ToArray();
        
        long[] dp = new long[100001];
        for (int i = 0; i < n; i++)
        {
            dp[arr[i]] += arr[i];
        }
        
        long ans = dp[1];
        for(int i = 2; i <= 100000; i++)
        {
            dp[i] = dp[i] + dp[i-2];
            ans = Math.Max(ans, dp[i]);
            dp[i] = Math.Max(dp[i], dp[i-1]);
        }
 
        Console.WriteLine(ans);
    }
}