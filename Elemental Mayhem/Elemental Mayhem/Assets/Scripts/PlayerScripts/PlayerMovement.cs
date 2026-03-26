using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public float acceleration = 60f;
    public float deceleration = 80f;
    public float airControlMultiplier = 0.6f;
    public bool canMove = true;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float gravityScale = 3f;
    public float fallMultiplier = 2.5f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Fast Fall")]
    public float fastFallSpeed = 20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Dash / Roll")]
    public bool hasDash = false;
    public bool hasRoll = false;

    public float dashSpeed = 15f;
    public float rollSpeed = 12f;

    private bool isDashing = false;
    private bool isRolling = false;

    private Rigidbody2D rb;
    public Animator animator;
    public PlayerCombat playerCombat;

    private float moveInput;
    [SerializeField]private float downInput;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private float currentVelocityX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;
    }

    private void Update()
    {
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        CheckGround();

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }

        HandleMovement();
        HandleBetterJump();
        HandleJump();
    }


    void UpdateAnimations()
    {
        if (rb.linearVelocityX != 0)
        {
            animator.SetFloat("HorizontalVelocity",1);
        }
        else
        {
            animator.SetFloat("HorizontalVelocity", 0);
        }

        if (rb.linearVelocityY < 0)
        {
            animator.SetFloat("VerticalVelocity", -1);
        }
        else if (rb.linearVelocityY > 0)
        {
            animator.SetFloat("VerticalVelocity", 1);
        }

        animator.SetBool("isGrounded", isGrounded);

    }

    public void MovementInput(InputAction.CallbackContext context)
    {
        moveInput = (context.ReadValue<Vector2>().x);
        downInput = (context.ReadValue<Vector2>().y);
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        TryDashOrRoll(isDashInput: true);
    }

    public void RollInput(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        TryDashOrRoll(isDashInput: false);
    }

    void HandleJump()
    {
        if (!canMove)
            return;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            coyoteTimeCounter = 0f;
            jumpBufferCounter = 0f;
        }
    }


    void HandleMovement()
    {
        if (!canMove)
            return;

        int moveInputFixed = 0;

        if (moveInput > 0)
        {
            moveInputFixed = 1;
        }
        else if (moveInput < 0)
        {
            moveInputFixed = -1;
        }

        float targetSpeed = moveInputFixed * moveSpeed;

        float accelRate = isGrounded ? acceleration : acceleration * airControlMultiplier;

        if (Mathf.Abs(targetSpeed) > 0.01f)
        {
            currentVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);
        }
        else
        {
            currentVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }

        rb.linearVelocity = new Vector2(currentVelocityX, rb.linearVelocity.y);

        if (moveInput > 0f)
        {
            transform.localScale = new Vector3(1,1,1);
        }
        else if (moveInput < 0f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        animator.SetTrigger("Jump");
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        StartCoroutine(ClearJumpTrigger());
    }

    IEnumerator ClearJumpTrigger()
    {
        yield return new WaitForSeconds(jumpBufferTime);
        animator.ResetTrigger("Jump");
    }

    void HandleBetterJump()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        if (!isGrounded && downInput < -0.5f)
        {
            Debug.Log("Falling fast");
            //float fastFallSpeed = 20f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    void TryDashOrRoll(bool isDashInput)
    {
        // Prevent overlap with other states
        if (!isGrounded || isDashing || isRolling || playerCombat.IsAttacking())
            return;

        if (isDashInput)
        {
            if (hasDash)
                StartDash();
            else if (hasRoll)
                StartRoll(); // fallback
        }
        else
        {
            if (hasRoll)
                StartRoll();
            else if (hasDash)
                StartDash(); // fallback
        }
    }

    void StartDash()
    {
        isDashing = true;
        canMove = false;

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        animator.SetTrigger("Dash");
    }

    void StartRoll()
    {
        isRolling = true;
        canMove = false;

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * rollSpeed, 0f);

        animator.SetTrigger("Roll");
    }

    public void EndMovementAbility()
    {
        isDashing = false;
        isRolling = false;
        canMove = true;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
