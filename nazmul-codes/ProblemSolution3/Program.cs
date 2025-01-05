using System;
using System.Linq;

public class Program
{
    static string s;
    static void Main()
    {
        int testCase = Convert.ToInt32(Console.ReadLine());
 
        while (testCase-- > 0)
        {
            int n = Convert.ToInt32(Console.ReadLine());
            s = Console.ReadLine();
            s = "." + s;
            Console.WriteLine(MinimumMove(1, n, 'a'));
        }
    }
 
    static int MinimumMove(int left, int right, char c)
    {
        if(left == right)
        {
            if (s[left] == c)
                return 0;
            else
                return 1;
        }
 
        int mid = (left + right) / 2;
 
        int leftChangeMove = 0;
        for(int i = left; i <= mid; i++)
        {
            if(s[i] != c)
                leftChangeMove++;
        }
 
        int rightChangeMove = 0;
        for(int i = mid + 1; i <= right; i++)
        {
            if(s[i] != c)
                rightChangeMove++;
        }
 
        return Math.Min(leftChangeMove + MinimumMove(mid + 1, right, (char)(c + 1)), rightChangeMove + MinimumMove(left, mid, (char)(c + 1)));
    }
}
