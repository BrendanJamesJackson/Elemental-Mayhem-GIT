using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    public PlayerMovement movement; // to check grounded
    public PlayerManager playerManager;

    [Header("Character Type")]
    public bool isComboCharacter = true;

    [Header("Attack Data")]
    public float[] damage;
    public float[] knockback;

    [Header("Attack Indices")]
    public int airAttackIndex = 3;
    public int specialAttackIndex = 4;
    public int rangedSpecialAttackIndex = 5;


    [Header("Special Attack")]
    public bool isChargeSpecial = false;
    public bool isChargeElementalSpecial = false;
    public bool isRangedSpecial = false;
    public bool isRangedElementalSpecial = false;

    private bool isCharging = false;

    [Header("Blocking")]
    public bool isBlocking = false;

    // Internal State
    [SerializeField]private int currentAttackIndex = 0;
    [SerializeField]private bool isAttacking = false;

    // Combo System
    private bool canCombo = false;
    private bool queueNextAttack = false;


    private Coroutine blockRoutine;
    public float blockDelay = 0.1f;
    private bool blockActive = false;


    public bool IsAttacking()
    {
        return isAttacking;
    }

    //Get player Input
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (isBlocking)
            return;

        if (!context.performed)
            return;

        // AIR ATTACK (overrides everything)
        if (!movement.IsGrounded())
        {
            DoAirAttack();
            return;
        }

        if (isComboCharacter)
        {
            HandleComboInput();
        }
        else
        {
            HandleCycleInput();
        }
    }

    public void BlockInput(InputAction.CallbackContext context)
    {
        if (!movement.IsGrounded())
        {
            return;
        }

        if (context.started)
        {
            //StartBlock();
            playerManager.evolveTriggered = false;
            if (blockRoutine != null)
            {
                StopCoroutine(blockRoutine);
            }

            blockRoutine = StartCoroutine(BlockDelayRoutine());
        }
        else if (context.canceled)
        {
            if (blockRoutine != null)
                StopCoroutine(blockRoutine);

            if (blockActive)
            {
                EndBlock();
                blockActive = false;
            }
        }
    }

    private IEnumerator BlockDelayRoutine()
    {
        yield return new WaitForSeconds(blockDelay);

        if (!playerManager.evolveTriggered)
        {
            StartBlock();
            blockActive = true;
        }
    }

    public void SpecialInput(InputAction.CallbackContext context)
    {
        if (isBlocking || !movement.IsGrounded())
            return;

        if ((!isChargeSpecial && !playerManager.GetIsElemental()) || (!isChargeElementalSpecial && playerManager.GetIsElemental()))
        {
            if (context.performed && !isAttacking)
            {
                DoSpecial();
            }
        }
        else
        {
            Debug.Log("Special Charging in progress");
            if (context.started && !isAttacking)
            {
                StartCharge();
            }
            else if (context.canceled && isCharging)
            {
                Debug.Log("Release Charge Function Called");
                ReleaseCharge();
            }
        }
    }

    public void RangedSpecialInput(InputAction.CallbackContext context)
    {
        if (isBlocking || !movement.IsGrounded())
            return;

        if (!isRangedSpecial && !playerManager.GetIsElemental())
        {
            Debug.Log("Not ranged special, transferrring ownership to standard special");
            SpecialInput(context);
        }
        else if (!isRangedElementalSpecial && playerManager.GetIsElemental())
        {
            SpecialInput(context);
        }
        else if (isRangedSpecial && !playerManager.GetIsElemental())
        {
            if (context.performed && !isAttacking)
            {
                DoRangedSpecial();
            }
        }
        else if (isRangedElementalSpecial && playerManager.GetIsElemental())
        {
            if (context.performed && !isAttacking)
            {
                DoRangedSpecial();
            }
        }
        
        
    }

    void DoRangedSpecial()
    {
        isAttacking = true;
        currentAttackIndex = specialAttackIndex;
        animator.SetTrigger("RangedSpecial");
    }

    void DoSpecial()
    {
        isAttacking = true;
        currentAttackIndex = specialAttackIndex;
        animator.SetTrigger("Special");
    }

    void StartCharge()
    {
        if (isAttacking)
            return;

        isCharging = true;
        isAttacking = true;

        animator.SetBool("IsCharging", true);
    }

    void ReleaseCharge()
    {
        Debug.Log("Release Charge Function");
        if (!isCharging)
            return;

        isCharging = false;
        currentAttackIndex = specialAttackIndex;

        animator.SetBool("IsCharging", false);
        animator.SetTrigger("ReleaseSpecial");
    }

    void StartBlock()
    {
        if (isAttacking)
            return;

        isBlocking = true;
        animator.SetBool("IsBlocking", true);
    }

    void EndBlock()
    {
        isBlocking = false;
        animator.SetBool("IsBlocking", false);
    }

    //Combo based attacking
    void HandleComboInput()
    {
        if (!isAttacking)
        {
            StartCombo();
        }
        else if (canCombo)
        {
            queueNextAttack = true;
        }
    }

    void StartCombo()
    {
        isAttacking = true;
        currentAttackIndex = 0;

        TriggerAttack(currentAttackIndex);
    }

    //Animation event called at end of each chain animation
    public void TryContinueCombo()
    {
        if (queueNextAttack)
        {
            queueNextAttack = false;
            currentAttackIndex++;

            // Clamp to array size
            if (currentAttackIndex >= 3)
                currentAttackIndex = 0;

            TriggerAttack(currentAttackIndex);
        }
        else
        {
            EndAttack();
        }
    }

    // Animation Event
    public void EnableCombo()
    {
        canCombo = true;
    }

    // Animation Event
    public void DisableCombo()
    {
        canCombo = false;
    }

    //Cycle based attacking
    void HandleCycleInput()
    {
        if (isAttacking)
            return;

        isAttacking = true;

        TriggerAttack(currentAttackIndex);

        currentAttackIndex++;
        if (currentAttackIndex >= damage.Length)
            currentAttackIndex = 0;
    }

    //Air attacking
    void DoAirAttack()
    {
        if (isAttacking)
            return;

        isAttacking = true;
        currentAttackIndex = airAttackIndex;

        animator.SetTrigger("AirAttack");
    }

    //Used to set animation triggers
    void TriggerAttack(int index)
    {
        string triggerName = "Attack" + (index + 1);
        animator.SetTrigger(triggerName);
    }

    //Called at the end of a final animation in chain combo setup
    public void EndAttack()
    {
        isAttacking = false;
        canCombo = false;
        queueNextAttack = false;
        currentAttackIndex = 0;
    }

    public float GetDamage()
    {
        return damage[currentAttackIndex];
    }

    public float GetDamageProjectile(int index)
    {
        return damage[index];
    }

    public float GetKnockback()
    {
        return knockback[currentAttackIndex];
    }

    public float GetKnockbackProjectile(int index)
    {
        return knockback[index];
    }

    public int GetCurrentAttackIndex()
    {
        return currentAttackIndex;
    }

}
