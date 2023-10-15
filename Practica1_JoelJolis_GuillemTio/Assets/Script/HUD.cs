using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI m_LoadedAmmoText;
    public TextMeshProUGUI m_OtherAmmoText;

    public void SetLoadedAmmoText(int ammo)
    {
        m_LoadedAmmoText.text = ammo.ToString();
    }

    public void SetOtherAmmoText(int ammo)
    {
        m_OtherAmmoText.text = ammo.ToString();
    }
}
