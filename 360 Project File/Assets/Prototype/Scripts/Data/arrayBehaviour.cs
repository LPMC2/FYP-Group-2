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
    public static T[,] Add2DArray<T>(T[,] arr, ArrayType arrayType = default)
    {
        int originalRowsize = arr.Length;
        int originalColsize = arr.GetLength(1);
        T[,] newArray = new T[originalRowsize, originalColsize];
        switch (arrayType)
        {
            case ArrayType.row:
                newArray = new T[originalRowsize + 1, originalColsize];
                break;
            case ArrayType.column:
                newArray = new T[originalRowsize, originalColsize + 1];
                break;
        }
        string debug = "List of array:\n";
        for (int i = 0; i < newArray.Length; i++)
        {
            for (int j = 0; j < newArray.GetLength(1); i++)
            {
                if (i < originalRowsize && j < originalColsize)
                {

                    newArray[i, j] = arr[i, j];
                }
                
            }

        }
        Debug.Log(debug);
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
    public static int[] BubbleSortArray(int[] originalArray)
    {
        int n = originalArray.Length;
        int[] newArray = originalArray;
        bool swapped;
        for(int i=0; i<n-1; i++)
        {
            swapped = false;
            for(int j=0; j<n-i-1; j++)
            {
                if(newArray[j] > newArray[j+1])
                {
                    int temp = newArray[j];
                    newArray[j] = newArray[j + 1];
                    newArray[j + 1] = temp;

                    swapped = true;
                }
            }
            if (!swapped)
                break;
        }

        return newArray;
    }
}
public enum ArrayType
{
    row,
    column

}