using System;

public class Program
{
    public static void Main() 
    {
        string[] inputs = Console.ReadLine().Split();
        int N = int.Parse(inputs[0]);
        char c1 = inputs[1][0];
        char c2 = inputs[2][0];
        string S = Console.ReadLine();
        char[] ans = S.ToCharArray();

        for (int i = 0; i < ans.Length; i++) {
            if (ans[i] != c1)
                ans[i] = c2;
        }
        
        Console.WriteLine(new string(ans));
    }
}