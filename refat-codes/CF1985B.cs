using System;

namespace MyApplication
{
  class Program
  {
    static void Main(string[] args)
    {
      int cases = Convert.ToInt32(Console.ReadLine());
      while(cases-- > 0){
        int n = Convert.ToInt32(Console.ReadLine());

        int maxSum = 0, ans = 0;
        for(int i = 2; i <= n; i++){
          int x = i, curSum = 0;
          while(x <= n){
            curSum += x;
            x += i;
          }
          if(curSum > maxSum){
            maxSum = curSum;
            ans = i;
          }
        }
        Console.WriteLine(ans);
      }
    }
  }
}
