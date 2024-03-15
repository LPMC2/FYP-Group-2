using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationBehaviour
{
    Animator animator;
    [SerializeField] private bool m_useAnimationRig = true;
    [SerializeField] private int animationLayer = 2;
    [SerializeField] private string[] playAttackAnimation;
    private int currentAttackAnimation = 0;
    [SerializeField] private float switchAnimationTime = 0.25f;
    // Start is called before the first frame update
    public bool UseAniRig { get { return m_useAnimationRig; } }
    public int GetAnimationLength()
    {
        return playAttackAnimation.Length;
    }
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
    public void ResetSpeed(Animator animator)
    {
        if (HasParameter("SpeedMultiplier", animator))
            animator.SetFloat("SpeedMultiplier", 1f);
    }
    public IEnumerator DelayStartAnimationConstant(float delayTime, Animator animator, int animationId, float speed)
    {
        yield return new WaitForSeconds(delayTime);
        StartAnimationConstant(animator, animationId, speed);
    }
    public void StartAnimationConstant(Animator animator, int animationId, float speed)
    {
        if (animator == null || animationId == -1) return;
        if (playAttackAnimation.Length > animationId || playAttackAnimation.Length == 1)
        {
            Debug.Log("original speed: " + speed + "\nnew speed: " + (1.0f / speed));
            float speedMultiplier = (1.0f / speed);
            if (HasParameter("SpeedMultiplier", animator))
                animator.SetFloat("SpeedMultiplier", speedMultiplier);
            if (animator.HasState(animationLayer, Animator.StringToHash(playAttackAnimation[animationId])))
                animator.CrossFade(playAttackAnimation[animationId], switchAnimationTime, animationLayer);
        }
    }
    public static bool HasParameter(string paramName, Animator animator)
    {
        if(animator != null)
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
