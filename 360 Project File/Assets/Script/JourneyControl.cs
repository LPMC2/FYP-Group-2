using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyControl : MonoBehaviour
{

    // Update is called once per frame
    public RectTransform rectTransform;
    //public Vector2 newPosition;
    public bool is_up;
    public Vector2 targetPosition;
    public float moveSpeed = 100.0f;

    private bool isMoving = false;

    private void Start()
    {
        targetPosition.x = rectTransform.anchoredPosition.x;
        targetPosition.y = rectTransform.anchoredPosition.y;
    }

    public void ChangePanelPosition()
    {
        isMoving = true;
        AddPosition();
    }
    public void Update()
    {
        if (isMoving)
        {
            float step = moveSpeed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
        }

    }
    public void AddPosition()
    {
        if (is_up)
        {
            targetPosition.y = rectTransform.anchoredPosition.y - 125;
        }
        else
        {
            targetPosition.y = rectTransform.anchoredPosition.y + 125;
        }
    }

}
