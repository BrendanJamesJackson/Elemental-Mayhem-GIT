using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 10f;

    private GameObject owner;

    public void Initialize(GameObject attacker)
    {
        owner = attacker;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
        {
            return;
        }

        KnockbackReceiver receiver = collision.GetComponent<KnockbackReceiver>();

        if (receiver != null)
        {
            Vector2 direction = (collision.transform.position - owner.transform.position).normalized;
            receiver.ApplyKnockback(direction, knockbackForce, owner);
        }
        
    }
}
