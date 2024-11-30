using UnityEngine;
using UnityEngine.UI; // Required for UI elements

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
    public GameObject[] blockPrefabs; // Array of block prefabs
    public Sprite[] blockImages; // Array of block sprites for UI
    public float interactionRange = 5f; // Range for placing/removing blocks
    private int selectedBlockIndex = 0; // Tracks the currently selected block

    [Header("UI Elements")]
    public Image blockSelectionImage; // Reference to the UI Image for block selection

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

    public PauseMenu pauseMenu;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        // Set initial block selection image
        UpdateBlockSelectionUI();
    }

    void Update()
    {
        if (!pauseMenu.isPaused) // Only lock the cursor if the game is not paused
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor when paused
            Cursor.visible = true; // Show the cursor when paused
        }
        isGrounded = characterController.isGrounded;

        HandleMouseLook();
        HandleMovement();
        HandleJumping();
        HandleSprinting();
        HandleBlockSelection(); // Handle block selection
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
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, Vector3.up, out hit, 1f))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
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

    void HandleBlockSelection()
    {
        for (int i = 0; i < blockPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) // Check keys 1-7
            {
                selectedBlockIndex = i;
                Debug.Log(string.Format("Selected Block: {0}", selectedBlockIndex));
                UpdateBlockSelectionUI(); // Update the UI when a block is selected
                break;
            }
        }
    }

    void HandleBlockInteraction()
    {
        RaycastHit hit;

        // Raycast to detect blocks
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, interactionRange))
        {
            // For block deletion (left mouse click)
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider != null)
                {
                    Destroy(hit.collider.gameObject); // Destroy the block at the hit point
                    interactionAudioSource.PlayOneShot(destroyBlockSound); // Play destroy sound
                }
            }

            // For block placement (right mouse click)
            if (Input.GetMouseButtonDown(1))
            {
                if (hit.collider != null)
                {
                    Vector3 placePosition = hit.point + hit.normal / 2f;
                    placePosition = new Vector3(
                        Mathf.Round(placePosition.x),
                        Mathf.Round(placePosition.y),
                        Mathf.Round(placePosition.z)
                    );

                    Instantiate(blockPrefabs[selectedBlockIndex], placePosition, Quaternion.identity); // Place the selected block
                    interactionAudioSource.PlayOneShot(placeBlockSound); // Play place block sound
                }
            }
        }
    }

    void UpdateBlockSelectionUI()
    {
        if (blockSelectionImage != null && blockImages.Length > selectedBlockIndex)
        {
            blockSelectionImage.sprite = blockImages[selectedBlockIndex]; // Update the image sprite
        }
    }
}
