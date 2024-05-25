using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Normal movement speed
    public float slowSpeed = 2f; // Slower movement speed
    private bool isMoving;
    public Vector2 input;
    private bool isCooldown = false; // To check if the cooldown is active
    public float slowDuration = 2f; // Duration for the slowdown effect
    public float cooldownDuration = 5f; // Duration of the cooldown

    private void Update() 
    {
        if (!isMoving) 
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? slowSpeed : moveSpeed));
            }

            // Check for Shift key press to trigger slow down, if not on cooldown
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && !isCooldown)
            {
                StartCoroutine(HandleSlowDown());
            }
        }   
    }

    IEnumerator Move(Vector3 targetPos, float speed)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    IEnumerator HandleSlowDown()
    {
        isCooldown = true;
        float originalSpeed = moveSpeed;

        // Apply the slow down
        moveSpeed = slowSpeed;
        yield return new WaitForSeconds(slowDuration);

        // Revert to normal speed after the slow duration
        moveSpeed = originalSpeed;
        yield return new WaitForSeconds(cooldownDuration);

        isCooldown = false;
    }
}
