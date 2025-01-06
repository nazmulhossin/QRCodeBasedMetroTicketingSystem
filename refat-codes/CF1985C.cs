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
        string[] input = Console.ReadLine().Split(' ');

        int[] ar = new int[n];
        for(int i = 0; i < n; i++){
          ar[i] = Convert.ToInt32(input[i]);
        }

        long maxNumber = 0, totalSum = 0, count = 0;

        for(int i = 0; i < n; i++){
          totalSum += ar[i];
          if(ar[i] > maxNumber) maxNumber = ar[i];
          long remSum = totalSum - maxNumber;
          if(remSum == maxNumber) count++;
        }
        Console.WriteLine(count);
      }
    }
  }
}
