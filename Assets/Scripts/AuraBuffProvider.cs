using UnityEngine;

public class AuraBuffProvider : MonoBehaviour
{
    public static AuraBuffProvider Instance { get; private set; }

    [Header("塔在幽灵 Aura 内的加成")]
    public float damageMultiplier = 1.0f;
    public float attackCooldownMultiplier = 1.0f; // <1 冷却更短
    public float rangeMultiplier = 1.0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
