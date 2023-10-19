using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldCount;

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
