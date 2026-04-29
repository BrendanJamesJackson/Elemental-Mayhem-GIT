using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public Transform cam;
    public float parallaxAmount = 0.1f; // lower = further away

    private Vector3 startPos;
    private float startCamX;

    void Start()
    {
        startPos = transform.position;
        startCamX = cam.position.x;
    }

    void LateUpdate()
    {
        float distance = cam.position.x - startCamX;
        transform.position = new Vector3(
            startPos.x + distance * parallaxAmount,
            startPos.y,
            startPos.z
        );
    }
}