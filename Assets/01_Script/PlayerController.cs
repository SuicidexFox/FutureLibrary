using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
 {
    ///////////////////////////////////// Variable \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    /// Input
    public PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction pauseAction;
    private InputAction lookAction;
    public CharacterController characterController;
    
    /// Walk
    private float walkSpeed = 2f;
    private float runSpeed = 4f;
    private float moveSpeed;
    private float minTurnSpeed = 0.2f;
    private float turnSpeed = 5f;
    
    /// Jump
    private float gravityVelocity;
    private bool isGrounded;
    private float gravity = -9.81f;
    private float gravityMulitplier = 2f;
    private float jumpHeight = 2.4f;
    
    /// Look
    public float cameraRotation;
    public Transform camTransform;
    public float maxCamRotation;
    public float minCamRotation;
    public float cameraSpeed;
    
    /// Interact
    public Interactable currentInteractable;
    
    
    ///////////////////////////////////// Start \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void Start()
    {
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

        ///////////////////////////////////// Look \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        lookAction = playerInput.actions.FindAction("Look");
    }
    
    ///////////////////////////////////// Update \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public void Update() 
    {   
        ////////////////////////////////// Movement \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
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
        
        
        ///////////////////////////////////// Look \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        //float mousex = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        //float mousey = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;
        
        
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, lookInput.x);
        
        cameraRotation = Mathf.Clamp(cameraRotation - lookInput.y, minCamRotation, maxCamRotation);
        camTransform.localRotation = Quaternion.Euler(cameraRotation, 0,0 );
        
        
        ///////////////////////////////////// Gravity/Jump \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
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

            
            ///////////////////////////////////// Footsteps\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
            if (input != Vector2.zero)
            { if (footstepCoroutine == null) { footstepCoroutine = StartCoroutine(PlayFootsteps()); } }
            else { if (footstepCoroutine != null) { StopCoroutine(footstepCoroutine); footstepCoroutine = null; } }
        }
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
        currentInteractable = null;
    }
    public void ActivateInput()
    {
        playerInput.SwitchCurrentActionMap("Player");
        GameManager.instance.inUI = false;
    }
    
    
    
    ///////////////////////////////////// Footsteps \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public float footstepInterval = 0.5f; 
    private AudioSource audioSource;
    Coroutine footstepCoroutine;
    
    private IEnumerator PlayFootsteps() { while (true) { PlayFootstep(); yield return new WaitForSeconds(footstepInterval); } }
    void PlayFootstep() { RuntimeManager.PlayOneShot("event:/SFX/FootSteps/Stone"); }
}