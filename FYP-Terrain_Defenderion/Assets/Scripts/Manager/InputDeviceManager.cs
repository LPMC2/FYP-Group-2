using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace InputDeviceControl.Manager
{
    public class InputDeviceManager
    {
        public static bool deviceState = true;
        public static void EnableAllDevice()
        {
            SetDevice(true);
            InputSystem.EnableDevice(UnityEngine.InputSystem.Keyboard.current);
        }
        public static void DisableAllDevice()
        {
            SetDevice(false);
            //InputSystem.DisableDevice(UnityEngine.InputSystem.Keyboard.current);
            //InputSystem.DisableDevice(UnityEngine.InputSystem.Mouse.current);
        }
        public static void SetDevice(bool state)
        {
            
            InputManager[] inputActions = GameObject.FindObjectsOfType<InputManager>();
            FlightController[] flightControllers = GameObject.FindObjectsOfType<FlightController>();
            InventoryBehaviour inventoryBehaviour = GameObject.FindObjectOfType<InventoryBehaviour>();
            if(inventoryBehaviour != null)
            {
                inventoryBehaviour.enabled = !state;
            }
            foreach(InputManager inputManager in inputActions)
            {
                inputManager.enabled = !state;
                deviceState = false;
            }
            foreach(FlightController flightController in flightControllers)
            {
                flightController.enabled = !state;
                deviceState = false;
            }
            if(inputActions.Length == 0 && flightControllers.Length == 0)
            {
                deviceState = true;
            }
            if (state)
            {
                //Debug.Log("Device State:" + deviceState);
                CursorBehaviour.StaticControlCursor(true);
            } else
            CursorBehaviour.StaticControlCursor(deviceState);
           
        }
        public static void ToggleDeviceState()
        {
            deviceState = !deviceState;
            if(deviceState)
            {
                EnableAllDevice();
            } else
            {
                DisableAllDevice();
            }
        }
    }
}
