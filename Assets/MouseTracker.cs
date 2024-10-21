using UnityEngine;

public class MouseTracker : MonoBehaviour
{
    public GameObject cursorDot; // Reference to the dot GameObject (e.g., a Sprite or Image)

    void Start()
    {
        // Hide the system cursor
        Cursor.visible = false;

        if (cursorDot == null)
        {
            Debug.LogError("Cursor dot is not assigned!");
        }
    }

    void Update()
    {
        // Update the dot position to follow the mouse
       
            Vector3 mousePosition = Input.mousePosition;

            // Convert mouse position to world space (for 2D game objects in world space)
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f; // Ensure the z-position is 0 for 2D objects

            // Set the dot's position to the mouse's world position
            cursorDot.transform.position = worldPosition;
        
    }
}