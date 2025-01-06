using System;

namespace MyApplication
{
  class Program
  {
    static void Main(string[] args)
    {
      int cases = Convert.ToInt32(Console.ReadLine());
      while(cases-- > 0){
        string[] firstLine = Console.ReadLine().Split(' ');
        int row = Convert.ToInt32(firstLine[0]);
        int col = Convert.ToInt32(firstLine[1]);

        char[,] grid = new char[row,col];

        for(int i = 0; i < row; i++){
          string line = Console.ReadLine();
          for(int j = 0; j < col; j++){
            grid[i,j] = line[j];
          }
        }

        int ansRow = 0, ansCol = 0, mx = 0;

        for(int i = 0; i < row; i++){
          int count = 0;
          for(int j = 0; j < col; j++){
            if(grid[i,j] == '#') count++;
          }

          if(count > mx) {
            mx = count;
            ansRow = i+1;
          }
        }

        mx = 0;
        for(int j = 0; j < col; j++){
          int count = 0;
          for(int i = 0; i < row; i++){
            if(grid[i,j] == '#') count++;
          }
          if(count > mx) {
            mx = count;
            ansCol = j + 1;
          }
        }

        Console.WriteLine($"{ansRow} {ansCol}");
      }
    }
  }
}
