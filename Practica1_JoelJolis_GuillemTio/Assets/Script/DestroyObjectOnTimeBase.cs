using UnityEngine;
using System.Collections;
public class DestroyObjectOnTimeBase : MonoBehaviour
{
    public float m_DestroyOnTime;

    public void Start()
    {
        StartCoroutine(DestroyOnTimeFn());
    }
    IEnumerator DestroyOnTimeFn()
    {
        yield return new WaitForSeconds(m_DestroyOnTime);
        GameObject.Destroy(gameObject);
    }
}