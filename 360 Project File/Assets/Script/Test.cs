using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    //This is two funtion need to use

    [SerializeField] int pagelimit = 1;


    [SerializeField] GameObject contentQuestions;
    [SerializeField] GameObject questionsUIObject;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject backBtn;
    Vector3 contentTransform;
    Vector3 contentbetween = new Vector3(0,26,0);
    [SerializeField] private int page = 0;
    void Start()
    {
        contentTransform = contentQuestions.transform.position;
        SavePosition = rectTransform.anchoredPosition.x;
        setQuestion();
        //backBtn.SetActive(false);
        //BackButton.SetActive(false);
        targetPosition.x = rectTransform.anchoredPosition.x;
        targetPosition.y = rectTransform.anchoredPosition.y;

    }
    public void nextPage()
    {
        if (page < pagelimit)
        {
            page++;
            foreach (Transform child in contentQuestions.transform)
            {
                Destroy(child.gameObject);
            }
            setQuestion();
            if (page == pagelimit)
            {
                nextBtn.SetActive(false);
            }
            else
            {

                nextBtn.SetActive(true);
            }
            backBtn.SetActive(true);
        }
    }
    public void backPage()
    {
        if (page > 0)
        {
            page--;
            foreach (Transform child in contentQuestions.transform)
            {
                Destroy(child.gameObject);
            }
            //setQuestion();
            if (page == 0)
            {
                backBtn.SetActive(false);
                nextBtn.SetActive(true);
            }
            else
            {
                nextBtn.SetActive(true);
            }
        }
    }

    public void setQuestion()
    {
        //Add UI Object to the content
        

        for(int i = 0; i < pagelimit; i++)
        {
            GameObject target = Instantiate(questionsUIObject, contentTransform, Quaternion.identity);
            target.transform.SetParent(contentQuestions.transform);
            
        }

    }

    //--------------------------------------------------------------------------------
    //There are make the scroll move
    // Update is called once per frame
    //cal the contect position (1314*1.2 + 100)*max-100= full size||fullsize/2 = middle ||
    public RectTransform rectTransform;
    [SerializeField] GameObject nextButton;
    [SerializeField] GameObject BackButton;
    int Min = 0;
    JourneyControl control;
    //public Vector2 newPosition;
    public bool is_up;
    public Vector2 targetPosition;
    public float moveSpeed = 100.0f;
    [SerializeField]
    double SavePosition;

    public void ChangePanelPosition(bool is_up)
    {
        this.is_up = is_up;
        AddPosition(is_up);
        checkpage();
    }
    public void Update()
    {
        float step = moveSpeed * Time.deltaTime;
        if (is_up)
        {
            if (page < pagelimit && page > Min || page == Min && !is_up || page == pagelimit && is_up)
            {
                rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
            }
        }




    }
    public void AddPosition(bool is_up)
    {
        double addnum = 1676.8;
        if (is_up)
        {
            SavePosition -= addnum;
        }
        else
        {
            SavePosition += addnum;
        }

        targetPosition.x = (int)SavePosition;
    }
    private void checkpage()
    {
        if (this.is_up)
        {
            page--;
        }
        else
        {
            page++;
        }
        if (page == Min)
        {
            BackButton.SetActive(false);
            nextButton.SetActive(true);
        }
        else if (page == pagelimit)
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


