using UnityEngine;

public class Key : MonoBehaviour
{
    public Door m_DoorToOpen;

    public void OpenKeyDoor()
    {

        m_DoorToOpen.OpenDoor();
        GameObject.Destroy(gameObject);
    }
}
