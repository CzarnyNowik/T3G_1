using UnityEngine;

#region NEW_INPUT
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#endregion

/// <summary>
/// Simple player movement.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Character")]
    // Reference to the character controller
    [SerializeField]
    private CharacterController character;

    // Character movement speed.
    [SerializeField]
    private float moveSpeed = 4;

    [Header("Camera")]
    // Reference to the character camera.
    [SerializeField]
    private Camera characterCamera;

    // Camera movement speed.
    [SerializeField]
    private float camSpeed = 40;

#region NEW_INPUT
    private InputMaster controls;
    public Vector2 moveInput;
    public Vector2 mouseInput;
    private void Awake()
    {
        controls = new InputMaster();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
#endregion

    /// <summary>
    /// Method called at the start.
    /// </summary>
    private void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    /// <summary>
    /// Method called every frame.
    /// </summary>
    private void Update()
    {
        // Get player input for character movement

#region OLD_INPUT
        //var v = Input.GetAxis("Vertical");
        //var h = Input.GetAxis("Horizontal");
#endregion

#region NEW_INPUT
        moveInput = controls.Player.Movement.ReadValue<Vector2>();
        var v = moveInput.y;
        var h = moveInput.x;
#endregion

        // Create move vector and rotate it
        var move = new Vector3(h, 0, v);
        move = character.transform.rotation * move;

        // If length of move vector is bigger than 1, normalize it.
        if (move.magnitude > 1)
            move = move.normalized;

        // Move character
        character.SimpleMove(move * moveSpeed);

        // Get player mouse input for camera and character rotation

#region OLD_INPUT
        //var mx = Input.GetAxisRaw("Mouse X");
        //var my = Input.GetAxisRaw("Mouse Y");
#endregion

#region NEW_INPUT
        mouseInput = controls.Player.Mouse.ReadValue<Vector2>();
        var mx = mouseInput.x;
        var my = mouseInput.y;
#endregion

        // Rotate character with mouse X value
        character.transform.Rotate(Vector3.up, mx * camSpeed);

        // Get camera rotation on X axis
        var currentRotationX = characterCamera.transform.localEulerAngles.x;
        // To get non-imersive cam you need to multiply my by negative
        currentRotationX += -my * camSpeed;

        // Limiting camera movement to (-60) - (60) degrees on X axis.
        if (currentRotationX < 180)
        {
            currentRotationX = Mathf.Min(currentRotationX, 60);
        }
        else if (currentRotationX > 180)
        {
            currentRotationX = Mathf.Max(currentRotationX, 300);
        }

        // Assign new camera rotation
        characterCamera.transform.localEulerAngles = new Vector3(currentRotationX, 0, 0);

    }
}