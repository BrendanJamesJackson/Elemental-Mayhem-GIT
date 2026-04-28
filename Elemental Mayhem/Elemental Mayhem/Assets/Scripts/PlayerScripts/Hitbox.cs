using System.Collections;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 10f;

    public GameObject owner;
    public PlayerCombat playerCombat;

    public bool isProjectile = false;

    [Header("Projectile Only")]
    public GameObject impactEffect;
    public float destroyDelay = 0.03f;
    public float stickTime = 0.02f;

    private bool hasHit = false;
    private Rigidbody2D rb;
    private Collider2D col;


    public void Initialize(GameObject attacker)
    {
        owner = attacker;
        playerCombat = owner.GetComponent<PlayerCombat>();
        
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        Debug.Log("Initialized");


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {



        if (collision.gameObject == owner)
        {
            return;
        }

        Debug.Log("Hitbox land" + collision.gameObject.name);

        PlayerManager pm = collision.GetComponent<PlayerManager>();

        if (pm != null)
        {
        /*#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
        #endif
        */
            Vector2 direction = (collision.transform.position - owner.transform.position).normalized;


            pm.TakeHit(
                playerCombat.GetDamage(),
                playerCombat.GetKnockback(),
                direction,
                owner
            );
            Debug.Log("Take Hit: Direction: " + direction);
            if (isProjectile)
            {
                StartCoroutine(ProjectileImpact(collision.transform));
            }

        }

    }

    IEnumerator ProjectileImpact(Transform hitTarget)
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (col != null)
            col.enabled = false;

        transform.SetParent(hitTarget);

        yield return new WaitForSeconds(stickTime);

        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, destroyDelay);

    }

}
