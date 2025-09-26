using UnityEngine;

// This line makes sure that a GameObject with this script also has an AudioSource component.
// It's useful because we want to play a sound from this script.
[RequireComponent(typeof(AudioSource))]
public class GameManager : MonoBehaviour
{
    // This will hold the current letter we are tracing. We can set this in the Unity Inspector.
    // For example, if the scene is for letter 'A', we would drag the 'A' GameObject here.
    [SerializeField] private LetterController m_CurrentLetter;

    // This is for the 'You Win!' panel that shows up when the letter is completely traced.
    // We can drag our UI panel to this slot in the Inspector.
    [SerializeField] private GameObject m_WinPanel;

    // This section is for sounds.
    [Header("Audio")]
    // This is the sound that plays when the whole letter is finished. A "success" or "congratulations" sound.
    [SerializeField] private AudioClip m_LetterCompletedSound;

    // This will hold the AudioSource component so we can play sounds.
    // We don't need to set this in the inspector because our script will find it.
    private AudioSource m_AudioSource;

    // Start is called before the first frame update. It's a good place to set things up.
    private void Start()
    {
        // We check if a letter has been assigned in the Inspector.
        if (m_CurrentLetter != null)
        {
            // If there is a letter, we want to know when it's completed.
            // 'OnLetterCompleted' is an event in the LetterController script.
            // When that event happens, we want to call our 'ShowWinPanel' method.
            // This line connects the event to our method.
            m_CurrentLetter.OnLetterCompleted.AddListener(ShowWinPanel);
        }
        else
        {
            // If we forgot to assign a letter in the Inspector, this message will remind us.
            Debug.LogError("Current Letter is not assigned in the GameManager. Please assign it in the Inspector.", this);
        }

        // We make sure the win panel is hidden when the game starts.
        if (m_WinPanel != null)
        {
            m_WinPanel.SetActive(false);
        }

        // This gets the AudioSource component that is on the same GameObject as this script.
        // The [RequireComponent] line at the top makes sure it's there.
        m_AudioSource = GetComponent<AudioSource>();
    }

    // This method is called when the letter is completed.
    private void ShowWinPanel()
    {
        // A message for us to see in the Unity console to know this is working.
        Debug.Log("GameManager: Letter completed! Showing Win Panel.");
        // If we have a win panel assigned, we make it visible.
        if (m_WinPanel != null)
            m_WinPanel.SetActive(true);

        // If we have assigned a completion sound and we have our AudioSource, we play the sound.
        // 'PlayOneShot' is good for sound effects that don't need to loop.
        if (m_LetterCompletedSound != null && m_AudioSource != null)
        {
            m_AudioSource.PlayOneShot(m_LetterCompletedSound);
        }
    }
}
