using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private UnitStats m_stats;
    [SerializeField]
    private LayerMask m_targetLayer;
    [SerializeField]
    private Vector2 m_direction = Vector2.right;
    public UnitHealthBar m_healthBar;

    private float m_currentHealth;
    private float m_lastAttackTime;
    private bool m_isDead = false;

    private Rigidbody2D m_rb;
    private UnitController m_currentTarget;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_currentHealth = m_stats.maxHealth;
        if (m_healthBar != null)
        {
            m_healthBar.UpdateHealthBar(m_currentHealth, m_stats.maxHealth);
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
                Attack();
            }
        }
        else
        {
            Move();
        }
    }

    void Move()
    {
        m_rb.linearVelocity = m_direction * m_stats.speed;
    }

    void StopMoving()
    {
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

        m_currentTarget = null;
        return false;
    }

    void Attack()
    {
        m_lastAttackTime = Time.time;

        if (m_currentTarget != null)
        {
            m_currentTarget.TakeDamage(m_stats.damage);
        }
    }

    public void TakeDamage(float amount)
    {
        m_currentHealth -= amount;

        if (m_healthBar != null)
        {
            m_healthBar.UpdateHealthBar(m_currentHealth, m_stats.maxHealth);
        }

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        m_isDead = true;
        Destroy(gameObject);
    }
    void OnDrawGizmos()
    {
        if (m_stats != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)m_direction * m_stats.attackRange);
        }
    }
}