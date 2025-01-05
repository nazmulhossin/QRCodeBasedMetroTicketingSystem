using System;

class Program
{
    static void Main()
    {
        string[] inputs = Console.ReadLine().Split();
        var nums = new List<int>();
        for (int i = 0; i < 3; i++) {
            int x = int.Parse(inputs[i]);
            nums.Add(x);
        }

        nums.Sort();
        bool ok = false;
        if (nums[0] == nums[1] && nums[1] == nums[2]) {
            ok = true;        
        } else if (nums[0] + nums[1] == nums[2]) {
            ok = true;
        }
        Console.WriteLine(ok ? "Yes" : "No");
    }
}
