using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Spike : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.TryGetComponent(out PlayerBehavior player))
        {
            player.Kill();
        }
    }
}
