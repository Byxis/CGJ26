using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private UnitStats m_stats;
    public UnitStats Stats => m_stats;

    public void SetStats(UnitStats stats)
    {
        m_stats = stats;
    }
    [SerializeField]
    private UnitHealthBar m_healthBar;
    private LayerMask m_targetLayer;
    private Vector2 m_direction = Vector2.right;

    private float m_currentHealth;
    private float m_lastAttackTime;
    private Rigidbody2D m_rb;
    private UnitController m_currentTarget;
    private UnitController m_lockedTarget;
    private bool m_isDead = false;

    private float m_baseSpeed;
    private float m_speedMultiplier = 1f;

    private int m_clicksReceived = 0;
    private bool m_isKnockedBack = false;
    private bool m_isJumping = false;

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
            m_targetLayer = LayerMask.GetMask("TeamEnemy");
        }
        else
        {
            m_direction = Vector2.left;
            m_targetLayer = LayerMask.GetMask("TeamPlayer");
        }
    }

    void FixedUpdate()
    {
        if (m_isDead)
            return;

        if (m_stats.unitType == UnitType.Pusher && m_lockedTarget == null && m_currentTarget != null)
        {
            m_currentTarget = null;
        }

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
        if (m_isKnockedBack || m_stats.unitType == UnitType.Base)
            return;

        if (m_stats.unitType == UnitType.Jumping)
        {
            if (!m_isJumping)
            {
                StartCoroutine(JumpRoutine());
            }
        }
        else
        {
            m_rb.linearVelocity = m_direction * (m_baseSpeed * m_speedMultiplier);
        }
    }

    void StopMoving()
    {
        if (m_isKnockedBack)
            return;

        if (m_stats.unitType != UnitType.Jumping)
        {
            m_rb.linearVelocity = Vector2.zero;
        }
    }

    bool CheckForTarget()
    {
        int layerToDetection = m_targetLayer.value;

        if (m_stats.unitType == UnitType.Healer)
        {
            layerToDetection |= (1 << gameObject.layer);
        }

        RaycastHit2D[] hits =
            Physics2D.RaycastAll(transform.position, m_direction, m_stats.attackRange, layerToDetection);

        if (m_stats.unitType == UnitType.Pusher && m_lockedTarget != null)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == m_lockedTarget.gameObject)
                {
                    m_currentTarget = m_lockedTarget;
                    return true;
                }
            }
            return false;
        }

        foreach (var hit in hits)
        {
            if (hit.collider.gameObject != gameObject)
            {
                if (m_stats.unitType == UnitType.Pusher)
                {
                    if (hit.collider.gameObject.layer == gameObject.layer)
                        continue;

                    m_lockedTarget = hit.collider.GetComponent<UnitController>();
                    if (m_lockedTarget != null)
                    {
                        m_currentTarget = m_lockedTarget;
                        return true;
                    }
                    continue;
                }

                UnitController target = hit.collider.GetComponent<UnitController>();
                if (target != null)
                {
                    if (m_stats.unitType == UnitType.Healer && target.m_stats.unitType == UnitType.Base &&
                        target.gameObject.layer == gameObject.layer)
                        continue;

                    m_currentTarget = target;
                    return true;
                }
            }
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

        if (m_stats.unitType == UnitType.Pusher)
        {
            if (m_currentTarget != null && m_currentTarget == m_lockedTarget)
            {
                if (m_currentTarget.m_stats.unitType == UnitType.Base)
                {
                    Die();
                    return;
                }

                m_currentTarget.TakeDamage(m_stats.damage);
                m_currentTarget.ApplyEffect(SpecialEffect.Knockback, 0.4f, m_stats.effectValue, m_direction);

                Collider2D[] nearby = Physics2D.OverlapCircleAll(m_currentTarget.transform.position, 1.5f);
                foreach (var col in nearby)
                {
                    UnitController uc = col.GetComponent<UnitController>();
                    if (uc != null && uc.m_stats.unitType == UnitType.Base &&
                        uc.gameObject.layer == m_currentTarget.gameObject.layer)
                    {
                        Die();
                        return;
                    }
                }
            }
            return;
        }

        if (m_stats.unitType == UnitType.Summoner)
        {
            InvokeUnit();
            return;
        }

        if (m_currentTarget != null)
        {
            if (m_stats.projectilePrefab != null)
            {
                LaunchProjectile();
            }
            else
            {
                if (m_stats.unitType == UnitType.Healer)
                {
                    if (m_currentTarget.gameObject.layer == gameObject.layer)
                        m_currentTarget.Heal(m_stats.damage);
                    else
                        m_currentTarget.TakeDamage(m_stats.damage);
                }
                else
                {
                    m_currentTarget.TakeDamage(m_stats.damage);
                    if (m_stats.effectOnHit != SpecialEffect.None)
                    {
                        m_currentTarget.ApplyEffect(
                            m_stats.effectOnHit, m_stats.effectDuration, m_stats.effectValue, m_direction);
                    }
                }
            }
        }
    }

    void LaunchProjectile()
    {
        GameObject projObj = Instantiate(m_stats.projectilePrefab, transform.position, Quaternion.identity);
        Projectile projScript = projObj.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Setup(m_currentTarget, m_stats, m_direction);
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

    IEnumerator JumpRoutine()
    {
        m_isJumping = true;

        Vector2 jumpDirection = (m_direction + Vector2.up).normalized;
        m_rb.AddForce(jumpDirection * (m_baseSpeed * 1.5f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(0.4f);

        m_isJumping = false;
    }

    void Die()
    {
        Debug.Log($"[UnitController] Die() called for {gameObject.name}, unitType = {m_stats.unitType}");
        m_isDead = true;

        if (m_stats.unitType == UnitType.Base)
        {
            Debug.Log($"[UnitController] Base detected! GameManager.Instance = {GameManager.Instance}");
            if (GameManager.Instance != null)
            {
                Debug.Log("[UnitController] Calling GameManager.Instance.OnBaseDestroyed()");
                GameManager.Instance.OnBaseDestroyed(this);
            }
            else
            {
                Time.timeScale = 0f;
                Debug.Log("GAME OVER - Base Detruite (GameManager manquant) !");
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
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
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_stats == null)
            return;

        if (m_stats.unitType == UnitType.Pusher)
        {
            UnitController other = collision.gameObject.GetComponent<UnitController>();
            if (other != null && other != m_lockedTarget)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
            }
        }
    }

    void InvokeUnit()
    {
        if (m_stats.invocablePrefab != null)
        {
            float offset = -2f;
            for (int i = 0; i < m_stats.numberOfInvocable; i++)
            {
                GameObject g = Instantiate(m_stats.invocablePrefab, transform.position, Quaternion.identity);
                g.layer = gameObject.layer;
                g.transform.position = transform.position + new Vector3(offset, 0, 0);
                offset += 0.5f;
            }
        }
    }
}