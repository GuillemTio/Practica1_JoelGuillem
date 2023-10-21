using UnityEngine;

public class HitCollider : MonoBehaviour
{
    const int m_HeadLifePoints = 50;
    const int m_HelixLifePoints = 25;
    const int m_BodyLifePoints = 0;
    public enum THitColliderType
    {
        HEAD = 0,
        HELIX,
        BODY
    }

    public THitColliderType m_HitColliderType;
    public Enemy m_Enemy;

    public void Hit()
    {
        int l_LifePoints = m_HeadLifePoints;

        if(m_HitColliderType == THitColliderType.HELIX)
        {
            l_LifePoints = m_HelixLifePoints;
        }
        else if (m_HitColliderType == THitColliderType.BODY)
        {
            l_LifePoints = m_BodyLifePoints;
        }

        m_Enemy.Hit(l_LifePoints);
    }

}
