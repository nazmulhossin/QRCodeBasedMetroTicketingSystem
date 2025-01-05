using System;
using System.Linq;

namespace TestPrograms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int testCase = Convert.ToInt32(Console.ReadLine());

            while (testCase-- > 0)
            {
                int n = Convert.ToInt32(Console.ReadLine());
                string input = Console.ReadLine();
                int[] arr = input.Split(' ').Select(int.Parse).ToArray();

                int ans = 0;
                for (int i = 0; i < n; i++)
                {
                    if (i % 2 == 0)
                        ans = Math.Max(ans, arr[i]);
                }

                Console.WriteLine(ans);
            }
        }
    }
}
