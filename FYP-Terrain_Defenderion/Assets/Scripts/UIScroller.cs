using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIScroller : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Vector2 scrollDirection;
    // Update is called once per frame
    void Start()
    {
        if(image == null)
        {
            image = gameObject.GetComponent<RawImage>();

        }
        
    }
    void Update()
    {
        image.uvRect = new Rect(image.uvRect.position + scrollDirection * Time.deltaTime, image.uvRect.size);

    }
}
