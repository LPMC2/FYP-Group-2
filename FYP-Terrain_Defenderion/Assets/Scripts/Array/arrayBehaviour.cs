using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrayBehaviour
{
    public static string[] addArray(string[] arr, string newValue)
    {
        int originalsize = arr.Length;
        string[] newArray = new string[originalsize + 1];
        for (int i = 0; i < originalsize + 1; i++)
        {
            if (i < originalsize)
            {

                newArray[i] = arr[i];
            }
            else
            {
                newArray[i] = newValue;
            }

        }
        return newArray;
    }
    public static int[] addIntArray(int[] arr, int newValue)
    {
        int originalsize = arr.Length;
        int[] newArray = new int[originalsize + 1];
        for (int i = 0; i < originalsize + 1; i++)
        {
            if (i < originalsize)
            {

                newArray[i] = arr[i];
            }
            else
            {
                newArray[i] = newValue;
            }

        }
        return newArray;
    }
    public static T[] AddArray<T>(T[] arr, int amount = 1)
    {
        int originalsize = arr.Length;
        T[] newArray = new T[originalsize + amount];
        for (int i = 0; i < originalsize + amount; i++)
        {
            if (i < originalsize)
            {

                newArray[i] = arr[i];
            }

        }
        return newArray;
    }
    public static void DebugArray<T>(T[] arr)
    {
        string Content = "Content List: \n";
        int count = 1;
        foreach(T content in arr)
        {
            Content += " " + count + " > " + content + "\n";
            count++;
        }
        if(count <= 1)
        {
            Content += "None!";
        }
        Debug.Log(Content);
    }
    public static T[,] Add2DArray<T>(T[,] arr, ArrayType arrayType = default)
    {
        int originalRowsize = arr.Length;
        int originalColsize = arr.GetLength(1);
        T[,] newArray = default;
        switch(arrayType)
        {
            case ArrayType.row:
                newArray = new T[originalRowsize + 1, originalColsize];
                break;
            case ArrayType.column:
                newArray = new T[originalRowsize, originalColsize + 1];
                break;
        }
        for (int i = 0; i < originalRowsize + 1; i++)
        {
            for (int j = 0; j < originalColsize + 1; i++)
            {
                if (i < originalRowsize && j< originalColsize)
                {

                    newArray[i,j] = arr[i,j];
                }
            }

        }
        return newArray;
    }
    public static T[] ResetArray<T>(T[] originalArray)
    {
        T[] newArray = new T[1];
        newArray[0] = default(T);

        return newArray;
    }
    //public static string[,] Add2DArray(string[,] originalArray, string newValue)
    //{
    //    int rows = originalArray.GetLength(0);
    //    int columns = originalArray.GetLength(1) + 1;

    //    string[,] newArray = new string[rows, columns];

    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int col = 0; col < columns - 1; col++)
    //        {
    //            newArray[row, col] = originalArray[row, col];
    //        }

    //        newArray[row, columns - 1] = newValue;
    //    }

    //    return newArray;
    //}
}
public enum ArrayType
{
    row,
    column

}
