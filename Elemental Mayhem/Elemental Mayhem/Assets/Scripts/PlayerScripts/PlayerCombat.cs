using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public Hitbox hitbox;
    public float attackDuration = 0.2f;

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            StartCoroutine(DoAttack());
        }
    }

    IEnumerator DoAttack()
    {
        hitbox.gameObject.SetActive(true);
        hitbox.Initialize(gameObject);

        yield return new WaitForSeconds(attackDuration);

        hitbox.gameObject.SetActive(false);
    }
}
