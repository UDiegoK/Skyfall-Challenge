using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float gravity = -9.81f;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundRadius = 0.4f;
    public LayerMask groundLayer;
    
    private CharacterController controller;
    private Vector3 verticalVelocity;
    private bool isGrounded;
    
    // Public properties for other scripts
    public Vector3 Velocity { get; private set; }
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsRunning { get; private set; }
    public bool IsJumping { get; private set; }
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.parent = transform;
            checkObj.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = checkObj.transform;
        }
    }
    
    void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer);
        
        if (isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
            IsJumping = false;
        }
        
        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Calculate movement direction
        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        
        // Determine if running
        IsRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = IsRunning ? runSpeed : walkSpeed;
        
        // Move character
        controller.Move(direction * currentSpeed * Time.deltaTime);
        
        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            verticalVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            IsJumping = true;
        }
        
        // Apply gravity
        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
        
        // Store velocity for other scripts
        Velocity = new Vector3(horizontal, 0, vertical);
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }
    }
}