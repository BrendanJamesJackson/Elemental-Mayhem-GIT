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
    public bool isInHitstun = false;

    [Header("Jump")]
    public float jumpForce = 14f;
    public float gravityScale = 3f;
    public float fallMultiplier = 2.5f;

    [Header("Jump Assist")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Fast Fall")]
    public float fastFallSpeed = 20f;

    [Header("Double Jump")]
    public int maxJumps = 2;

    private int jumpsUsed = 0;
    private bool hasDoubleJumped = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Collider2D currentPlatform;
    [SerializeField] private float dropDuration = 0.25f;
    private bool isDropping = false;
    [SerializeField] private Collider2D floorCollider;

    [Header("Dash / Roll")]
    public bool hasDash = false;
    public bool hasRoll = false;
    public bool hasElementalMovementAbility = false;

    public float dashSpeed = 15f;
    public float rollSpeed = 12f;

    private bool isDashing = false;
    private bool isRolling = false;

    private Rigidbody2D rb;
    public Animator animator;
    public PlayerCombat playerCombat;

    private float moveInput;
    [SerializeField]private float downInput;
    [SerializeField] private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private float currentVelocityX;

    public PlayerManager playerManager;

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
        if (!(rb.linearVelocityY > 0))
        {
            CheckGround();
        }
        else
        {
            isGrounded = false;
        }


        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.fixedDeltaTime;
        }

        HandleMovement();
        HandleBetterJump();
        HandleJump();
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        currentVelocityX = 0f;
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
        if (!canMove || isInHitstun)
            return;


        if (jumpBufferCounter > 0f && isGrounded && downInput < -0.5f && currentPlatform != null && !isDropping)
        {
            StartCoroutine(DropThroughPlatform());
            jumpBufferCounter = 0f;
            return;
        }


        if (jumpBufferCounter > 0f)
        {
            // FIRST JUMP (ground or coyote)
            if (coyoteTimeCounter > 0f)
            {
                Jump();
                jumpsUsed = 1;
                coyoteTimeCounter = 0f;
                jumpBufferCounter = 0f;
            }
            // DOUBLE JUMP
            else if (jumpsUsed < maxJumps)
            {
                Jump();
                jumpsUsed++;
                hasDoubleJumped = true;
                jumpBufferCounter = 0f;
            }
        }
    }


    private IEnumerator DropThroughPlatform()
    {
        isDropping = true;
        animator.SetBool("isDropping", isDropping);
        
        Collider2D tempCol = currentPlatform;
        Physics2D.IgnoreCollision(floorCollider, currentPlatform, true);
        //isGrounded = false;
        // small push downward so you actually fall
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);

        yield return new WaitForSeconds(dropDuration);

        Physics2D.IgnoreCollision(floorCollider, tempCol, false);

        isDropping = false;
        animator.SetBool("isDropping", isDropping);
    }

    void HandleMovement()
    {
        


        if (isInHitstun || isDashing || isRolling)
        {
            return;
        }

        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            currentVelocityX = 0f;
            return;
        }


        int moveInputFixed = 0;

        if (moveInput > 0.5f)
        {
            moveInputFixed = 1;
        }
        else if (moveInput < -0.5f)
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
            //currentVelocityX = 0f;
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
        // Faster fall
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Fast fall input
        if (!isGrounded && downInput < -0.5f)
        {
            if (hasDoubleJumped)
            {
                Debug.Log("GROUND POUND TRIGGERED");
                hasDoubleJumped = false; // prevent spam
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Min(rb.linearVelocity.y, -fastFallSpeed));
        }
    }

    void CheckGround()
    {

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            jumpsUsed = 0;
            hasDoubleJumped = false;
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
        if ( isDashing || isRolling || playerCombat.IsAttacking() || (playerManager.GetIsElemental() && !hasElementalMovementAbility))
            return;

        if (playerManager.GetIsElemental() && hasElementalMovementAbility)
        {
            isDashing = true;
            animator.SetTrigger("Dash");
        }
        else if (isDashInput)
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
        //canMove = false;

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(direction * dashSpeed, 0f);

        animator.SetTrigger("Dash");
    }

    void StartRoll()
    {
        isRolling = true;
        //canMove = false;

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

    public void SetCurrentPlatform(Collider2D platform)
    {
        currentPlatform = platform;
    }

    public void ClearCurrentPlatform(Collider2D platform)
    {
        if (currentPlatform == platform)
        {
            currentPlatform = null;
        }
    }

}
