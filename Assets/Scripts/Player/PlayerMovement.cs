using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;

    private float maxVelocity = 1;

    private Vector2 move;

    private Rigidbody rb;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Movement();
        // HandleStepsSound(); - play sound
    }

    private void Movement()
    {
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = camForward * move.y + camRight * move.x;

        Vector3 targetVelocity = inputDirection * moveSpeed;

        targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - currentVelocity;

        velocityChange = Vector3.ClampMagnitude(velocityChange, maxVelocity);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        if (inputDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
