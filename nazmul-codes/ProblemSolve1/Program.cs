using System;

namespace TestPrograms
{
    class Program
    {
        public static void Main(string[] args)
        {
            var testCase = Convert.ToInt32(Console.ReadLine());

            while (testCase-- > 0)
            {
                var nm = Console.ReadLine();
                var data = nm.Split(' ');
                var n = int.Parse(data[0]);
                var m = int.Parse(data[1]);

                var ans = 0;
                var flag = false;
                for (var i = 0; i < n; i++)
                {
                    var s = Console.ReadLine();
                    if (m - s.Length >= 0 && flag == false)
                    {
                        m -= s.Length;
                        ans = i + 1;
                    }

                    else
                    {
                        flag = true;
                    }
                }
                
                Console.WriteLine(ans);
            }
        }
    }
}
