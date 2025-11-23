using UnityEngine;

public abstract class UpgradeData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
    // public UpgradeCategory category;

    // 是否可重复拿（例如：移速+10%可以叠）
    public bool canStack = true;

    // 具体生效逻辑，由子类实现
    public abstract void Apply();
}

public enum UpgradeCategory
{
    Movement,
    Aura,
    TowerBuff,
    EnemyDebuff,
    ActiveSkill
}