using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    public Rigidbody2D player;
    public PlayerMovement movement;
    private enum MovementState { idle, running, jumping, falling };

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float dirX = Input.GetAxisRaw("Horizontal");

        MovementState state;

        if (!movement.isDashing)
        {
            if (dirX > 0f || dirX < 0f)
            {
                state = MovementState.running;
            }
            else
            {
                state = MovementState.idle;
            }
        }
        else
            state = MovementState.jumping;


        if (player.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (player.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }
}
