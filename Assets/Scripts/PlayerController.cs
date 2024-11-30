using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    [Header("Block Interaction")]
    public GameObject blockPrefab; // Prefab of the block to place
    public float interactionRange = 5f; // Range for placing/removing blocks

    [Header("Sound Effects")]
    public AudioClip walkRunSound; // Combined walk and run sound
    public AudioClip placeBlockSound;
    public AudioClip destroyBlockSound;
    public AudioClip backgroundMusic; // For looping background music

    private AudioSource walkRunAudioSource; // AudioSource for walk/run sounds
    private AudioSource interactionAudioSource; // AudioSource for interaction sounds (place, destroy)
    private AudioSource musicSource; // AudioSource for background music
    private CharacterController characterController;
    private Vector3 velocity;
    private float currentSpeed;
    private float xRotation = 0f;
    private bool isSprinting = false;
    private bool isGrounded = false;
    private bool isWalking = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();

        // Initialize the AudioSources
        walkRunAudioSource = gameObject.AddComponent<AudioSource>();
        interactionAudioSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        // Configure the audio sources
        walkRunAudioSource.loop = true;
        walkRunAudioSource.playOnAwake = false; // Don't play immediately
        interactionAudioSource.playOnAwake = false; // Don't play immediately
        musicSource.clip = backgroundMusic; // Assign background music
        musicSource.loop = true; // Set music to loop
        musicSource.Play(); // Start playing the background music

        currentSpeed = walkSpeed; // Start at normal walking speed
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        HandleMouseLook();
        HandleMovement();
        HandleJumping();
        HandleSprinting();
        HandleBlockInteraction(); // Block interaction logic
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Play walking/running sound when moving, but only if grounded
        if (isGrounded)
        {
            if (horizontal != 0 || vertical != 0)
            {
                if (!isWalking)
                {
                    isWalking = true;
                    walkRunAudioSource.clip = walkRunSound; // Set the walk/run sound
                    walkRunAudioSource.loop = true; // Loop the sound
                    walkRunAudioSource.Play(); // Start playing the sound
                }

                // Adjust pitch when sprinting
                if (isSprinting)
                {
                    walkRunAudioSource.pitch = 1.2f; // Faster pitch for running
                }
                else
                {
                    walkRunAudioSource.pitch = 1f; // Normal pitch for walking
                }
            }
            else
            {
                if (isWalking)
                {
                    isWalking = false;
                    walkRunAudioSource.Stop(); // Stop the walking sound when idle
                }
            }
        }
        else
        {
            // Ensure sound stops when in the air
            if (isWalking)
            {
                isWalking = false;
                walkRunAudioSource.Stop(); // Stop sound when in the air
            }
        }

        // Apply gravity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    void HandleJumping()
    {
        // Check if the player is on the ground
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            // Check if there's enough space above the player to jump
            RaycastHit hit;

            // Raycast upwards to check if there is a block above the player
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1f))
            {
                // Check if the player is close to a block ahead while walking/running
                Vector3 forwardRayStart = transform.position + transform.forward * 0.5f;
                RaycastHit forwardHit;

                // Check for an obstacle ahead of the player
                if (!Physics.Raycast(forwardRayStart, Vector3.up, out forwardHit, 1f))
                {
                    // Apply jump force if no block above and no block ahead
                    velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                }
            }
        }
    }



    void HandleSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isSprinting)
            {
                isSprinting = true;
                currentSpeed = sprintSpeed;
            }
        }
        else
        {
            if (isSprinting)
            {
                isSprinting = false;
                currentSpeed = walkSpeed;
            }
        }
    }

    void HandleBlockInteraction()
    {
        RaycastHit hit; // Declare the RaycastHit variable outside the Raycast call

        // Raycast to detect blocks
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange))
        {
            // For block deletion (left mouse click)
            if (Input.GetMouseButtonDown(0)) // Left click to delete block
            {
                if (hit.collider != null)
                {
                    Destroy(hit.collider.gameObject); // Destroy the block at the hit point
                    interactionAudioSource.PlayOneShot(destroyBlockSound); // Play destroy sound
                }
            }

            // For block placement (right mouse click)
            if (Input.GetMouseButtonDown(1)) // Right click to place block
            {
                if (hit.collider != null)
                {
                    Vector3 placePosition = hit.point + hit.normal / 2f; // Position slightly offset to place on the surface
                    placePosition = new Vector3(
                        Mathf.Round(placePosition.x),
                        Mathf.Round(placePosition.y),
                        Mathf.Round(placePosition.z)
                    );

                    Instantiate(blockPrefab, placePosition, Quaternion.identity); // Place block at the calculated position
                    interactionAudioSource.PlayOneShot(placeBlockSound); // Play place block sound
                }
            }
        }
    }
}
