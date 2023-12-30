using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorBehaviour : PositionManager
{
    [Header("Door Settings")]
    [SerializeField] private Vector3 openedPosition = Vector3.zero;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveTime = 1f;
    [SerializeField] private DoorState doorState;
    private float moveTimer = 0f;
    [Header("Debug")]
    [SerializeField] private bool openDoor = false;
    private void Update()
    {
        if(openDoor)
        {
            StopAllCoroutines();
            ToggleDoorState();
            openDoor = false;
        }
    }
    public void ToggleDoorState()
    {
        ModifyDoorState((DoorState)(((int)doorState + 1) % 2));
    }
    public void SetDoorState(bool state)
    {
        if(state)
        {
            ModifyDoorState(DoorState.Open);
        } else
        {
            ModifyDoorState(DoorState.Close);
        }
    }
    public void ModifyDoorState(DoorState state)
    {
        doorState = state;
        switch(doorState)
        {
            case DoorState.Close:
                StartDoorEnumerator(gameObject.transform.position, initialPos);
                break;
            case DoorState.Open:
                StartDoorEnumerator(gameObject.transform.position, openedPosition);
                break;
        }
    }
    public void StartDoorEnumerator(Vector3 startPos, Vector3 endPos)
    {
        moveTimer = 0f;
        StartCoroutine(DoorEnumerator(startPos, endPos));
    }
    private IEnumerator DoorEnumerator(Vector3 startPosition, Vector3 endPosition)
    {
        while (true)
        {
            moveTimer += Time.deltaTime * moveSpeed;
            float t = moveTimer / moveTime;
            gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            if(gameObject.transform.position == endPosition)
            {
                StopAllCoroutines();
            }
            Debug.Log("Test");
            yield return null;
        }
    }
    public enum DoorState
    {
        Close,
        Open
    }
}
