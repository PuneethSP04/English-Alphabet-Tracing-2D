using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

// This line makes sure that a GameObject with this script also has a PolygonCollider2D.
// The collider is used to detect when the mouse clicks on this part.
[RequireComponent(typeof(PolygonCollider2D))]
public class TracePart : MonoBehaviour
{
    // This section is for how the trace part looks.
    [Header("Visuals")]
    // This is the solid color that the part will be when it's successfully traced.
    [SerializeField] private Color m_CompletedColor = new Color(0.4f, 0.25f, 0.7f, 1f);
    // This is the see-through color that shows the player where to trace.
    [SerializeField] private Color m_GuideColor = new Color(0.4f, 0.25f, 0.7f, 0.4f);
    // This is the sprite that gets filled with color as the player traces.
    [SerializeField] private SpriteRenderer m_FillSprite;
    // This holds all the little waypoint dots that make up the path to trace.
    [SerializeField] private Transform m_WaypointsContainer;
    // This is the image of a pencil or brush that will follow the mouse cursor.
    [SerializeField] private GameObject m_Pencil;

    // This section is for sounds.
    [Header("Audio")]
    // This sound plays when just this one part is finished.
    [SerializeField] private AudioClip m_PartCompletedSound;
    // This sound plays if the player makes a mistake and the trace resets.
    [SerializeField] private AudioClip m_TraceResetSound;


    // This is an event that we trigger when this part is completed.
    // The LetterController listens to this to know when to activate the next part.
    [Header("Events")]
    public UnityEvent OnTraceCompleted;

    // A list to hold all the Waypoint components from the m_WaypointsContainer.
    private List<Waypoint> m_Waypoints;
    // This keeps track of the next waypoint the player needs to reach.
    private int m_CurrentWaypointIndex;
    // This is how close the mouse has to be to a waypoint to "collect" it. It's adjustable in the Inspector.
    [SerializeField] private float m_WaypointThreshold = 0.5f;
    // A simple flag to know if the player is currently dragging the mouse to trace.
    private bool m_IsTracing;
    // We need a reference to the main camera to convert mouse screen position to world position.
    private Camera m_MainCamera;
    public int score = 0;

    // A property to check if this part has been completed. Other scripts can read this.
    public bool IsCompleted { get; private set; }

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Find the main camera in the scene.
        m_MainCamera = Camera.main;
        if (m_FillSprite == null)
        {
            // If we forgot to assign the fill sprite in the Inspector, this will warn us.
            Debug.LogError("Fill Sprite is not assigned in the inspector!", this);
            return;
        }

        if (m_Pencil != null)
        {
            // Make sure the pencil is hidden at the start.
            m_Pencil.SetActive(false);
        }

        // Prepare the list of waypoints.
        m_Waypoints = new List<Waypoint>();
        if (m_WaypointsContainer != null)
        {
            // Go through each child object in the waypoints container.
            foreach (Transform child in m_WaypointsContainer)
            {
                // Get the Waypoint script from the child.
                Waypoint waypoint = child.GetComponent<Waypoint>();
                if (waypoint != null)
                    m_Waypoints.Add(waypoint);
                else
                    // If a child doesn't have the Waypoint script, it's a problem.
                    Debug.LogError($"Waypoint GameObject '{child.name}' is missing the Waypoint component.", child);
            }
        }

        // When the game starts, this part is inactive, so its fill should be invisible.
        m_FillSprite.color = Color.clear;
    }

    // This function is called by the LetterController when it's this part's turn to be traced.
    public void Activate()
    {
        IsCompleted = false;
        m_CurrentWaypointIndex = 0;
        // Show the semi-transparent guide color so the player knows what to trace.
        m_FillSprite.color = m_GuideColor;
        Debug.Log($"Part {gameObject.name} activated.");
    }

    // This is a Unity function that is called when the user presses the mouse button over this object's collider.
    private void OnMouseDown()
    {
        // We only start tracing if this part is enabled (by the LetterController) and not already finished.
        if (enabled && !IsCompleted)
        {
            m_IsTracing = true;
            if (m_Pencil != null)
            {
                // Show the pencil when tracing starts.
                m_Pencil.SetActive(true);
            }
        }
    }

    // This is called every frame while the user holds down the mouse button and drags.
    private void OnMouseDrag()
    {
        // If we're not in tracing mode, do nothing.
        if (!m_IsTracing) return;

        if (m_MainCamera == null)
        {
            Debug.LogError("Main Camera is not found. Make sure you have a camera in the scene tagged as 'MainCamera'.");
            return;
        }

        // Get the mouse position on the screen and convert it to a position in our 2D game world.
        Vector2 mousePosition = m_MainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Move the pencil to follow the mouse.
        if (m_Pencil != null)
        {
            m_Pencil.transform.position = new Vector3(mousePosition.x, mousePosition.y, m_Pencil.transform.position.z);
        }

        // Check if we have reached the next waypoint in our path.
        if (m_CurrentWaypointIndex < m_Waypoints.Count)
        {
            // If the mouse is close enough to the current waypoint...
            if (Vector2.Distance(mousePosition, m_Waypoints[m_CurrentWaypointIndex].transform.position) < m_WaypointThreshold)
            {
                // ...we move on to the next waypoint.
                m_CurrentWaypointIndex++;
                ActivateNextWaypointVisual();
            }
        }

        // We calculate how much of the path has been traced.
        float progress = 0;
        if (m_Waypoints.Count > 0)
        {
            progress = (float)m_CurrentWaypointIndex / m_Waypoints.Count;
        }
        // We update the fill color's transparency to show the progress.
        m_FillSprite.color = new Color(m_CompletedColor.r, m_CompletedColor.g, m_CompletedColor.b, progress);

        // If we've hit all the waypoints, the part is complete!
        if (m_CurrentWaypointIndex >= m_Waypoints.Count)
        {
            CompleteTrace();
        }
    }

    // This is called when the user releases the mouse button.
    private void OnMouseUp()
    {
        // If the player lets go of the mouse before finishing the part, we reset.
        if (m_IsTracing && !IsCompleted)
        {
            ResetTrace();
        }
        else
        {
            // If the trace was completed, we hide the pencil.
            if (m_Pencil != null)
                m_Pencil.SetActive(false);
        }
        // Stop tracing mode and hide the pencil.
        m_IsTracing = false;
        if (m_Pencil != null)
        {
            m_Pencil.SetActive(false);
        }
    }

    // This is called if the mouse cursor leaves the area of this part's collider.
    private void OnMouseExit()
    {
        // If the player's mouse goes outside the lines while tracing, we reset.
        if (m_IsTracing)
        {
            ResetTrace();
            m_IsTracing = false;
            if (m_Pencil != null)
            {
                m_Pencil.SetActive(false);
            }
        }
    }

    // This function resets the tracing progress for this part.
    private void ResetTrace()
    {
        Debug.Log($"Trace reset for {gameObject.name}");
        // If we have a reset sound and a camera, play the sound at the camera's position.
        if (m_TraceResetSound != null && m_MainCamera != null)
        {
            AudioSource.PlayClipAtPoint(m_TraceResetSound, m_MainCamera.transform.position);
        }
        // Go back to the first waypoint.
        m_CurrentWaypointIndex = 0;
        // Change the color back to the semi-transparent guide color.
        m_FillSprite.color = m_GuideColor;
        // Hide all the little green dots on the waypoints.
        foreach (var waypoint in m_Waypoints)
        {
            waypoint.Deactivate();
        }
    }

    // This function is called when the part has been successfully traced.
    private void CompleteTrace()
    {
        // If it's already completed, we don't need to do anything.
        if (IsCompleted) return;

        Debug.Log($"Trace completed for {gameObject.name}!");
        // If we have a part completion sound and a camera, play it.
        if (m_PartCompletedSound != null && m_MainCamera != null)
        {
            AudioSource.PlayClipAtPoint(m_PartCompletedSound, m_MainCamera.transform.position);
        }
        // Mark this part as completed.
        IsCompleted = true;
        m_IsTracing = false;
        // Set the fill to the solid 'completed' color.
        m_FillSprite.color = new Color(m_CompletedColor.r, m_CompletedColor.g, m_CompletedColor.b, 1f);
        // Hide the pencil.
        if (m_Pencil != null)
        {
            m_Pencil.SetActive(false);
        }
        // Trigger the 'OnTraceCompleted' event to let the LetterController know we're done.
        OnTraceCompleted.Invoke();
    }

    // This makes the little green dot appear on the waypoint we just passed.
    private void ActivateNextWaypointVisual()
    {
        // The waypoint we just passed is at the previous index.
        int passedWaypointIndex = m_CurrentWaypointIndex - 1;
        if (passedWaypointIndex >= 0 && passedWaypointIndex < m_Waypoints.Count)
            m_Waypoints[passedWaypointIndex].Activate();
    }
}