//using System.Collections;
//using UnityEngine;
//using Spine.Unity;

//public class SpineAnimationBehaviour : StateMachineBehaviour
//{
//    public AnimationClip[] motion;
//    private string[] animationClips;
//    private bool loop;

//    [Header("Spine Motion Layer")]
//    public int layer = 0;
//    public float timeScale = 1.0f;

//    private SkeletonAnimation skeletonAnimation;
//    private Spine.AnimationState spineAnimationState;
//    private Spine.TrackEntry trackEntry;

//    private void Awake()
//    {
//        if (motion != null && motion.Length > 0)
//        {
//            animationClips = new string[motion.Length];
//        }
//        for(int i = 0; i < motion.Length; i++)
//        {
//            animationClips[i] = motion[i].name;
//            Debug.Log("AnimationClip : " + animationClips[i]);
//        }
//    }

//    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//    {
//        if(skeletonAnimation == null)
//        {
//            skeletonAnimation = animator.GetComponentInChildren<SkeletonAnimation>();
//            spineAnimationState = skeletonAnimation.state;
//        }

//        if(animationClips != null && animationClips.Length > 0)
//        {
//            int animationIndex = GetAnimationIndexFromInput(animator);
//            loop = stateInfo.loop;

//            trackEntry = spineAnimationState.SetAnimation(layer, animationClips[animationIndex], loop);
//            trackEntry.TimeScale = timeScale;
//        }
//    }

//    private int GetAnimationIndexFromInput(Animator animator)
//    {
//        float input = animator.GetFloat("InputDirection");
//        if(input > 0)
//        {
//            return 0;
//        }
//        else
//        {
//            return 1;
//        }

//    }
//}
