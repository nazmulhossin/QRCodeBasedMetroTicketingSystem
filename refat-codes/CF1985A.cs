using System;

namespace MyApplication
{
  class Program
  {
    static void Main(string[] args)
    {
      int cases = Convert.ToInt32(Console.ReadLine());
      for(int t = 1; t <= cases; t++){
        string line = Console.ReadLine();
        string[] parts = line.Split(' ');

        string a = parts[1][0] + parts[0].Substring(1,2);
        string b = parts[0][0] + parts[1].Substring(1,2);
        Console.WriteLine($"{a} {b}");
      }
    }
  }
}
