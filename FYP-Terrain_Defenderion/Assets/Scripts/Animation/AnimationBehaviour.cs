using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationBehaviour
{
    Animator animator;
    [SerializeField] private int animationLayer = 2;
    [SerializeField] private string[] playAttackAnimation;
    private int currentAttackAnimation = 0;
    [SerializeField] private float switchAnimationTime = 0.25f;
    // Start is called before the first frame update

    public void StartAnimationRandom(Animator animator, float speed)
    {
        if (playAttackAnimation.Length > currentAttackAnimation || playAttackAnimation.Length == 1)
        {
            float speedMultiplier = (1.0f / speed);
            if(HasParameter("SpeedMultiplier", animator))
                animator.SetFloat("SpeedMultiplier", speedMultiplier);
            if (animator.HasState(animationLayer, Animator.StringToHash(playAttackAnimation[currentAttackAnimation])))
                animator.CrossFade(playAttackAnimation[currentAttackAnimation], switchAnimationTime, animationLayer);
            currentAttackAnimation = UnityEngine.Random.Range(0, playAttackAnimation.Length);
            if (currentAttackAnimation >= playAttackAnimation.Length) currentAttackAnimation = 0;
        }
    }
    public void StartAnimationConstant(Animator animator, int animationId, float speed)
    {
        if (playAttackAnimation.Length > animationId || playAttackAnimation.Length == 1)
        {
            float speedMultiplier = (1.0f / speed);
            if (HasParameter("SpeedMultiplier", animator))
                animator.SetFloat("SpeedMultiplier", speedMultiplier);
            if (animator.HasState(animationLayer, Animator.StringToHash(playAttackAnimation[animationId])))
                animator.CrossFade(playAttackAnimation[animationId], switchAnimationTime, animationLayer);
        }
    }
    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
