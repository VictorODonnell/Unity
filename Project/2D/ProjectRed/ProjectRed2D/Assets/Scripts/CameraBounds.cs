using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public Camera mainCamera; // Public variable to assign camera manually

    private void Start()
    {
        // If mainCamera is not assigned in the Inspector, find the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Check if the camera is still null (e.g., no camera tagged as MainCamera)
        if (mainCamera == null)
        {
            Debug.LogError("No main camera found. Please assign a camera to the CameraBounds script.");
        }
    }

    public Vector3 ClampPositionToCameraBounds(Vector3 targetPos, Vector3 playerSize)
    {
        if (mainCamera == null)
        {
            return targetPos; // Return original position if no camera is found
        }

        float halfPlayerWidth = playerSize.x / 2f;
        float halfPlayerHeight = playerSize.y / 2f;

        Vector3 minScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 maxScreenBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.nearClipPlane));

        float minX = minScreenBounds.x + halfPlayerWidth;
        float maxX = maxScreenBounds.x - halfPlayerWidth;
        float minY = minScreenBounds.y + halfPlayerHeight;
        float maxY = maxScreenBounds.y - halfPlayerHeight;

        Vector3 clampedPos = targetPos;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);

        return clampedPos;
    }
}
