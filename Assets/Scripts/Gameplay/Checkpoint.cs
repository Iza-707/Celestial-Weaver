using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") ||
            GameManager.Instance == null)
            return;

        GameManager.Instance.SetCheckpoint(respawnPoint);

        Debug.Log("Checkpoint activated!");
    }
}