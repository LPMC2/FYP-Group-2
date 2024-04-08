using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransparentUIAnimationBehaviour : ObjectAnimationCoroutineBehaviour<float>
{
    [SerializeField] private CanvasGroup canvasGroup;
    public override void StartAnimation()
    {
        base.StartAnimation();
    }
    // Update is called once per frame
    void Update()
    {
        if(coroutine != null && canvasGroup != null)
        {
            canvasGroup.alpha = CurrentValue;
        }
    }
}
