using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Test : MonoBehaviour
{
    [SerializeField] GameObject contentQuestions;
    [SerializeField] GameObject questionsUIObject;
    [SerializeField] GameObject nextBtn;
    [SerializeField] GameObject backBtn;
    Vector3 contentTransform;
    Vector3 contentbetween = new Vector3(0,26,0);
    [SerializeField] private int page = 0;
    [SerializeField]int pagelimit = 1;
    void Start()
    {
        contentTransform = contentQuestions.transform.position;
        setQuestion();
        backBtn.SetActive(false);

    }
    public void nextPage()
    {
        if (page < pagelimit)
        {
            page++;
            //Remove All Child Objects inside the content(As the previous page content still exists)
            foreach (Transform child in contentQuestions.transform)
            {
                Destroy(child.gameObject);
            }
            setQuestion();
            //Set "Next" Button and "Back" button to be visible or not depending on the current page
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
            setQuestion();
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


}


