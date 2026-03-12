using UnityEngine;

public class DamageZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
        if (playerRespawn == null)
            return;

        GameManager.Instance.LoseLife();

        if (GameManager.Instance.GetLives() > 0)
        {
            playerRespawn.Respawn();
        }
    }
}