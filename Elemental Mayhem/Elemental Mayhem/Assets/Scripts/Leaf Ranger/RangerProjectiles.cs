using UnityEngine;

public class RangerProjectiles : MonoBehaviour
{
    public GameObject basicArrow;
    public GameObject arrowShower;
    public GameObject diagonalArrow;
    public GameObject thorns;
    public GameObject spear;

    public Transform shootPoint;

    public float shootSpeed;


    public void FireAttack2()
    {
        GameObject temp = Instantiate(basicArrow,shootPoint.position,shootPoint.rotation);
        Hitbox hb = temp.GetComponent<Hitbox>();
        if (hb != null )
        {
            hb.Initialize(this.gameObject);
        }

        Rigidbody2D rb = temp.GetComponent<Rigidbody2D>();
        float direction = this.gameObject.transform.localScale.x;
        temp.transform.localScale = this.gameObject.transform.localScale;
        rb.linearVelocityX = direction * shootSpeed;
        Destroy(temp, 5f);
    }

    public void FireAttack3()
    {
        GameObject temp = Instantiate(arrowShower, shootPoint.position, shootPoint.rotation);
        Hitbox hb = temp.GetComponentInChildren<Hitbox>();
        if (hb != null)
        {
            hb.Initialize(this.gameObject);
        }
        Destroy(temp, 3f);

    }


    /*public void FireProjectile()
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

    }*/

}
