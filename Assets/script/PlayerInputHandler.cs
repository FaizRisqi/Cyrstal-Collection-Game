using UnityEngine;
using UnityEngine.InputSystem; // <-- TAMBAHKAN BARIS INI!

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Movement";
    [SerializeField] private string rotation = "Rotation";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string rotateObject = "RotateObject";

    private InputActionAsset playerControlsCached;
    private InputActionMap actionMapReference;
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction rotateObjectAction;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool RotateObjectTriggered { get; private set; }

    private void Awake()
    {
        // Cache the playerControls reference to avoid repeated lookups
        playerControlsCached = playerControls;

        // Find the action map by name
        actionMapReference = playerControlsCached.FindActionMap(actionMapName);
        if (actionMapReference == null)
        {
            Debug.LogError($"Action map '{actionMapName}' not found in playerControls.");
            return;
        }

        // Find individual actions
        movementAction = actionMapReference.FindAction(movement);
        rotationAction = actionMapReference.FindAction(rotation);
        jumpAction = actionMapReference.FindAction(jump);
        sprintAction = actionMapReference.FindAction(sprint);
        rotateObjectAction = actionMapReference.FindAction(rotateObject);

        // Log errors for missing actions
        if (movementAction == null) Debug.LogError($"Action '{movement}' not found in action map '{actionMapName}'.");
        if (rotationAction == null) Debug.LogError($"Action '{rotation}' not found in action map '{actionMapName}'.");
        if (jumpAction == null) Debug.LogError($"Action '{jump}' not found in action map '{actionMapName}'.");
        if (sprintAction == null) Debug.LogError($"Action '{sprint}' not found in action map '{actionMapName}'.");
        if (rotateObjectAction == null) Debug.LogError($"Action '{rotateObject}' not found in action map '{actionMapName}'.");
    }

    private void OnEnable()
    {
        if (actionMapReference != null)
        {
            actionMapReference.Enable();
            SubscribeToTriggerEvents();
        }
    }

    private void OnDisable()
    {
        if (actionMapReference != null)
        {
            UnsubscribeFromTriggerEvents();
            actionMapReference.Disable();
        }
    }

    private void OnDestroy()
    {
        // Ensure we unsubscribe if the object is destroyed while enabled
        UnsubscribeFromTriggerEvents();
    }

    private void Update()
    {
        // Read continuous input values every frame
        if (movementAction != null)
            MovementInput = movementAction.ReadValue<Vector2>();
        if (rotationAction != null)
            RotationInput = rotationAction.ReadValue<Vector2>();
    }

    private void SubscribeToTriggerEvents()
    {
        if (jumpAction != null)
        {
            jumpAction.performed += OnJumpPerformed;
            jumpAction.canceled += OnJumpCanceled;
        }
        if (sprintAction != null)
        {
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;
        }
        if (rotateObjectAction != null)
        {
            rotateObjectAction.performed += OnRotateObjectPerformed;
            rotateObjectAction.canceled += OnRotateObjectCanceled;
        }
    }

    private void UnsubscribeFromTriggerEvents()
    {
        if (jumpAction != null)
        {
            jumpAction.performed -= OnJumpPerformed;
            jumpAction.canceled -= OnJumpCanceled;
        }
        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
        }
        if (rotateObjectAction != null)
        {
            rotateObjectAction.performed -= OnRotateObjectPerformed;
            rotateObjectAction.canceled -= OnRotateObjectCanceled;
        }
    }

    // Trigger event handlers
    private void OnJumpPerformed(InputAction.CallbackContext ctx) => JumpTriggered = true;
    private void OnJumpCanceled(InputAction.CallbackContext ctx) => JumpTriggered = false;
    private void OnSprintPerformed(InputAction.CallbackContext ctx) => SprintTriggered = true;
    private void OnSprintCanceled(InputAction.CallbackContext ctx) => SprintTriggered = false;
    private void OnRotateObjectPerformed(InputAction.CallbackContext ctx) => RotateObjectTriggered = true;
    private void OnRotateObjectCanceled(InputAction.CallbackContext ctx) => RotateObjectTriggered = false;

    /// <summary>
    /// Atomically consumes the jump trigger flag. Returns true if the trigger was set and clears it.
    /// </summary>
    public bool ConsumeJumpTrigger()
    {
        bool value = JumpTriggered;
        JumpTriggered = false;
        return value;
    }

    /// <summary>
    /// Atomically consumes the sprint trigger flag. Returns true if the trigger was set and clears it.
    /// </summary>
    public bool ConsumeSprintTrigger()
    {
        bool value = SprintTriggered;
        SprintTriggered = false;
        return value;
    }

    /// <summary>
    /// Atomically consumes the rotate object trigger flag. Returns true if the trigger was set and clears it.
    /// </summary>
    public bool ConsumeRotateObjectTriggered()
    {
        bool value = RotateObjectTriggered;
        RotateObjectTriggered = false;
        return value;
    }
}