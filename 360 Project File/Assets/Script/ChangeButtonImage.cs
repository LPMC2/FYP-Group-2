using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChangeButtonImage : MonoBehaviour
{
    [SerializeField] private Image OldImage;
    [SerializeField] private Sprite NewImage;

    public void ChangeImage()
    {
        OldImage.sprite = NewImage;
    }


}
