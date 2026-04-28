using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    public Material glowMaterial;

    public Color emissionColor;
    public Color elementalEmissionColor;
    public float emissionStrength;
    public float chargingMaxStrength;
    public float tolerance;

    public float manaRatio;


    public PlayerManager playerManager;


    public float pulseMin = 8f;
    public float pulseMax = 12f;
    public float pulseSpeed = 2f;

    public float elementalBrightness = 10f;

    private float pulseTimer = 0f;

    private void Start()
    {
        glowMaterial = GetComponent<SpriteRenderer>().material;
    }


    private void Update()
    {
        manaRatio = playerManager.GetManaRatio();
        bool isElementalForm = playerManager.GetIsElemental();


        if (isElementalForm)
        {
            emissionStrength = elementalBrightness;
            tolerance = 0.4f;
        }

        else if (manaRatio >= 1 )
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float t = (Mathf.Sin(pulseTimer) + 1f) / 2f; 
            emissionStrength = Mathf.Lerp(pulseMin, pulseMax, t);
            tolerance = 0.4f;
        }
        else
        {
            emissionStrength = math.remap(0,1,0,chargingMaxStrength,manaRatio);
            tolerance = math.remap(0,1,0.1f,0.4f,manaRatio);
        }
        SetMaterialProperties();
    }

    public void SetMaterialProperties()
    {
        glowMaterial.SetFloat("_EmissionStrength",emissionStrength);
        glowMaterial.SetFloat("_Tolerance", tolerance);
    }


}
