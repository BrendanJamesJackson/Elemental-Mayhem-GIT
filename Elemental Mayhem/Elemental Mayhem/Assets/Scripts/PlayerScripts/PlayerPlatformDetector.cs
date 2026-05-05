using UnityEngine;

public class PlayerPlatformDetector : MonoBehaviour
{
    public PlayerMovement playerMovement;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    playerMovement.SetCurrentPlatform(collision.collider);
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            playerMovement.ClearCurrentPlatform(collision.collider);
        }
    }
}