using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationBehaviour
{
    Animator animator;
    [SerializeField] private int animationLayer = 1;
    [SerializeField] private string[] playAttackAnimation;
    private int currentAttackAnimation = 0;
    // Start is called before the first frame update

    public void StartAnimation(Animator animator, float AttackSpeed)
    {
        if (playAttackAnimation.Length > currentAttackAnimation || playAttackAnimation.Length == 1)
        {
            float speedMultiplier = (1.0f / AttackSpeed);
            if(HasParameter("SpeedMultiplier", animator))
                animator.SetFloat("SpeedMultiplier", speedMultiplier);
            if (animator.HasState(animationLayer, Animator.StringToHash(playAttackAnimation[currentAttackAnimation])))
                animator.CrossFade(playAttackAnimation[currentAttackAnimation], 0.2f, animationLayer);
            currentAttackAnimation = UnityEngine.Random.Range(0, playAttackAnimation.Length);
            if (currentAttackAnimation >= playAttackAnimation.Length) currentAttackAnimation = 0;
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
