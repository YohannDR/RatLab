using UnityEngine;

public class MirrorBehavior : MonoBehaviour
{
    public bool Reflect(ref RaycastHit hit, Vector2 direction, float maxDist, out Vector2 newDir)
    {
        // Compute new direction
        newDir = Vector2.Reflect(direction, hit.normal);

        // Compute remaining distance
        float distance = maxDist - hit.distance;

        StasisParticlesManager.Instance.PlayStasisBeam(hit.point, newDir);

        // Cast a ray from the hitpoint,
        // using the reflected direction that lasts the remaining distance
        bool res = Physics.Raycast(hit.point, newDir, out hit, distance,
            int.MaxValue, QueryTriggerInteraction.Ignore);

        AudioManager.Instance.PlaySound("Mirror_reflect", transform.position);

        return res;
    }
}
