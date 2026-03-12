using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;
    private Rigidbody rb;

    private void Start()
    {
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

    public void Respawn()
    {
        transform.position = respawnPoint;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}