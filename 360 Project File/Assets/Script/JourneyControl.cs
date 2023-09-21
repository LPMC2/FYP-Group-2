using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JourneyControl : MonoBehaviour
{

    // Update is called once per frame
    public RectTransform rectTransform;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject BackButton;
    [SerializeField] int Max;
    int Min = 0;
    [SerializeField]int Journey = 0;

    //public Vector2 newPosition;
    public bool is_up;
    public Vector2 targetPosition;
    public float moveSpeed = 100.0f;

    private bool isMoving = false;

    private void Start()
    {
        BackButton.SetActive(false);
        targetPosition.x = rectTransform.anchoredPosition.x;
        targetPosition.y = rectTransform.anchoredPosition.y;
    }

    public void ChangePanelPosition(bool is_up)
    {
        this.is_up = is_up;
        isMoving = true;
        AddPosition(is_up);
        checkJourney();
    }
    public void Update()
    {
        if (isMoving)
        {
            if (Journey < Max || Journey > Min)
            {
                float step = moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
            }
            else if(Journey == Min || !is_up)
            {
                float step = moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
            }
            else if (Journey == Max || is_up)
            {
                float step = moveSpeed * Time.deltaTime;
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
            }
            if(targetPosition.y == rectTransform.anchoredPosition.y)
            {
                isMoving = !isMoving;
            }
        }

    }
    public void AddPosition(bool is_up)
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
    private void checkJourney()
    {
        if (this.is_up)
        {
            Journey--;
        }
        else
        {
            Journey++;
        }
        if (Journey == Min)
        {
            BackButton.SetActive(false);
            nextButton.SetActive(true);
        }
        else if(Journey == Max)
        {
            BackButton.SetActive(true);
            nextButton.SetActive(false);
        }
        else
        {
            BackButton.SetActive(true);
            nextButton.SetActive(true);
        }


    }
}
