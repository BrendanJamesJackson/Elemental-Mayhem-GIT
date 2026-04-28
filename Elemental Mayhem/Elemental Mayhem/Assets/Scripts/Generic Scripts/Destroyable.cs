using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
