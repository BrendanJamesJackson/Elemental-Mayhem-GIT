using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Life (Durability System)")]
    public float maxLife = 100f;
    public float currentLife;

    [Header("Mana")]
    public float maxMana = 300f;
    public float currentMana;

    [Header("Hearts (Lives / Stocks)")]
    public int maxHearts = 3;
    public int currentHearts;

    [Header("Knockback Scaling")]
    public float minKnockbackMultiplier = 1f;
    public float maxKnockbackMultiplier = 2f;

    [Header("Respawn")]
    //public Transform spawnPoint;
    public float respawnInvulnerabilityTime = 2f;

    private KnockbackReceiver knockbackReceiver;

    [Header("State")]
    public bool isInElementalForm = false;
    public bool isEliminated = false;
    public bool isInvulnerable = false;

    public Animator animator;

    [HideInInspector] public GameObject lastAttacker;

    public bool evolveTriggered = false;

    private void Awake()
    {
        currentLife = maxLife;
        currentMana = 0f;
        currentHearts = maxHearts;

        knockbackReceiver = GetComponent<KnockbackReceiver>();
    }

    private void Start()
    {
        GameManager.instance.RegisterFighter(this);
    }

    public float GetManaRatio()
    {
        return currentMana / maxMana;
    }
    public void EvolveInputHandler(InputAction.CallbackContext context)
    {

        if (currentMana < maxMana)
        {
            return;
        }

        if (context.performed)
        {
            evolveTriggered = true;
            isInElementalForm = true;
        }
    }

    public void TakeHit(float baseDamage,float baseKnockback, Vector2 direction, GameObject attacker = null)
    {

        if (isEliminated || isInvulnerable)
            return;

        Debug.Log(gameObject.name + " took hit");

        lastAttacker = attacker;

        //Reduce life
        currentLife -= baseDamage;
        currentLife = Mathf.Max(currentLife, 0);

        //Calc knockback
        float lifePercent = currentLife / maxLife;
        float knockbackMultiplier = Mathf.Lerp(maxKnockbackMultiplier, minKnockbackMultiplier, lifePercent);

        float finalKnockback = baseKnockback * knockbackMultiplier;
        Debug.Log("Base Knockback: " + baseKnockback);
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

    private void Update()
    {
        animator.SetBool("isElemental",isInElementalForm);

        if (isInElementalForm)
        {
            evolveTriggered = false;
        }
    }

    public bool GetIsElemental()
    {
        return isInElementalForm;
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

        if (isEliminated)
        {
            return;
        }

        currentHearts--;

        Debug.Log(gameObject.name + " lost a heart. Remaining: " + currentHearts);

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
        isEliminated = true;

        Debug.Log(gameObject.name + " is out");
        //Notify game manager
        gameObject.SetActive(false);
    }

    void Respawn()
    {
        Debug.Log(gameObject.name + " respawning");

        currentLife = maxLife;

        currentMana = 0f;
        isInElementalForm = false;

        
        transform.position = GameManager.instance.spawnPoints
            [(Random.Range(0, GameManager.instance.spawnPoints.Length - 1))]
            .position;
        

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        StartCoroutine(RespawnInvulnerability());

    }

    private IEnumerator RespawnInvulnerability()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(respawnInvulnerabilityTime);
        isInvulnerable = false;
    }


    public float GetLifePercent()
    {
        return currentLife / maxLife;
    }


    public int GetHearts()
    {
        return currentHearts;
    }

}
