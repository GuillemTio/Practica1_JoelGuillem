using UnityEngine;

public class HealthItem : Item
{
    public int m_HealthCount;

    public override bool CanPick()
    {
        return GameController.GetGameController().m_Player.CanPickAmmo();
    }

    public override void Pick()
    {
        GameController.GetGameController().m_Player.AddAmmo(m_AmmoCount);
        base.Pick();
    }
}
