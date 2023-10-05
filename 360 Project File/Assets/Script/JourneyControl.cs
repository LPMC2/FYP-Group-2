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

    private void Start()
    {
        BackButton.SetActive(false);
        targetPosition.x = rectTransform.anchoredPosition.x;
        targetPosition.y = rectTransform.anchoredPosition.y;
    }

    public void ChangePanelPosition(bool is_up)
    {
        this.is_up = is_up;
        AddPosition(is_up);
        checkJourney();
    }
    public void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        if (Journey < Max && Journey > Min || Journey == Min && !is_up || Journey == Max && is_up)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
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
