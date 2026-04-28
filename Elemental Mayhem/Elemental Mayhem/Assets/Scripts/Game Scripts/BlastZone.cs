using UnityEngine;

public class BlastZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
       PlayerManager player = collision.gameObject.GetComponent<PlayerManager>();

        if (player == null)
        {
            return;
        }

        if (player.isEliminated)
        {
            return;
        }

       
        GameManager.instance.KnockOut(player);
        

    }
}
