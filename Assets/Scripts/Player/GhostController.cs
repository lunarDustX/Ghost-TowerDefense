using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float currentMoveSpeed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentMoveSpeed = moveSpeed;
    }

    void Update()
    {
        // 默认 Horizontal / Vertical 同时支持 WASD + 方向键
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector2(h, v);

        // 斜向移动时归一化，避免比直线更快
        if (moveInput.sqrMagnitude > 1f)
        {
            moveInput = moveInput.normalized;
        }
    }

    void FixedUpdate()
    {
        Vector2 targetPos = rb.position + moveInput * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(targetPos);
    }

    // 给 HeroUpgradeTree 调用的接口
    public void ApplyMoveSpeedFromUpgrade(float multiplier)
    {
        currentMoveSpeed = moveSpeed * multiplier;
    }
}
