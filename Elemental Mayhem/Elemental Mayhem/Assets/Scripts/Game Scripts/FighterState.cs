using UnityEngine;

public class FighterState : MonoBehaviour
{
    public int stocks = 3;
    public float damagePercent = 0f;

    public Transform respawnPoint;

    [HideInInspector] public GameObject lastAttacker;
}
