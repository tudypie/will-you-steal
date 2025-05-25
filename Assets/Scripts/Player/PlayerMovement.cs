using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 15f;

    private float maxVelocity = 1;

    private Vector2 move;

    private Rigidbody rb;
    private Animator animator;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("IsMoving", false);
    }

    private void Update()
    {
        HandleAnimator();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleAnimator()
    {
        if (rb.linearVelocity != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private void HandleMovement()
    {
        // Get camera-relative movement directions
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Flatten to horizontal plane
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Calculate movement direction based on input and target velocity
        Vector3 inputDirection = camForward * move.y + camRight * move.x;
        Vector3 targetVelocity = inputDirection * moveSpeed;
        targetVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Apply velocity change with clamping
        Vector3 velocityChange = Vector3.ClampMagnitude(targetVelocity - rb.linearVelocity, maxVelocity);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Rotate player toward movement direction
        if (inputDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }
}
