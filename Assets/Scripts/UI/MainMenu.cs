using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    public static bool showAnim = true;

    public GameObject anim;
    public GameObject end;

    private AudioSource source;
    
    private void Awake() => Instance = this;
        
    private void Start()
    {
        source = GetComponent<AudioSource>();
        anim.SetActive(showAnim);
    }

    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneIndex);
    }

    public void HoverSound()
    {
        source.pitch = Random.Range(0.8f,1.2f);
        source.Play();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Disable()
    {
        showAnim = false;
        anim.SetActive(showAnim);
    }

    public void End()
    {
        end.SetActive(false);
    }
}