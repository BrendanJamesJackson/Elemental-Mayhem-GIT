using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float damage = 10f;
    public float knockbackForce = 10f;

    public GameObject owner;
    public PlayerCombat playerCombat;

    public void Initialize(GameObject attacker)
    {
        //owner = attacker;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == owner)
        {
            return;
        }

        Debug.Log("Hitbox land");

        PlayerManager pm = collision.GetComponent<PlayerManager>();

        if (pm != null)
        {
            Vector2 direction = (collision.transform.position - owner.transform.position).normalized;


            pm.TakeHit(
                playerCombat.GetDamage(),
                playerCombat.GetKnockback(),
                direction,
                owner
            );
        }

    }
}
