using UnityEngine;

public class MainMenuGlowPulse : MonoBehaviour
{

    public Material glowMaterial;

    public float emissionStrength;
    public float pulseTimer = 0f;
    public float pulseSpeed = 2f;
    public float pulseMin = 0f;
    public float pulseMax = 10f;
    public float pulseOffset = 1f;

    private void Start()
    {
        glowMaterial = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        pulseTimer += Time.deltaTime * pulseSpeed;
        float t = (Mathf.Sin(pulseTimer) + pulseOffset) / 2f;
        emissionStrength = Mathf.Lerp(pulseMin, pulseMax, t);

        glowMaterial.SetFloat("_EmissionStrength", emissionStrength);

    }

}
