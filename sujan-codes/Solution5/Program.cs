using System;

public class Program
{
    public static void Main() 
    {
        string[] inputs = Console.ReadLine().Split(); 
        int N = int.Parse(inputs[0]);
        int Rating = int.Parse(inputs[1]);

        for (int i = 0; i < N; i++) {
            string[] s = Console.ReadLine().Split(); 
            int D = int.Parse(s[0]);
            int A = int.Parse(s[1]);
            
            if (D == 2 && Rating >= 1200 && Rating <= 2399) {
                Rating += A;
            }
            else if (D == 1 && Rating >= 1600 && Rating <= 2799) {
                Rating += A;
            }
        }
        Console.WriteLine(Rating); 
    }
}
