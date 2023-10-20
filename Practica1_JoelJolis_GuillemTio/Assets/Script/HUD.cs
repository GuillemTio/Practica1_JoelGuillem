using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI m_LoadedAmmoText;
    public TextMeshProUGUI m_OtherAmmoText;

    public TextMeshProUGUI m_ScorePointsText;
    public TextMeshProUGUI m_ScoreTimer;

    public Slider m_HealthBar;
    public Slider m_ShieldBar;
    public TextMeshProUGUI m_HealthText;
    public TextMeshProUGUI m_ShieldText;

    private void Awake()
    {
        if (GameController.GetGameController().m_HUD == null)
        {
            GameController.GetGameController().m_HUD = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    internal void SetLoadedAmmoText(int ammo)
    {
        m_LoadedAmmoText.text = ammo.ToString();
    }

    internal void SetOtherAmmoText(int ammo)
    {
        m_OtherAmmoText.text = ammo.ToString();
    }

    internal void SetHealthBar(float health)
    {
        m_HealthBar.value = Mathf.Clamp(health, 0, 100);
    }

    internal void SetShieldBar(float shield)
    {
        m_ShieldBar.value = shield;
        if (m_ShieldBar.value == 0)
        {
            m_ShieldBar.fillRect.gameObject.SetActive(false);
        }
    }
    internal void SetHealthText(float HealthCurrent)
    {
        m_HealthText.text = HealthCurrent.ToString();
    }

    internal void SetShieldText(float ShieldCurrent)
    {
        m_ShieldText.text = ShieldCurrent.ToString();
    }

    internal void SetScorePoints(float points)
    {
        m_ScorePointsText.text = points.ToString();
    }

    internal void SetScoreTimer(int timer)
    {
        m_ScoreTimer.text = timer.ToString();
    }

}
