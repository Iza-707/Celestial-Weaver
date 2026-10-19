using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    public static SceneScript Instance { get; private set; }

    public Animator anim;
    public string nextScene;

    [Header("Audio")]
    public AudioClip transitionSFX;

    [Range(0f, 1f)]
    [SerializeField] private float transitionSFXVolume = 0.5f;
    

    private void Awake()
    {
        Instance = this;
    }

    public void NextScene(string scene)
    {
        nextScene = scene;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(transitionSFX, transitionSFXVolume);

        if (anim != null)
        {
            StartCoroutine(PlayTransitionAndLoad());
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    private IEnumerator PlayTransitionAndLoad()
    {
        // Trigger the fade-to-black animation
        anim.SetTrigger("In");

        // Wait until the "in" state actually begins
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).IsName("in")
        );

        // Wait until the fade animation finishes
        yield return new WaitUntil(() =>
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
        );

        // Load the requested scene
        SceneManager.LoadScene(nextScene);
    }
}