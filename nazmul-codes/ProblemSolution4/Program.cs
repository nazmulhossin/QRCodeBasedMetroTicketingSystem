using System;
using System.Linq;

public class Program
{
    static int[] arr = new int[100001];
    static int[] dp = new int[100001];
    
    static void Main()
    {
        int n = Convert.ToInt32(Console.ReadLine());
        int[] input = Console.ReadLine().Split().Select(int.Parse).ToArray();
        
        int maxLen = 0;
        for (int i = 0; i < n; i++)
        {
            dp[input[i]] = PrimeFactors(input[i]);
            maxLen = Math.Max(maxLen, dp[input[i]]);
        }
        
        Console.WriteLine(maxLen + 1);
    }
 
    static int PrimeFactors(int n)
    {
        int temp = n;
        int maxLen = 0;
        if (n % 2 == 0)
        {
            if (arr[2] > 0)
            {
                dp[temp] = dp[arr[2]] + 1;
                maxLen = Math.Max(maxLen, dp[temp]);
            }
 
            arr[2] = temp;
        }
 
        while (n % 2 == 0)
            n = n / 2;
 
        for (int i = 3; i <= Math.Sqrt(n); i = i + 2)
        {
            if (n % i == 0)
            {
                if (arr[i] > 0)
                {
                    dp[temp] = dp[arr[i]] + 1;
                    maxLen = Math.Max(maxLen, dp[temp]);
                }
 
                arr[i] = temp;
            }
 
            while(n % i == 0)
                n = n / i;
        }
 
        if (n > 2)
        {
            if(arr[n] > 0)
            {
                dp[temp] = dp[arr[n]] + 1;
                maxLen = Math.Max(maxLen, dp[temp]);
            }
 
            arr[n] = temp;
        }
 
        return maxLen;
    }
}