using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisBeamBehavior : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> m_RayPoints_Debug = new();

    private StatisObject m_CurrentlyFreezedObject;

    void Update()
    {
        if (m_RayPoints_Debug.Count == 1)
            return;

        for (int i = 0; i < m_RayPoints_Debug.Count - 1; i++)
        {
            Debug.DrawLine(m_RayPoints_Debug[i], m_RayPoints_Debug[i + 1]);
        }
    }

    public void ShootBeam(Vector2 direction, float distance)
    {
        m_RayPoints_Debug.Clear();

        m_RayPoints_Debug.Add(transform.position);
        // Get direction vector
        Vector3 v = new(direction.x, direction.y, 0.0f);
        Debug.DrawLine(transform.position, transform.position + v * distance);

        // Cast ray from the player to the aim direction
        bool collision = Physics.Raycast(transform.position, v,
            out RaycastHit hit, distance, int.MaxValue, QueryTriggerInteraction.Ignore);

        // Check has collided
        if (!collision)
        {
            CheckUnfreezeCurrentObject();
            return;
        }

        //m_RayPoints_Debug.Add(hit.point);

        float distanceLeft = distance - hit.distance;
        for (int i = 0; i < 10; i++)
        {
            // Check has a mirror
            if (hit.transform.TryGetComponent(out MirrorBehavior mirror))
            {
                m_RayPoints_Debug.Add(hit.point);
                // Try to reflect the ray
                if (!mirror.Reflect(ref hit, direction, distanceLeft, out direction))
                {
                    CheckUnfreezeCurrentObject();
                    return; // Didn't hit anything, abort
                }
                else
                {
                    m_RayPoints_Debug.Add(hit.point);
                    // Update distance left by distance traveled
                    distanceLeft -= hit.distance;
                    if (distanceLeft <= 0)
                        break; // No more distance left, stop processing
                }
            }
            else
            {
                // Didn't hit a mirror, stop processing
                break;
            }
        }

        m_RayPoints_Debug.Add(hit.point);

        // Check if the object that was hit has a StatisObject script
        if (hit.transform.TryGetComponent(out StatisObject so))
        {
            if (m_CurrentlyFreezedObject != so)
                CheckUnfreezeCurrentObject();
            m_CurrentlyFreezedObject = so;

            // Has a script, so can be frozen
            so.SetFreezeState(!so.IsFreezed());
        }
        else
        {
            CheckUnfreezeCurrentObject();
        }
    }

    private void CheckUnfreezeCurrentObject()
    {
        if (m_CurrentlyFreezedObject != null && m_CurrentlyFreezedObject.IsFreezed())
        {
            m_CurrentlyFreezedObject.SetFreezeState(false);
            m_CurrentlyFreezedObject = null;
        }
    }
}
