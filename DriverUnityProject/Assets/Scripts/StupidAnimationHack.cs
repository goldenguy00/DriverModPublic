using UnityEngine;

public class StupidAnimationHack : MonoBehaviour
{
    private Animator animator;
    private bool hopoo;

    public void Start()
    {
        animator = this.GetComponent<Animator>();
        animator.SetLayerWeight(animator.GetLayerIndex("Body, Hammer"), 1);
        Invoke("PlayAnim", 1f);
        this.PlayAnimation("Gesture, Override", "BufferEmpty");
    }

    public void PlayAnim()
    {
        this.animator.SetBool("holding", false);
        PlayCrossfade("Gesture, Override", "ChargeSlash", "Slash.playbackRate", 0.3f, 0.1f);
        PlayCrossfade("Body", this.hopoo ? "JumpChargeHopoo" : "JumpCharge", "Jump.playbackRate", 0.65f, 0.1f);
        this.animator.SetFloat("airBlend", 1f);
        this.hopoo = !this.hopoo;
        Invoke("PlayAnim1", 0.65f);
    }
    public void PlayAnim1()
    {
        PlayAnimation("Body", "AscendDescend");
        this.animator.SetFloat("leapDir", 0);
        PlayAnimation("FullBody, Override", "ThrowSlashMax", "Slash.playbackRate", 1.6f);
        PlayCrossfade("Gesture, Override", "ThrowSlashMax", "Slash.playbackRate", 1.6f, 0.1f);
        Invoke("PlayAnim2", 3f);
    }
    public void PlayAnim2()
    {
        this.animator.SetFloat("airBlend", 0f);
        PlayAnimation("Body", "Sprint");
        PlayAnimation("Gesture, Override", "HoldSword");
        PlayCrossfade("FullBody, Override", "Slide", 0.1f);
        this.animator.SetBool("holding", true);
        Invoke("PlayAnim", 3f);
    }

    public void PlayAnimation(string layerName, string animationStateName)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        animator.speed = 1f;
        animator.Update(0f);
        animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
    }

    public void PlayAnimation(string layerName, string animationStateName, string animationParam, float duration)
    {
        int layerIndex = animator.GetLayerIndex(layerName);
        animator.SetFloat(animationParam, 1f);
        animator.PlayInFixedTime(animationStateName, layerIndex, 0f);
        animator.Update(0f);

        float length = animator.GetCurrentAnimatorStateInfo(layerIndex).length;
        animator.SetFloat(animationParam, length / duration);
    }

    public void PlayCrossfade(string layerName, string animationStateName, float crossfadeDuration)
    {
        animator.speed = 1f;
        animator.Update(0f);
        int layerIndex = animator.GetLayerIndex(layerName);
        animator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
    }

    public void PlayCrossfade(string layerName, string animationStateName, string playbackRateParam, float duration, float crossfadeDuration)
    {
        animator.speed = 1f;
        animator.Update(0f);
        int layerIndex = animator.GetLayerIndex(layerName);
        animator.SetFloat(playbackRateParam, 1f);
        animator.CrossFadeInFixedTime(animationStateName, crossfadeDuration, layerIndex);
        animator.Update(0f);
        float length = animator.GetNextAnimatorStateInfo(layerIndex).length;
        animator.SetFloat(playbackRateParam, length / duration);
    }
}
