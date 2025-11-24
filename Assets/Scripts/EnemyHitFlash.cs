using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashDuration = 0.08f;

    private Color _originalColor;
    private float _flashTimer;

    void Awake()
    {
        if (sr == null)
            sr = GetComponentInChildren<SpriteRenderer>();

        if (sr != null)
            _originalColor = sr.color;
    }

    public void PlayFlash()
    {
        _flashTimer = flashDuration;
        if (sr != null)
            sr.color = flashColor;
    }

    void Update()
    {
        if (_flashTimer > 0f)
        {
            _flashTimer -= Time.deltaTime;
            if (_flashTimer <= 0f && sr != null)
            {
                sr.color = _originalColor;
            }
        }
    }
}
