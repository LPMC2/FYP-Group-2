using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimationCoroutineBehaviour : ObjectAnimationCoroutineBehaviour<Vector3>
{

    public override void StartAnimation()
    {
        base.StartAnimation();

    }
    public void Update()
    {
        if(coroutine != null)
        {
            gameObject.transform.localScale = CurrentValue;
        }
    }
}
