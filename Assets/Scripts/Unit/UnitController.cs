using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour, IPointerDownHandler
{
    public UnitStats m_stats;
    public LayerMask m_targetLayer;
    private Vector2 m_direction = Vector2.right;
    public UnitHealthBar m_healthBar;

    private float m_currentHealth;
    private float m_lastAttackTime;
    private Rigidbody2D m_rb;
    private UnitController m_currentTarget;
    private bool m_isDead = false;

    private float m_baseSpeed;
    private float m_speedMultiplier = 1f;

    private int m_clicksReceived = 0;
    private bool m_isKnockedBack = false;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_currentHealth = m_stats.maxHealth;
        m_baseSpeed = m_stats.speed;

        if (m_healthBar != null)
        {
            m_healthBar.UpdateHealthBar(m_currentHealth, m_stats.maxHealth);
            m_healthBar.ChangeFillColor(LayerMask.LayerToName(gameObject.layer));
        }

        if (LayerMask.LayerToName(gameObject.layer).Contains("Player"))
        {
            m_direction = Vector2.right;
        }
        else
        {
            m_direction = Vector2.left;
        }
    }

    void Update()
    {
        if (m_isDead)
            return;

        if (CheckForTarget())
        {
            StopMoving();
            if (Time.time >= m_lastAttackTime + m_stats.attackCooldown)
            {
                PerformAttack();
            }
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        if (m_isKnockedBack)
            return;

        m_rb.linearVelocity = m_direction * (m_baseSpeed * m_speedMultiplier);
    }

    void StopMoving()
    {
        if (m_isKnockedBack)
            return;
        m_rb.linearVelocity = Vector2.zero;
    }

    bool CheckForTarget()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, m_direction, m_stats.attackRange, m_targetLayer);
        if (hit.collider != null)
        {
            m_currentTarget = hit.collider.GetComponent<UnitController>();
            return m_currentTarget != null;
        }
        return false;
    }

    void PerformAttack()
    {
        m_lastAttackTime = Time.time;

        if (m_stats.unitType == UnitType.SuicideBomber)
        {
            Explode();
            return;
        }

        if (m_stats.unitType == UnitType.Healer)
        {
            if (m_currentTarget != null)
                m_currentTarget.Heal(m_stats.damage);
            return;
        }

        if (m_currentTarget != null)
        {
            m_currentTarget.TakeDamage(m_stats.damage);

            if (m_stats.effectOnHit != SpecialEffect.None)
            {
                m_currentTarget.ApplyEffect(
                    m_stats.effectOnHit, m_stats.effectDuration, m_stats.effectValue, m_direction);
            }
        }
    }

    void Explode()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_stats.explosionRadius, m_targetLayer);
        foreach (Collider2D collider in colliders)
        {
            UnitController targetUnit = collider.GetComponent<UnitController>();
            if (targetUnit != null)
            {
                float distance = Vector2.Distance(transform.position, collider.transform.position);
                float normalizedDistance = Mathf.Clamp01(distance / m_stats.explosionRadius);

                float curveValue = m_stats.explosionDamageCurve != null
                                       ? m_stats.explosionDamageCurve.Evaluate(normalizedDistance)
                                       : 1f;
                float finalDamage = m_stats.damage * curveValue;

                targetUnit.TakeDamage(finalDamage);
            }
        }

        Die();
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        if (m_stats.clickToKill > 0)
        {
            m_clicksReceived++;
            StartCoroutine(FlashColor());

            if (m_clicksReceived >= m_stats.clickToKill)
            {
                Die();
            }
        }
    }

    IEnumerator FlashColor()
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite)
        {
            Color original = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = original;
        }
    }

    public void TakeDamage(float _amount)
    {
        m_currentHealth -= _amount;
        if (m_healthBar != null)
            m_healthBar.UpdateHealthBar(m_currentHealth, m_stats.maxHealth);
        if (m_currentHealth <= 0)
            Die();

        Debug.Log("TakeDamage: " + _amount);
    }

    public void Heal(float _amount)
    {
        m_currentHealth += _amount;
        if (m_currentHealth > m_stats.maxHealth)
            m_currentHealth = m_stats.maxHealth;
        if (m_healthBar != null)
        {
            m_healthBar.UpdateHealthBar(m_currentHealth, m_stats.maxHealth);
        }
    }

    public void ApplyEffect(SpecialEffect _effect, float _duration, float _value, Vector2 _knockbackDir)
    {
        switch (_effect)
        {
            case SpecialEffect.Poison:
                StartCoroutine(PoisonRoutine(_duration, _value));
                break;
            case SpecialEffect.Slow:
                StartCoroutine(SlowRoutine(_duration, _value));
                break;
            case SpecialEffect.Knockback:
                StartCoroutine(KnockbackRoutine(_knockbackDir, _value));
                break;
        }
    }

    IEnumerator PoisonRoutine(float _duration, float _dps)
    {
        float elapsed = 0;
        while (elapsed < _duration)
        {
            TakeDamage(_dps / 10);
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
    }

    IEnumerator SlowRoutine(float _duration, float _slowAmount)
    {
        m_speedMultiplier = _slowAmount;
        yield return new WaitForSeconds(_duration);
        m_speedMultiplier = 1f;
    }

    IEnumerator KnockbackRoutine(Vector2 _pushDirection, float _force)
    {
        m_isKnockedBack = true;

        m_rb.linearVelocity = Vector2.zero;
        Vector2 knockbackVector = (_pushDirection + Vector2.up * 0.5f).normalized;
        m_rb.AddForce(knockbackVector * _force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.4f);
        m_isKnockedBack = false;
    }

    void Die()
    {
        m_isDead = true;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (m_stats == null)
            return;

        Gizmos.color = Color.green;
        Vector3 start = transform.position;

        Vector2 previewDir = m_direction;
        if (!Application.isPlaying)
        {
            previewDir = LayerMask.LayerToName(gameObject.layer).Contains("Player") ? Vector2.right : Vector2.left;
        }

        Vector3 end = start + (Vector3)(previewDir * m_stats.attackRange);
        Gizmos.DrawLine(start, end);

        Gizmos.DrawWireSphere(end, 0.1f);

        if (m_stats.unitType == UnitType.SuicideBomber)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(start, m_stats.explosionRadius);
        }
    }
}