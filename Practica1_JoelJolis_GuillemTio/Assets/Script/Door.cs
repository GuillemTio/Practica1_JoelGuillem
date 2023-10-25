using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        AUTOMATIC = 0,
        POINTS_NECESSARY,
        KEY_NECESSARY
    }

    public DoorType m_DoorType;
    public float m_AutomaticDoorDistance;
    bool m_IsDoorOpen = false;
    public LayerMask m_LayerMask;
    public Transform m_RayTransform;

    public Animation m_DoorAnimation;
    public AnimationClip m_OpenDoorAnimationClip;
    public AnimationClip m_CloseDoorAnimationClip;
    public AnimationClip m_StayOpenAnimationClip;
    public AnimationClip m_StayClosedAnimationClip;

    private void Start()
    {
        m_DoorAnimation.CrossFade(m_StayClosedAnimationClip.name);
    }
    void Update()
    {
        switch (m_DoorType)
        {
            case DoorType.AUTOMATIC:
                UpdateAutomatic();
                break;
            case DoorType.POINTS_NECESSARY:
                UpdatePoints_Necessary();
                break;
            case DoorType.KEY_NECESSARY:
                UpdateKey_Necessary();
                break;
            default:
                break;
        }
    }

    void UpdateAutomatic()
    {
        if(!m_IsDoorOpen && IsPlayerNear())
        {
            m_IsDoorOpen = true;
            OpenDoor();
        }
        else if (m_IsDoorOpen && !IsPlayerNear())
        {
            m_IsDoorOpen = false;
            CloseDoor();
        }
    }

    void UpdatePoints_Necessary()
    {
        if((GameController.GetGameController().m_Player.m_PointsAchieved) && !m_IsDoorOpen)
        {
            m_IsDoorOpen = true;
            OpenDoor();
        }
      
    }

    void UpdateKey_Necessary()
    {

    }

    bool IsPlayerNear()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().m_Player.transform.position;
        Vector3 l_DoorPosition = m_RayTransform.position;
        l_PlayerPosition.y += GameController.GetGameController().m_Player.GetComponent<CharacterController>().height / 2;

        Vector3 l_DoorToPlayer = l_PlayerPosition - l_DoorPosition;
        l_DoorToPlayer.Normalize();

        Ray l_Ray = new Ray(l_DoorPosition, l_DoorToPlayer);
        Debug.DrawRay(m_RayTransform.position, l_DoorToPlayer * m_AutomaticDoorDistance, Color.yellow);
        return Physics.Raycast(l_Ray, m_AutomaticDoorDistance,m_LayerMask);
    }

    internal void OpenDoor()
    {
        m_DoorAnimation.CrossFade(m_OpenDoorAnimationClip.name, 0.1f);
        m_DoorAnimation.CrossFadeQueued(m_StayOpenAnimationClip.name, 0.1f);
    }

    private void CloseDoor()
    {
        m_DoorAnimation.CrossFade(m_CloseDoorAnimationClip.name, 0.1f);
        m_DoorAnimation.CrossFadeQueued(m_StayClosedAnimationClip.name, 0.1f);
    }
}
