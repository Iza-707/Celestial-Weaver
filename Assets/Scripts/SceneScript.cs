using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
    public static SceneScript Instance { get; private set; }
    public Animator anim;
    public string nextScene;

    private void Awake()
    {
        SceneScript.Instance = this;
    }

    public void NextScene(string scene)
    {
        nextScene = scene;
        anim.SetTrigger("In");
    }

    public void LoadScene(string s) {
    SceneManager.LoadScene(nextScene);
    }
}
