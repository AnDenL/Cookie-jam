using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;
    private AudioSource source;
    
    private void Awake() => Instance = this;
        
    private void Start()
    {
        source = GetComponent<AudioSource>();
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
}