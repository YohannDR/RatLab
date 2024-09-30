using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform m_ObjectToFollow;
    [SerializeField] private Vector3 m_Offset;

    void Update()
    {
        transform.position = m_ObjectToFollow.position + m_Offset;
    }
}
