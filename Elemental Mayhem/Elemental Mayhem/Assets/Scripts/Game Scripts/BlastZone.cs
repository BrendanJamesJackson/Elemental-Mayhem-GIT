using UnityEngine;

public class BlastZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        FighterState fighter = collision.GetComponent<FighterState>();

        if (fighter != null )
        {
            GameManager.instance.KnockOut(fighter );
        }

    }
}
