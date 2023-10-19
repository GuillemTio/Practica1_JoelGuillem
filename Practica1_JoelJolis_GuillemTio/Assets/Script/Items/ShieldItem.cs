using UnityEngine;

public class ShieldItem : Item
{
    public int m_ShieldCount;

    public override bool CanPick()
    {
        return GameController.GetGameController().m_Player.CanPickShield();
    }

    public override void Pick()
    {
        GameController.GetGameController().m_Player.AddShield(m_ShieldCount);
        base.Pick();
    }
}
