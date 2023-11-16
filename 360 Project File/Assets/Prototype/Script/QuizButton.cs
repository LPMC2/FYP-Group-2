using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class QuizButton : MonoBehaviour
{
    [SerializeField]
    private Image NowImage;
    [SerializeField]
    private Sprite NoChoose;
    [SerializeField]
    private Sprite HasChoose;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Choose()
    {
        NowImage.sprite = HasChoose;

    }
    public void deleteChoose()
    {
        NowImage.sprite = NoChoose;

    }
}
