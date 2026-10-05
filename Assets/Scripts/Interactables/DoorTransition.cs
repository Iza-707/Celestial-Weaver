using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoorTransition : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextSceneName;
    [Header("Door Visuals")]
    [SerializeField] private GameObject lockedDoorVisual;
    [SerializeField] private GameObject openDoorVisual;
    
    [Header("Exit Settings")]
[SerializeField] private bool requireCompletion = false;

    private bool isUnlocked = false;
    private bool playerInRange = false;

    public string InteractionPrompt
    {
        get
        {
            if (!isUnlocked)
                return "[E] Locked";

            return "[E] Enter";
        }
    }

    private void Start()
    {
        isUnlocked = !requireCompletion;

        if (lockedDoorVisual != null)
            lockedDoorVisual.SetActive(requireCompletion);

        if (openDoorVisual != null)
            openDoorVisual.SetActive(!requireCompletion);
    }

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
        if (!isUnlocked && requireCompletion)
        {
            Debug.Log("The dungeon door is locked.");
            return;
        }

        Debug.Log("Entering next area...");

        if (SceneScript.Instance != null)
            SceneScript.Instance.NextScene(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }

    public void UnlockDoor()
    {
        isUnlocked = true;

        if (lockedDoorVisual != null)
            lockedDoorVisual.SetActive(false);

        if (openDoorVisual != null)
            openDoorVisual.SetActive(true);

        Debug.Log("Dungeon door unlocked!");
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