using UnityEngine;

public class PostHitHandler : MonoBehaviour
{

    public PlayerManager playerManager;
    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;



    public void ResolvePostHit()
    {
        playerCombat.EndAttack();
        playerMovement.EnableMovement();
        playerMovement.EndMovementAbility();
    }
}
