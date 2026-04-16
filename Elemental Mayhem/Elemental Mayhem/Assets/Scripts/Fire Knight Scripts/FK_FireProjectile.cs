using UnityEngine;

public class FK_FireProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;

    public Transform shootPoint;

    public float shootSpeed;

    public void FireProjectile()
    {
        GameObject temp = Instantiate(projectilePrefab,shootPoint.position,shootPoint.rotation);

        Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();

        float direction = this.gameObject.transform.localScale.x;

        rb.linearVelocityX = direction * shootSpeed;


        Destroy(temp, 5f);

    }

}
