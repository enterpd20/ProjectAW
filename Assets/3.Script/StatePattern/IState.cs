using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class PlayerController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle, walking;
    public string currentState;
    public float speed;
    public float movement;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentState = "Idle";
        SetCharacterState(currentState);
    }

    private void Update()
    {
        Move();
    }

    public void SetAnimation(AnimationReferenceAsset animation, bool loop, float timeScale)
    {
        skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timeScale;
    }

    public void SetCharacterState(string state)
    {
        if(state.Equals("Idle"))
        {
            SetAnimation(idle, true, 1f);
        }
        else if(state.Equals("walking"))
        {
            SetAnimation(walking, true, 1f);
        }
    }

    public void Move()
    {
        movement = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(movement * speed, rb.velocity.y);
        if(movement != 0)
        {
            SetCharacterState("walking");
        }
        else
        {
            SetCharacterState("Idle");
        }
    }
}
