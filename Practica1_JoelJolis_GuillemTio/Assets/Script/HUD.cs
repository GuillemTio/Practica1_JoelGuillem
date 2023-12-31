using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI m_LoadedAmmoText;
    public TextMeshProUGUI m_OtherAmmoText;

    public TextMeshProUGUI m_EnterText;
    public TextMeshProUGUI m_ExitText;
    public TextMeshProUGUI m_ScorePointsText;
    public TextMeshProUGUI m_ScoreTimer;
    public GameObject m_InspectorScoreTimer;
    public GameObject m_InspectorScorePoints;
    public GameObject m_InspectorEnterText;
    public GameObject m_InspectorExitText;

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

            m_InspectorScoreTimer.SetActive(false);
            m_InspectorScorePoints.SetActive(false);
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
        m_InspectorScorePoints.SetActive(true);
        m_ScorePointsText.text = "Score: "+ points.ToString();
    }

    internal void SetScoreTimer(float timer)
    {
        m_InspectorScoreTimer.SetActive(true);
        m_ScoreTimer.text = "Time: " + timer.ToString("0");
    }

    internal void EnableEnterText()
    {
        m_InspectorEnterText.SetActive(true);
    }

    internal void EnableExitText()
    {
        m_InspectorExitText.SetActive(true);
    }

    internal void DisableScoreSystem()
    {
        m_InspectorScoreTimer.SetActive(false);
        m_InspectorScorePoints.SetActive(false);
    }

    internal void DisableEnterText()
    {
        m_InspectorEnterText.SetActive(false);
    }

    internal void DisableExitText()
    {
        m_InspectorExitText.SetActive(false);
    }

}
