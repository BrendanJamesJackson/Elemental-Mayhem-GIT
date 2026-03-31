using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Life (Durability System)")]
    public float maxLife = 100f;
    public float currentLife;

    [Header("Mana")]
    public float maxMana = 300f;
    public float currentMana;

    [Header("Hearts (Lives)")]
    public int maxHearts = 3;
    public int currentHearts;

    [Header("Knockback Scaling")]
    public float minKnockbackMultiplier = 1f;
    public float maxKnockbackMultiplier = 2f;

    private KnockbackReceiver knockbackReceiver;

    private void Awake()
    {
        currentLife = maxLife;
        currentHearts = maxHearts;

        knockbackReceiver = GetComponent<KnockbackReceiver>();
    }

    public void TakeHit(float baseDamage,float baseKnockback, Vector2 direction, GameObject attacker = null)
    {
        Debug.Log("Take hit");
        //Reduce life
        currentLife -= baseDamage;
        currentLife = Mathf.Max(currentLife, 0);

        //Calc knockback
        float lifePercent = currentLife / maxLife;
        float knockbackMultiplier = Mathf.Lerp(maxKnockbackMultiplier, minKnockbackMultiplier, lifePercent);

        float finalKnockback = baseKnockback * knockbackMultiplier;

        //Apply knockback
        if (knockbackReceiver != null)
        {
            knockbackReceiver.ApplyKnockback(direction,finalKnockback, attacker);
        }

        //Gain mana
        if (attacker != null)
        {
            PlayerManager attackerManager = attacker.GetComponent<PlayerManager>();
            if (attackerManager != null)
            {
                attackerManager.GainMana(baseDamage * 2f);
            }
        }
    }


    public void GainMana(float amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana,0, maxMana);
    }

    public bool HasFullMana()
    {
        return currentMana >= maxMana;
    }

    public void SpendMana()
    {
        currentMana = 0;
    }

    public void LoseHeart()
    {
        currentHearts--;

        if (currentHearts <= 0)
        {
            Die();
        }
        else
        {
            Respawn();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " is out");
        //Notify game manager
        gameObject.SetActive(false);
    }

    void Respawn()
    {
        Debug.Log(gameObject.name + " lost a heart!");

        currentLife = maxLife;

        // TODO:
        // - move player to spawn point
        // - temporary invulnerability if needed
    }

}
