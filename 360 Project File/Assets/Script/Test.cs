using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    //This is two funtion need to use

    [SerializeField] int pagelimit = 1;
    int Min = 1;

    [SerializeField] GameObject contentQuestions;
    [SerializeField] GameObject questionsUIObject;
    [SerializeField] GameObject Tour360Button;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject backBtn;
    Vector3 contentTransform;
    Vector3 contentbetween = new Vector3(0,26,0);
    [SerializeField] private int page = 0;
    void Start()
    {
        contentTransform = contentQuestions.transform.position;
        SavePosition = rectTransform.anchoredPosition.x;
        targetPosition.x = (int)SavePosition;
        //setQuestion();
        backBtn.SetActive(false);

    }
    //public void nextPage()
    //{
    //    if (page < pagelimit)
    //    {
    //        page++;
    //        foreach (Transform child in contentQuestions.transform)
    //        {
    //            Destroy(child.gameObject);
    //        }
    //        setQuestion();
    //        if (page == pagelimit)
    //        {
    //            nextBtn.SetActive(false);
    //        }
    //        else
    //        {

    //            nextBtn.SetActive(true);
    //        }
    //        backBtn.SetActive(true);
    //    }
    //}
    //public void backPage()
    //{
    //    if (page > 0)
    //    {
    //        page--;
    //        foreach (Transform child in contentQuestions.transform)
    //        {
    //            Destroy(child.gameObject);
    //        }
    //        //setQuestion();
    //        if (page == 0)
    //        {
    //            backBtn.SetActive(false);
    //            nextBtn.SetActive(true);
    //        }
    //        else
    //        {
    //            nextBtn.SetActive(true);
    //        }
    //    }
    //}

    //public void setQuestion()
    //{
    //    //Add UI Object to the content
        

    //    for(int i = 0; i < pagelimit; i++)
    //    {
    //        GameObject target = Instantiate(questionsUIObject, contentTransform, Quaternion.identity);
    //        target.transform.SetParent(contentQuestions.transform);
            
    //    }

    //}

    //--------------------------------------------------------------------------------
    //There are make the scroll move
    // Update is called once per frame
    //cal the contect position (1314*1.2 + 100)*max-100= full size||fullsize/2 = middle ||
    public RectTransform rectTransform;
    JourneyControl control;
    //public Vector2 newPosition;
    public bool is_up;
    public Vector2 targetPosition;
    public float moveSpeed = 20.0f;
    [SerializeField]
    float SavePosition;

    public void ChangePanelPosition(bool is_up)
    {
        this.is_up = is_up;
        AddPosition(is_up);
        addpage();
        checkpage();
    }
    public void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        //if (page < pagelimit && page > Min || page == Min && !is_up || page == pagelimit && is_up)
        //{
            //rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);
        //}
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, step);

    }
    public void AddPosition(bool is_up)
    {
        float addnum = 1676.8f;
        if (is_up)
        {
            SavePosition += addnum;
        }
        else
        {
            SavePosition -= addnum;
        }

        targetPosition.x = (int)SavePosition;
    }
    public void changePointPosition(int a)
    {
        float addnum = 1676.8f;
        float fullsize = (addnum) * pagelimit - 100 + 402;
        page = a;
        SavePosition = 0;
        SavePosition -= addnum * (a-1);
        targetPosition.x = (int)SavePosition;
        //SavePosition +=201+ (addnum - 100) / 2;
        checkpage();
    }
    private void checkpage()
    {
        
        if (page == Min)
        {
            backBtn.SetActive(false);
            nextBtn.SetActive(true);
            Tour360Button.SetActive(false);
        }
        else if (page == pagelimit)
        {
            backBtn.SetActive(true);
            nextBtn.SetActive(false);
            Tour360Button.SetActive(true);
        }
        else
        {
            backBtn.SetActive(true);
            nextBtn.SetActive(true);
            Tour360Button.SetActive(false);
        }
    }
    public void addpage()
    {
        if (this.is_up)
        {
            page--;
        }
        else
        {
            page++;
        }
    }
}


