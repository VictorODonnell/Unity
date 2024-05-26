using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal movement speed
    public float slowSpeed = 2f; // Slower movement speed
    public float slowDuration = 2f; // Duration for slow-down effect
    public float cooldownDuration = 5f; // Cooldown period before slow-down can be triggered again
    private bool isMoving;
    private bool isAnimating; // Flag to track if the player is currently animating
    public Vector2 input;
    private bool canSlowDown = true; // To check if the slow-down can be triggered
    private CameraBounds cameraBounds;
    private Vector3 playerSize;

    private void Start()
    {
        cameraBounds = GetComponent<CameraBounds>();

        if (cameraBounds == null)
        {
            Debug.LogError("CameraBounds component not found on the player GameObject.");
        }

        // Calculate player size
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            playerSize = renderer.bounds.size;
        }
        else
        {
            // Default player size if no renderer is found
            playerSize = new Vector3(1, 1, 0);
        }
    }

    private void Update()
    {
        if (!isMoving && isAnimating) // Check if not moving but animating
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                // Stop the animation if any WASD key is pressed
                SetIsAnimating(false);
            }
        }

        if (!isMoving && !isAnimating) // Check if neither moving nor animating
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Adjusted movement within the bounds
            var targetPos = transform.position + new Vector3(input.x * 0.5f, input.y * 0.5f, 0);
            targetPos.x = Mathf.Clamp(targetPos.x, -9f, 9f);
            targetPos.y = Mathf.Clamp(targetPos.y, -3.9f, 3.9f);

            if (input != Vector2.zero)
            {
                float currentSpeed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && canSlowDown ? slowSpeed : moveSpeed;
                StartCoroutine(Move(targetPos, currentSpeed));
            }
        }

        // Check for Shift key press to trigger slow down
        if (canSlowDown && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            StartCoroutine(HandleSlowDown());
        }
    }

    IEnumerator Move(Vector3 targetPos, float speed)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            Vector3 step = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            transform.position = cameraBounds.ClampPositionToCameraBounds(step, playerSize);
            yield return null;
        }

        transform.position = cameraBounds.ClampPositionToCameraBounds(targetPos, playerSize);
        isMoving = false;
    }

    IEnumerator HandleSlowDown()
    {
        canSlowDown = false;
        float originalSpeed = moveSpeed;

        // Apply slow-down effect
        moveSpeed = slowSpeed;
        yield return new WaitForSeconds(slowDuration);
        moveSpeed = originalSpeed;

        // Start cooldown
        yield return new WaitForSeconds(cooldownDuration);
        canSlowDown = true;
    }

    // Method to set isAnimating flag
    public void SetIsAnimating(bool value)
    {
        isAnimating = value;
    }
}
