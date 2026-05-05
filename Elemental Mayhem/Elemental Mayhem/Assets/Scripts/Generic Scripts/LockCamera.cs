using UnityEngine;

public class LockCamera : MonoBehaviour
{
    public float locked = 0f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = locked;
        pos.x = locked;
        transform.position = pos;
    }
}