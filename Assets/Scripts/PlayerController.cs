using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 4f;        // MÁS LENTO
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 30f;
    [SerializeField] private float airControl = 0.6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBuffer = 0.12f;

    [Header("Gravity Feel")]
    [SerializeField] private float fallMultiplier = 2.2f;
    [SerializeField] private float lowJumpMultiplier = 1.6f;

    [Header("Ground Check")]
    [SerializeField] private float groundRadius = 0.18f;

    private Rigidbody rb;

    private Vector2 input;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Input AWSD
        input.x = Input.GetAxisRaw("Horizontal"); // A / D
        input.y = Input.GetAxisRaw("Vertical");   // W / S
        input = Vector2.ClampMagnitude(input, 1f);

        // Jump buffer
        if (Input.GetKeyDown(KeyCode.Space))
            lastJumpPressedTime = Time.time;
    }

    private void FixedUpdate()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );

        if (isGrounded)
            lastGroundedTime = Time.time;

        Move();
        Jump();
        BetterGravity();
    }

    private void Move()
    {
        // 🔥 MOVIMIENTO RELATIVO A DONDE MIRA EL PLAYER
        Vector3 desiredDir =
            transform.forward * input.y +
            transform.right   * input.x;

        desiredDir.y = 0f;
        desiredDir.Normalize();

        float control = isGrounded ? 1f : airControl;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityXZ = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 targetVelocity = desiredDir * maxSpeed;

        float accelRate = desiredDir.sqrMagnitude > 0.01f ? acceleration : deceleration;

        Vector3 newVelocityXZ = Vector3.MoveTowards(
            velocityXZ,
            targetVelocity,
            accelRate * control * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(newVelocityXZ.x, velocity.y, newVelocityXZ.z);

        // Rotar el Player hacia donde se mueve
        if (desiredDir.sqrMagnitude > 0.01f && input.y > 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(desiredDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                5f * Time.fixedDeltaTime
            );
        }
    }

    private void Jump()
    {
        bool canCoyoteJump = (Time.time - lastGroundedTime) <= coyoteTime;
        bool bufferedJump = (Time.time - lastJumpPressedTime) <= jumpBuffer;

        if (bufferedJump && canCoyoteJump)
        {
            lastGroundedTime = -999f;
            lastJumpPressedTime = -999f;

            Vector3 v = rb.linearVelocity;
            if (v.y < 0f) v.y = 0f;
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void BetterGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y *
                           (fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y *
                           (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}