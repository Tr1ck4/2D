using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel; 

    public void PlayGame()
    {
        SceneManager.LoadScene("Outdoor");
    }

    public void Options()
    {
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed. Exiting the game.");
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
