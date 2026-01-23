using UnityEngine;

public class Projectile : MonoBehaviour
{
    private UnitController m_target;
    private UnitStats m_stats;
    private Vector2 m_direction;
    private float m_speed = 15f;

    public void Setup(UnitController _target, UnitStats _stats, Vector2 _direction)
    {
        m_target = _target;
        m_stats = _stats;
        m_direction = _direction;
    }

    void Update()
    {
        if (m_target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position =
            Vector3.MoveTowards(transform.position, m_target.transform.position, m_speed * Time.deltaTime);

        Vector3 diff = m_target.transform.position - transform.position;
        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Vector3.Distance(transform.position, m_target.transform.position) < 0.2f)
        {
            OnHit();
        }
    }

    void OnHit()
    {
        if (m_target != null)
        {
            if (m_stats.unitType == UnitType.Healer)
            {
                if (m_target.gameObject.layer == gameObject.layer)
                    m_target.Heal(m_stats.damage);
                else
                    m_target.TakeDamage(m_stats.damage);
            }
            else
            {
                m_target.TakeDamage(m_stats.damage);

                if (m_stats.effectOnHit != SpecialEffect.None)
                {
                    m_target.ApplyEffect(m_stats.effectOnHit, m_stats.effectDuration, m_stats.effectValue, m_direction);
                }
            }
        }

        Destroy(gameObject);
    }
}
