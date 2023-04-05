using System.Runtime.CompilerServices;

namespace CourseDB.WebAPI;

public class Tools
{
    public static string EliminateSubString(string str, string subStr)
    {
        string alteredString = null, temp;
        
        for (int x = 0; x < str.Length - subStr.Length; x++)
            if (str.Substring(x, subStr.Length) == subStr)
                x += subStr.Length;
            else
                alteredString += str[x];

        if ((temp = str.Substring(str.Length - subStr.Length, subStr.Length - 1)) != subStr)
            alteredString += temp;

        return alteredString;
    }
    
    public static int LinearSearch<T>(T val, T[] array)
    {
        for (int x = 0; x < array.Length; x++)
            if (val.Equals(array[x]))
                return x;
        
        return -1;
    }
} 