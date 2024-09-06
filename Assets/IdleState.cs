using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class IdleState : StateMachineBehaviour
{
    public Transform CharTransform;
    public Character_TEST Char;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CharTransform = animator.GetComponent<Transform>();
        Char = animator.GetComponent<Character_TEST>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Vector3.Distance(CharTransform.position, Char.Enemy.position) <= 100)
        {
            animator.SetBool("isMove", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
