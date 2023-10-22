using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMover : MonoBehaviour
{
    public float m_ScrollSpeedX;
    public float m_ScrollSpeedY;
    MeshRenderer m_MeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float l_offsetX = m_MeshRenderer.material.mainTextureOffset.x + m_ScrollSpeedX*Time.deltaTime;
        float l_offsetY = m_MeshRenderer.material.mainTextureOffset.y + m_ScrollSpeedY*Time.deltaTime;

        if (l_offsetX > 1)
            l_offsetX -= 1;
        if (l_offsetY > 1)
            l_offsetY -= 1;

        m_MeshRenderer.material.mainTextureOffset = new Vector2(l_offsetX, l_offsetY);
    }
}
