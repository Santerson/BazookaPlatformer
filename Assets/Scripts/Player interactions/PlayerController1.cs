using System.Linq.Expressions;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
public class PlayerPhysicsController : MonoBehaviour
{
    /// <summary>
    /// The force that the player starts moving at
    /// </summary>
    [Tooltip("The force that the player starts moving at")]
    [SerializeField] float MoveForce = 20.0f;
    /// <summary>
    /// The player's max force
    /// </summary>
    [Tooltip("The player's max movement speed")]
    [SerializeField] float MaxVelocity = 6.0f;
    /// <summary>
    /// The speed at which the player slows down after releasing movement buttons (should be <1, over 1 will accelerate the player)
    /// </summary>
    [Tooltip("The speed at which the player slows down after releasing movement buttons (should be <1, over 1 will accelerate the player)")]
    [SerializeField] float DampingCoefficient = 0.97f;
    /// <summary>
    /// The keybind to jump. Duh.
    /// </summary>
    [Tooltip("Hey Siri, I forgot how to jump. How do I jump?")]
    [SerializeField] KeyCode KeyBindJump = KeyCode.Space;
    Rigidbody2D RefRigidbody = null;
    CapsuleCollider2D RefCapsule = null;
    /// <summary>
    /// Amount of extra jumps (not including the first grounded one)
    /// </summary>
    [Tooltip("Amount of extra jumps (not including the first grounded one)")]
    [SerializeField] int ExtraJumpCount = 1;
    /// <summary>
    /// The distance that the player can be off the ground while still technically being considered grounded
    /// </summary>
    [Tooltip("The distance that the player can be off the ground while still technically being considered grounded")]
    [SerializeField] float GroundedGraceDistance = 0.1f;
    [Tooltip("Tonight, my friends, we will SOAR the skies. - Scarlet, Wings of Fire")]
    [SerializeField] float JumpHeight = 5f;
    [Tooltip("The tag surfaces must need to be able to reset the jumps of the player. IF THE JUMPS ARE NOT RESETTING, THIS IS PROBABLY WHY")]
    [SerializeField] string NoJumpResetTag;
    int JumpsLeft = 0;
    Vector2 RespawnLocation = new Vector2(-5.84f, -3f); // SpawnLocation


    void Awake()
    {
        // Cache a reference to the rigidbody component on the game start to avoid
        // the expensive GetComponent<> call each frame.
        RefRigidbody = GetComponent<Rigidbody2D>();
        if (RefRigidbody == null)
        {
            Debug.LogError("The player controller needs to have a rigidbody.");
        }
        RefCapsule = GetComponent<CapsuleCollider2D>();
        if (RefCapsule == null)
        {
            Debug.LogError("Player Controller could not find a CapsuleCollider");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
        UpdateMovement();
        UpdateJump();
    }

    /// <summary>
    /// Moves the player based on input and the direction of the camera
    /// </summary>
    private void UpdateMovement()
    {
        
        // Collect input in a vector to see which keys the player is holding down this frame.
        Vector2 inputVector = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) { inputVector.x -= 1; } //Left
        if (Input.GetKey(KeyCode.D)) { inputVector.x += 1; } //Right
        inputVector.Normalize();
        // Accelerate the player using our max allowed move force.
        RefRigidbody.AddForce(inputVector * MoveForce * (Time.deltaTime * 600));
        // Clamp the maximum velocity to our defined cap.
        if ((RefRigidbody.velocity.magnitude - Mathf.Abs(RefRigidbody.velocity.y)) > MaxVelocity)
        {
            // Maintain the current direction by using the normalized velocity vector
            float ySpeed = RefRigidbody.velocity.y;
            RefRigidbody.velocity = RefRigidbody.velocity.normalized * MaxVelocity;
            RefRigidbody.velocity = new Vector2(RefRigidbody.velocity.x, ySpeed);
        }
        // We know the player let go of the controls if the input vector is nearly zero.
        if (inputVector.sqrMagnitude <= 0.1f)
        {
            // Quickly damp the movement when we let go of the inputs by multiplying the vector
            // by a value less than one each frame.
            float verticalComponent = RefRigidbody.velocity.y;
            RefRigidbody.velocity *= DampingCoefficient * Time.deltaTime;
            RefRigidbody.velocity = new Vector2(RefRigidbody.velocity.x, verticalComponent);
        }
        
    }

    /// <summary>
    /// Checks if the jump hotkey is pressed  and jumps if possible
    /// </summary>
    private void UpdateJump()
    {

        //Check if the player is grounded

        //The origin of the raycast should be from the bottom of the player's capsule
        Vector2 origin = new Vector2(transform.position.x, transform.position.y - 1.001f);
        Vector2 direction = Vector2.down;

        //If on the ground (returns true if there is a collision)
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, GroundedGraceDistance);
       
        if(Physics2D.Raycast(origin, direction, GroundedGraceDistance))
        {
            //Checks if the player is legally considered grounded using the GroundedGraceDistnace AND is going downward
            if (IsGrounded(hit))JumpsLeft = ExtraJumpCount + 1;
        }

        //If the jump key is pressed AND player has jumps left
        if (Input.GetKeyDown(KeyBindJump) && JumpsLeft > 0)
        {
            //jump 
            //Set the velocity upwards
            RefRigidbody.velocity = new Vector2(RefRigidbody.velocity.x, JumpHeight);
            //Decrease the jumps left
            JumpsLeft--;
        }
        

    }

    void RespawnPlayer()
    {
        transform.position = RespawnLocation;
        RefRigidbody.velocity = Vector2.zero;
        FindObjectOfType<Bazooka>().XRecoil = 0;
        FindObjectOfType<Bazooka>().YRecoil = 0;
        Camera.main.transform.position = new Vector3(RespawnLocation.x, RespawnLocation.y, -10);
        FindObjectOfType<CameraLocking>().LockTheCamera(Camera.main.transform.position);
    }

    /// <summary>
    /// Returns true if the player is considered grounded
    /// </summary>
    /// <param name="hitInfo">The result of the raycast to the surface</param>
    bool IsGrounded(RaycastHit2D hitInfo)
    {
        bool bCloseToGround = hitInfo.distance < GroundedGraceDistance;
        bool bIsFalling = RefRigidbody.velocity.y <= 0;
        bool bSurfaceCantResetJumps = hitInfo.transform.CompareTag(NoJumpResetTag);
        return bCloseToGround && bIsFalling && !bSurfaceCantResetJumps;
    }
}