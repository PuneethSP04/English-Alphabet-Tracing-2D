using UnityEngine;

// This script is for a single waypoint (a small dot) on the tracing path.
// It just controls whether the dot is visible or not.
public class Waypoint : MonoBehaviour
{
    // This is the little green dot or checkmark that appears when the player traces over this waypoint.
    // We should drag the visual part of our waypoint prefab here in the Inspector.
    [SerializeField] private GameObject m_WaypointVisual;

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        if (m_WaypointVisual == null)
        {
            // If we forgot to assign the visual part, this will remind us.
            Debug.LogWarning("Waypoint Visual is not assigned in the inspector!", this);
            return;
        }
        // Make sure the waypoint's visual is hidden when the game starts.
        m_WaypointVisual.SetActive(false);
    }

    // This function makes the waypoint's visual (the green dot) appear.
    public void Activate()
    {
        if (m_WaypointVisual != null)
            m_WaypointVisual.SetActive(true);
    }

    // This function hides the waypoint's visual.
    public void Deactivate()
    {
        if (m_WaypointVisual != null)
            m_WaypointVisual.SetActive(false);
    }
}