using UnityEngine;

public class AmmoItem : Item
{
    public int m_AmmoCount;

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
