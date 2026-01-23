using UnityEngine;
using UnityEngine.UI;

public class UnitHealthBar : MonoBehaviour
{
    [SerializeField]
    private Image m_fillImage;
    [SerializeField]
    private GameObject m_parent;

    private void Start()
    {
        Desactivate();
    }

    public void Desactivate()
    {
        if (m_parent != null && m_parent.activeSelf)
            m_parent.SetActive(false);
    }

    public void Activate()
    {
        if (m_parent != null && !m_parent.activeSelf)
            m_parent.SetActive(true);
    }

    public void ChangeFillColor(string _team)
    {
        if (_team.Contains("Player"))
            m_fillImage.color = Color.green;
        else
            m_fillImage.color = Color.red;
    }

    public void UpdateHealthBar(float _currentHealth, float _maxHealth)
    {
        if (m_fillImage == null)
            return;

        if (_currentHealth < _maxHealth)
            Activate();

        m_fillImage.fillAmount = _currentHealth / _maxHealth;
    }
}