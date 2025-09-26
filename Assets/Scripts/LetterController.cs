using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LetterController : MonoBehaviour
{
    // This is a list of all the parts that make up our letter.
    // We need to drag them into the Inspector in the exact order we want the player to trace them.
    // For example, for the letter 'A', it might be the left stroke, then the right stroke, then the crossbar.
    [SerializeField] private List<TracePart> m_TraceParts;

    // This is an event that other scripts can listen to.
    // When the whole letter is finished, we'll trigger this event.
    // The GameManager script listens to this to know when to show the 'You Win!' panel.
    [Header("Events")]
    public UnityEvent OnLetterCompleted;

    // This keeps track of which part of the letter we are currently tracing.
    // It starts at 0, which is the first part in our list.
    private int m_CurrentPartIndex = 0;

    // Start is called just before the first frame update.
    void Start()
    {
        InitializeLetter();
    }

    // This function sets up the letter at the beginning.
    private void InitializeLetter()
    {
        // We start with the first part.
        m_CurrentPartIndex = 0;

        // We go through each part of the letter to set it up.
        for (int i = 0; i < m_TraceParts.Count; i++)
        {
            var part = m_TraceParts[i];
            part.gameObject.SetActive(true); // Make sure the part's GameObject is active.
            // We disable the 'TracePart' script on all parts at first.
            // This stops the player from tracing them out of order.
            part.enabled = false;
            part.OnTraceCompleted.RemoveListener(OnPartCompleted); // Good practice to remove old listeners first.
            // When a part is completed, it will trigger its 'OnTraceCompleted' event.
            // We want our 'OnPartCompleted' method to be called when that happens.
            part.OnTraceCompleted.AddListener(OnPartCompleted);
        }

        // If we have at least one part, we get the first one ready to be traced.
        if (m_TraceParts.Count > 0)
        {
            ActivateCurrentPart();
        }
    }

    // This function gets the next part ready for tracing.
    private void ActivateCurrentPart()
    {
        // We check if there are still parts left to trace.
        if (m_CurrentPartIndex < m_TraceParts.Count)
        {
            // We enable the script on the current part, so the player can interact with it.
            m_TraceParts[m_CurrentPartIndex].enabled = true;
            // We also call its 'Activate' method, which makes it visible and ready.
            m_TraceParts[m_CurrentPartIndex].Activate();
        }
    }

    // This method is called whenever a single part of the letter is successfully traced.
    private void OnPartCompleted()
    {
        // We move to the next part in our list.
        m_CurrentPartIndex++;
        // If we have finished all the parts...
        if (m_CurrentPartIndex >= m_TraceParts.Count)
        {
            // ...the whole letter is complete! We trigger the 'OnLetterCompleted' event.
            Debug.Log("Letter Completed!");
            OnLetterCompleted.Invoke();
        }
        else
        {
            // Otherwise, we activate the next part in the sequence.
            ActivateCurrentPart();
        }
    }
}