using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This function will be called when the Play button is pressed
    public void PlayGame()
    {
        // Load the scene named "Indoor"
        SceneManager.LoadScene("Indoor");
    }

    // This function will be called when the Options button is pressed
    public void Options()
    {
        // Currently, this function does nothing
        Debug.Log("Options button pressed. This button currently does nothing.");
    }

    // This function will be called when the Quit button is pressed
    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit button pressed. Exiting the game.");
        Application.Quit();

        // If running in the Unity editor, stop playing the scene
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
