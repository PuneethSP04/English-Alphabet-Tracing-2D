using UnityEngine;
using UnityEngine.SceneManagement;

// This script helps us load different scenes in our game.
// We can attach these methods to buttons in our UI.
public class SceneLoader : MonoBehaviour
{
    // This function will be called when we want to go to the 'Letter A' scene.
    public void LoadLetterA()
    {
        // 'SceneManager.LoadScene' is Unity's way of loading a new scene.
        // We just need to give it the name of the scene we want to load.
        SceneManager.LoadScene("Letter A");
    }

    // This one loads the 'Letter B' scene.
    public void LoadLetterB()
    {
        SceneManager.LoadScene("Letter B");
    }

    // This one loads the 'Letter C' scene.
    public void LoadLetterC()
    {
        SceneManager.LoadScene("Letter C");
    }

    // And this one takes us back to the 'Home' or main menu scene.
    public void LoadHome()
    {
        SceneManager.LoadScene("Home");
    }
}