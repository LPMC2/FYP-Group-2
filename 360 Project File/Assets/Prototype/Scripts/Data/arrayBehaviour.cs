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
    public static T[] addArray<T>(T[] arr)
    {
        int originalsize = arr.Length;
        T[] newArray = new T[originalsize + 1];
        for (int i = 0; i <= originalsize; i++)
        {
            if (i < originalsize)
            {

                newArray[i] = arr[i];
            }
            else
            {

                newArray[i] = default;
            }
        }
        return newArray;
    }
    public static T[] RemoveArray<T>(T[] arr, int element)
    {
        int originalsize = arr.Length;
        T[] newArray = new T[originalsize - 1];
        int numArray = 0;
        for (int i = 0; i < originalsize; i++)
        {
            if (i != element)
            {

                newArray[numArray] = arr[i];
                numArray++;
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
    public static T[] RemoveArrayElement<T>(T[] originalArray, int rmId)
    {
        originalArray[rmId] = default(T);
        T[] newArray = new T[originalArray.Length - 1];
        for(int i=0; i<newArray.Length; i++)
        {
            if(i != rmId)
            {
                newArray[i] = originalArray[i];
            } else
            {
                if(i != originalArray.Length-1)
                newArray[i] = originalArray[i+1];
            }
        }
        return newArray;

    }

    public static string[,] Add2DArray(string[,] originalArray, string newValue)
    {
        int rows = originalArray.GetLength(0);
        int columns = originalArray.GetLength(1) + 1;

        string[,] newArray = new string[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns - 1; col++)
            {
                newArray[row, col] = originalArray[row, col];
            }

            newArray[row, columns - 1] = newValue;
        }

        return newArray;
    }
}
