using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextSceneName;

    public string InteractionPrompt => "[E] Enter";

    private bool playerInRange;

    private void Update()
    {
        if (playerInRange &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (SceneScript.Instance != null)
        {
            SceneScript.Instance.NextScene(nextSceneName);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}