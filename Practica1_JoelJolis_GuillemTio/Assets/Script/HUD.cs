using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI m_LoadedAmmoText;
    public TextMeshProUGUI m_OtherAmmoText;

    public TextMeshProUGUI m_ScorePointsText;

    public Slider m_HealthBar;
    public Slider m_ShieldBar;

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

    public void SetLoadedAmmoText(int ammo)
    {
        m_LoadedAmmoText.text = ammo.ToString();
    }

    public void SetOtherAmmoText(int ammo)
    {
        m_OtherAmmoText.text = ammo.ToString();
    }

    public void SetHealthBar(float health)
    {
        m_HealthBar.value = Mathf.Clamp(health, 0, 100);
    }

    public void SetShieldBar(float shield)
    {
        m_ShieldBar.value = Mathf.Clamp(shield, 0, 100);
        if (m_ShieldBar.value == 0)
        {
            m_ShieldBar.fillRect.gameObject.SetActive(false);
        }
    }

    public void SetScorePoints(float points)
    {
        //m_ScorePointsText
    }
}
