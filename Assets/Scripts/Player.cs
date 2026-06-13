using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Speed")]
    [Range(0f, 2f)]
    public float Speed;
    [Range(0f, 1f)]
    public float Inertia;

    [Header("Jump")]
    [Range(0f, 12f)]
    public float JumpForce;
    [Range(0f, 1f)]
    public float JumpTorque;
    [Range(0f, 0.5f)]
    public float CoyotteTime;
    public Transform GroundPoint;
    public LayerMask GroundLayer;

    private Rigidbody2D rb;
    private float lastGroundedTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        rb.velocity = new Vector2(rb.velocity.x * Inertia + Speed * input.x, rb.velocity.y);
    }

    private void Update()
    {
        if (IsGrounded()) lastGroundedTime = Time.time;

        if (Input.GetKeyDown(KeyCode.Space) && lastGroundedTime > Time.time - CoyotteTime)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        }
        else if (rb.velocity.y > 0 && Input.GetKeyUp(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * JumpTorque);
        }
    }

    private bool IsGrounded() => Physics2D.OverlapCircleAll(GroundPoint.position, 0.1f, GroundLayer).Length > 1;
}
