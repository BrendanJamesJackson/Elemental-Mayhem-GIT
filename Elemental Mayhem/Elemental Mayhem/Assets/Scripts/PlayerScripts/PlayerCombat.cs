using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Setup")]
    public Animator animator;
    public PlayerMovement movement; // to check grounded

    [Header("Character Type")]
    public bool isComboCharacter = true;

    [Header("Attack Data")]
    public float[] damage;
    public float[] knockback;

    [Header("Air Attack")]
    public int airAttackIndex = 0;

    // Internal State
    [SerializeField]private int currentAttackIndex = 0;
    private bool isAttacking = false;

    // Combo System
    private bool canCombo = false;
    private bool queueNextAttack = false;


    //Get player Input
    public void AttackInput(InputAction.CallbackContext context)
    {
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
            if (currentAttackIndex >= damage.Length)
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

    public float GetKnockback()
    {
        return knockback[currentAttackIndex];
    }

    public int GetCurrentAttackIndex()
    {
        return currentAttackIndex;
    }

}
