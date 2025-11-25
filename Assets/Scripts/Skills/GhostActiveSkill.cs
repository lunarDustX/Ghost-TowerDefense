using UnityEngine;

public class GhostActiveSkill : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] GhostAura aura;          
    [SerializeField] SlowField slowFieldPrefab;

    [Header("技能设置")]
    public KeyCode key = KeyCode.Q;
    public float cooldown = 10f;
    public float fieldDuration = 3f;
    public float slowMultiplier = 0.5f;  // 速度倍率：0.5 = -50%

    private float cooldownTimer = 0f;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(key))
        {
            TryCast();
        }
    }

    void TryCast()
    {
        if (cooldownTimer > 0f) return;
        if (aura == null || slowFieldPrefab == null) return;

        // 以当前 Aura 中心位置生成领域
        Vector3 center = aura.transform.position;

        SlowField field = Instantiate(slowFieldPrefab, center, Quaternion.identity);

        // 用 Aura 半径 & 技能参数初始化
        field.slowMultiplier = slowMultiplier;
        field.duration = fieldDuration;
        field.ApplyRadius(aura.radius);

        cooldownTimer = cooldown;
    }

    public float CooldownRemaining => Mathf.Max(0f, cooldownTimer);
}
