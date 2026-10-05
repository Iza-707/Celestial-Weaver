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
            // Catalogue doors open the catalogue UI instead of changing scenes
        if (gameObject.CompareTag("Zodiac_Catalogue") ||
            gameObject.CompareTag("Navigation_Catalogue") ||
            gameObject.CompareTag("Legends_Catalogue"))
        {
            OpenCataloguePanel();
            return;
        }

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

    private void OpenCataloguePanel()
{
    if (CataloguePanelController.Instance == null)
        return;

    switch (gameObject.tag)
    {
        case "Zodiac_Catalogue":
            CataloguePanelController.Instance.OpenCatalogue("ZODIAC CATALOGUE");
            break;

        case "Navigation_Catalogue":
            CataloguePanelController.Instance.OpenCatalogue("NAVIGATION CATALOGUE");
            break;

        case "Legends_Catalogue":
            CataloguePanelController.Instance.OpenCatalogue("LEGENDS & MYTHS");
            break;

        default:
            Debug.LogWarning("Unknown catalogue door tag: " + gameObject.tag);
            break;
    }
}
}