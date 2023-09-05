using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControll : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private bool press = true;

    public void Play()
    {

            anim.Play("New Animation");



    }


    public void Close()
    {
     
            anim.Play("Close");
     

    }
    // Start is called before the first frame update
    void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
