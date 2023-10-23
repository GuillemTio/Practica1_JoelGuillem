using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPoolElements 
{
    List<GameObject> m_Elements;
    int m_CurrentElementsId = 0;

    public CPoolElements(int ElementsCount, GameObject Prefab, Transform Parent)
    {
        m_Elements = new List<GameObject>();
        for (int i=0; i < ElementsCount; ++i)
        {
            GameObject l_Instance = GameObject.Instantiate(Prefab);
            l_Instance.transform.SetParent(Parent);
            m_Elements.Add(l_Instance);
            GameObject.DontDestroyOnLoad(l_Instance);
        }
    }
    public GameObject GetNextElement()
    {
        GameObject l_Element = m_Elements[m_CurrentElementsId];
        ++m_CurrentElementsId;
        if (m_CurrentElementsId >= m_Elements.Count)
            m_CurrentElementsId = 0;
        return l_Element;
    }
}
