using UnityEngine;

public class Generic_FireProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;

    public Transform shootPoint;

    public float shootSpeed;

    public void FireProjectile()
    {
        GameObject temp = Instantiate(projectilePrefab,shootPoint.position,shootPoint.rotation);
        
        Hitbox hb = temp.GetComponent<Hitbox>();

        if (hb != null)
        {
            hb.Initialize(this.gameObject);
        }

        Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();

        float direction = this.gameObject.transform.localScale.x;

        temp.transform.localScale = this.gameObject.transform.localScale;

        rb.linearVelocityX = direction * shootSpeed;


        Destroy(temp, 5f);

    }

}
