using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    ///////////////////////////////////// Variable \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public CinemachineInputProvider playerCamInputProvider;
    public CinemachineBrain cinemachineBrain;
    
    public PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction pauseAction;
    
    public CharacterController characterController;
    public Transform camTransform;
    
    public Interactable currentInteractable;
    
    //Move
    private float walkSpeed = 2f;
    private float runSpeed = 4f;
    private float moveSpeed;
    private float minTurnSpeed = 0.2f;
    private float turnSpeed = 5f;

    private float gravityVelocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float gravityMulitplier = 2f;
    private float jumpHeight = 2.4f;
    
    
    
    ///////////////////////////////////// Start \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void Start()
    {
        ///////////////////////////////////// Movement \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        camTransform = Camera.main.transform;
        
        ///////////////////////////////////// Walk & Run \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        moveAction = playerInput.actions.FindAction("Move");
        runAction = playerInput.actions.FindAction("Run");
        
        ///////////////////////////////////// Jump \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        jumpAction = playerInput.actions.FindAction("Jump");
        
        ///////////////////////////////////// Interact \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        interactAction = playerInput.actions.FindAction("Interact");
        interactAction.performed += Interact;

        ///////////////////////////////////// Pause \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        playerInput.actions.FindActionMap("Player").FindAction("Pause").performed +=Pause;
        playerInput.actions.FindActionMap("UI").FindAction("Pause").performed += Pause;
    }
    
    public void Update() 
    {   ////////////////////////////////// Movement \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        Vector2 input = moveAction.ReadValue<Vector2>();
        float horizontalInput = input.x;
        float verticalInput = input.y;

        ///////////////////////////////////// Sprint \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        if (runAction.ReadValue<float>() == 1f)
        { moveSpeed = runSpeed; }
        else
        { moveSpeed = walkSpeed; }
        
        ///////////////////////////////////// Move with Mouse \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        Vector3 horizontalVelocity = Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized * horizontalInput; //rechts,links
        Vector3 verticalVelocity = Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized * verticalInput; //hoch,runter
        Vector3 velocity = Vector3.ClampMagnitude(horizontalVelocity + verticalVelocity, 1); //es wird -2/5, 0-9 = 0-1 berechnet
        
        ///////////////////////////////////// Rotation at Mouse \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        if (velocity.magnitude > minTurnSpeed)
        {
            Quaternion rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(velocity), Time.deltaTime * turnSpeed);
            transform.rotation = rotation; // DeltaTime passt Frames von unterschiedlichen PC`s an 
        }
        
        ///////////////////////////////////// Gravity \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        if (characterController.enabled == true)
        {
            isGrounded = characterController.isGrounded;
            float finalGravity = gravity * gravityMulitplier;
            
            if (isGrounded && gravityVelocity < 0)
            { gravityVelocity = -2f;} // Hält den Player am Boden}

            if (jumpAction.triggered && isGrounded)
            {gravityVelocity = Mathf.Sqrt(jumpHeight * -2f * finalGravity);} //Sprungstärke

            gravityVelocity += finalGravity * Time.deltaTime; //Sprunggeschwindigkeit
            velocity = new Vector3(velocity.x * moveSpeed, gravityVelocity, velocity.z * moveSpeed); //Bewegungsvektor x,y,z

            characterController.Move(velocity * Time.deltaTime); //Player bewegen
        }
        /*if (characterController.enabled == true)
        { characterController.SimpleMove(new Vector3(velocity.x * moveSpeed, 0, velocity.z * moveSpeed)); }*/
        
        
        ///////////////////////////////////// Animations \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        float animatonSpeed = velocity.magnitude;
        if (input == Vector2.zero)
        { animatonSpeed = 0.0f; }
        else if (runAction.inProgress) { }
        
        /*///////////////////////////////////// QuestLog \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //Keyboard
        if (Input.GetKeyDown(KeyCode.Tab))
        { questLog = true; }
        // Prüfe, ob die Taste losgelassen wurde
        if (Input.GetKeyUp(KeyCode.Tab))
        { questLog = false; GameManager.instance.questLog.ResetCanvasPosition(); }
        // Bewege das Canvas, wenn die Taste gedrückt wird
        if (questLog)
        { GameManager.instance.questLog.MoveCanvas();}
        //Controller
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            if (gamepad.dpad.right.wasPressedThisFrame) { questLog = true; }
            if (gamepad.dpad.right.wasReleasedThisFrame)
            { questLog = false; GameManager.instance.questLog.ResetCanvasPosition(); }
            if (questLog) { GameManager.instance.questLog.MoveCanvas(); }
        }*/
    } 
    
    private void OnDisable() //Disable behavior 
    {
        interactAction.performed -=Interact;
        playerInput.actions.FindActionMap("Player").FindAction("Pause").performed -=Pause;
        playerInput.actions.FindActionMap("UI").FindAction("Pause").performed -= Pause;
    }
    
    
    ///////////////////////////////////// Interact & Collect \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    private void Interact(InputAction.CallbackContext obj)
    {
        if (currentInteractable == null) { return; }
        currentInteractable._onInteract.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        Interactable _newInteractable = other.GetComponent<Interactable>();
        if (_newInteractable == null) { return; }
        currentInteractable = _newInteractable;
    }
    private void OnTriggerExit(Collider other)
    { 
        Interactable _newInteractable = other.GetComponent<Interactable>();
        if (currentInteractable == null) { return; } 
        if (_newInteractable == currentInteractable)
        {
            currentInteractable = null;
        }
    }
    
    ///////////////////////////////////// PauseUI \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    private void Pause(InputAction.CallbackContext obj) { GameManager.instance.TogglePause(); }
    
    ///////////////////////////////////// Player Inputs \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void DeactivateInput()  //Deaktiviere Alles
    {
        playerInput.SwitchCurrentActionMap("UI");
        playerCamInputProvider.enabled = false;
        currentInteractable = null;
    }
    public void ActivateInput()
    {
        playerInput.SwitchCurrentActionMap("Player");
        playerCamInputProvider.enabled = true;
        GameManager.instance.inUI = false;
    }
    
    ///////////////////////////////////// Extras \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void BrainTime(int newBrainTime) { cinemachineBrain.m_DefaultBlend.m_Time = newBrainTime; }
    
}
