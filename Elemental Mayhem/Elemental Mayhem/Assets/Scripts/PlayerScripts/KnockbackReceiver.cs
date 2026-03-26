using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody2D rb;
    public PlayerMovement movement;

    public float hitstunDuration = 0.2f;
    public float hitstopDuration = 5f;
    public float hitstopShake = 0.05f;
    public bool isShaking = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement>();
    }

    public void ApplyKnockback(Vector2 direction,float force, GameObject attacker)
    {
        StopAllCoroutines();
        StartCoroutine(HandleHit(direction, force, attacker));
    }

    IEnumerator HandleHit(Vector2 direction, float force, GameObject attacker)
    {
        PlayerMovement attackerMovement = attacker.GetComponent<PlayerMovement>();

        yield return StartCoroutine(DoHitStop(attackerMovement));

        // Disable movement
        if (movement != null)
            movement.canMove = false;

        // Apply knockback
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);

        // Wait for hitstun
        yield return new WaitForSeconds(hitstunDuration);

        // Re-enable movement
        if (movement != null)
            movement.canMove = true;
    }

    private void Update()
    {
        if (isShaking)
        {
            transform.position += (Vector3)Random.insideUnitCircle * hitstopShake;
        }
    }

    IEnumerator DoHitStop(PlayerMovement attackerMovement)
    {
        //Vector2 originalVelocity = rb.linearVelocity;

        Rigidbody2D attackerRb = attackerMovement != null ? attackerMovement.GetComponent<Rigidbody2D>() : null;
        //Vector2 attackerVelocity = attackerRb != null ? attackerRb.linearVelocity : Vector2.zero;

        rb.linearVelocity = Vector2.zero;
        if (attackerRb != null)
            attackerRb.linearVelocity = Vector2.zero;

        if (movement != null)
            movement.canMove = false;
        this.GetComponent<Animator>().speed = 0;
        this.GetComponent<Animator>().SetTrigger("Hit");

        if (attackerMovement != null)
            attackerMovement.canMove = false;
        attackerMovement.gameObject.GetComponent<Animator>().speed = 0;

        isShaking = true;

        yield return new WaitForSeconds(hitstopDuration);

        isShaking = false;

        if (movement != null)
            movement.canMove = true;
        this.GetComponent<Animator>().speed = 1;

        if (attackerMovement != null)
            attackerMovement.canMove = true;
        attackerMovement.gameObject.GetComponent<Animator>().speed = 1;

    }
}
