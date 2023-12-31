using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DisplayManager : MonoBehaviour
{
    public Display[] displayList;
    public Display GetDisplay(string name)
    {
        foreach(Display display in displayList)
        {
            if (name == display.Name) 
                return display;
        }
        return null;
    }
}
[Serializable]
public class Display
{
    [SerializeField] private string name;
    public string Name { get { return name; } }
    [SerializeField] private DisplayBehaviour displayController;
    public DisplayBehaviour DisplayController { get { return displayController; } }
}