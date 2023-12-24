using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingbar : MonoBehaviour {

    private RectTransform rectComponent;
    private Image imageComp;
    public float speed = 0.0f;
    private RotateState rotateState = RotateState.clockwise;

    // Use this for initialization
    void Start () {
        rectComponent = GetComponent<RectTransform>();
        imageComp = rectComponent.GetComponent<Image>();
        imageComp.fillAmount = 0.0f;
    }

    void Update()
    {
        if (rotateState == RotateState.clockwise)
        {
            imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * speed;
        }
        if (rotateState == RotateState.antiClockwise)
        {
            imageComp.fillAmount = imageComp.fillAmount + Time.deltaTime * -speed;
        }
        if (imageComp.fillAmount >= 1f)
        {
            rotateState = RotateState.antiClockwise;
            
        }
        else if(imageComp.fillAmount <= 0f)
        {
            rotateState = RotateState.clockwise;
            
        }

    }
    public enum RotateState
    {
        clockwise,
        antiClockwise
    }
}
