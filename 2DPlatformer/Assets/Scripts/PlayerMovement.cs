using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D RB { get; private set; }

    [Header("Stats")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float jumpForce = 16f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.2f;
    private Vector2 wallJumpingPower = new Vector2(10f, 16f);

    [Space]
    [Header("Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool isFacingRight = true;

    private void Awake() {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        float dirX = Input.GetAxis("Horizontal");
        float dirY = Input.GetAxis("Vertical");
        Vector2 dir = new Vector2(dirX, dirY);

        if (RB.velocity.magnitude > maxSpeed)
            RB.velocity = Vector2.ClampMagnitude(RB.velocity, maxSpeed);

        if (isDashing)
            return;

        if (IsGrounded()) {
            coyoteTimeCounter = coyoteTime;
            canDash = true;
        } else
            coyoteTimeCounter -= Time.deltaTime;

        if (!isWallJumping)
            Walk(dir);

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f) {
            if (coyoteTimeCounter > 0f) {
                Jump(Vector2.up);

                jumpBufferCounter = 0f;
            }
        }

        if (Input.GetButtonUp("Jump") && RB.velocity.y > 0f) {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
            StartCoroutine(Dash());

        WallSlide();
        WallJump();

        if (!isWallJumping)
            Flip();
    }

    private bool IsGrounded() {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private bool IsWalled() {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    private void Flip() {
        if (isFacingRight && RB.velocity.x < 0f || !isFacingRight && RB.velocity.x > 0f) {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void WallSlide() {
        if (IsWalled() && !IsGrounded() && RB.velocity.x != 0f) {
            isWallSliding = true;
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Clamp(RB.velocity.y, -wallSlidingSpeed, float.MaxValue));
        } else
            isWallSliding = false;
    }

    private void WallJump() {
        if (isWallSliding) {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        } else
            wallJumpingCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f) {
            isWallJumping = true;
            RB.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection) {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping() {
        isWallJumping = false;
    }

    private IEnumerator Dash() {
        CinemachineShake.Instance.ShakeCamera(2, .1f);
        canDash = false;
        isDashing = true;
        float originalGravity = RB.gravityScale;
        RB.gravityScale = 0f;
        RB.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        RB.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
    }

    private void Jump(Vector2 dir) {
        RB.velocity = new Vector2(RB.velocity.x, 0);
        RB.velocity += dir * jumpForce;
    }

    private void Walk(Vector2 dir) {
        RB.velocity = new Vector2(dir.x * speed, RB.velocity.y);
    }
}
