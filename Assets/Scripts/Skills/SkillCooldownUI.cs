using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Image ring;
    public GameObject root;   // UI 的根节点，可整体隐藏

    private float cooldown = 0f;
    private float timer = 0f;

    public void StartCooldown(float cd)
    {
        cooldown = cd;
        timer = cd;

        if (root != null)
            root.SetActive(true);
    }

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;

            float t = Mathf.Clamp01(timer / cooldown);
            if (ring != null)
                ring.fillAmount = 1-t;

            if (timer <= 0f)
            {
                if (root != null)
                    root.SetActive(false);  // 冷却完自动隐藏
            }
        }
    }
}
