using System.Collections;
using UnityEngine;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
public class MoveBehaviour : GenericBehaviour
{
	public float walkSpeed = 0.15f;                 // Default walk speed.
	public float runSpeed = 1.0f;                   // Default run speed.
	public float sprintSpeed = 2.0f;                // Default sprint speed.
	public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
	public string jumpButton = "Jump";              // Default jump button.
    public string attackButton = "Attack";          // Default attack key.
    public float jumpHeight = 1.5f;                 // Default jump height.
	public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.



    public System.Action OnAttack;

	private float speed, speedSeeker;               // Moving speed.
	private int jumpBool;                           // Animator variable related to jumping.
	private int groundedBool;                       // Animator variable related to whether or not the player is on ground.
    private int crouchBool;                         // Animator variable related to crouching.
    private int attackBool;                      // Animator variable related to attacking.
    private bool jump;                              // Boolean to determine whether or not the player started a jump.
    private bool crouch;                            // Boolean to determine whether or not the player started crouching.
    private bool attack;                            // Boolean to determine whether or not the player is attacking.
    private bool isColliding;                       // Boolean to determine if the player has collided with an obstacle.


    static int jumpIdleState = Animator.StringToHash("IdleJump");
    static int attackState = Animator.StringToHash("Attack");
    static int floatAttackType = Animator.StringToHash("AttackType");

    // Start is always called after any Awake functions.
    void Start() 
	{
		// Set up the references.
		jumpBool = Animator.StringToHash("Jump");
		groundedBool = Animator.StringToHash("Grounded");
        crouchBool = Animator.StringToHash("Crouch");
        attackBool = Animator.StringToHash("Attack");

        behaviourManager.GetAnim.SetBool (groundedBool, true);

		// Subscribe and register this behaviour as the default behaviour.
		behaviourManager.SubscribeBehaviour (this);
		behaviourManager.RegisterDefaultBehaviour (this.behaviourCode);
		speedSeeker = runSpeed;

        player = GetComponent<Player>();
	}

	// Update is used to set features regardless the active behaviour.
	void Update ()
	{
		// Get jump input.
		if (!jump && Input.GetButtonDown(jumpButton) && behaviourManager.IsCurrentBehaviour(this.behaviourCode) && !behaviourManager.IsOverriding())
		{
			jump = true;
		}
        ManageAim();
    }

	// LocalFixedUpdate overrides the virtual function of the base class.
	public override void LocalFixedUpdate()
	{
        // Call the basic movement manager.
        MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

		// Call the jump manager.
		JumpManagement();

        AttackManagment();

        // Set camera position and orientation to the aim mode parameters.
        if (aim)
            behaviourManager.GetCamScript.SetTargetOffsets(aimPivotOffset, aimCamOffset);
    }

	// Execute the idle and walk/run jump movements.
	void JumpManagement()
	{
		// Start a new jump.
		if (jump && !behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.IsGrounded())
		{
			// Set jump related parameters.
			behaviourManager.LockTempBehaviour(this.behaviourCode);
			behaviourManager.GetAnim.SetBool(jumpBool, true);
			// Is a locomotion jump?
			if(behaviourManager.GetAnim.GetFloat(speedFloat) > 0.1)
			{
				// Temporarily change player friction to pass through obstacles.
				GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
				GetComponent<CapsuleCollider>().material.staticFriction = 0f;
				// Set jump vertical impulse velocity.
				float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
				velocity = Mathf.Sqrt(velocity);
				behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
			}
		}
		// Is already jumping?
		else if (behaviourManager.GetAnim.GetBool(jumpBool))
		{
			// Keep forward movement while in the air.
			if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
			{
				behaviourManager.GetRigidBody.AddForce(transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
			}
			// Has landed?
			if ((behaviourManager.GetRigidBody.velocity.y < 0) && behaviourManager.IsGrounded())
			{
				behaviourManager.GetAnim.SetBool(groundedBool, true);
				// Change back player friction to default.
				GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
				GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
				// Set jump related parameters.
				jump = false;
				behaviourManager.GetAnim.SetBool(jumpBool, false);
				behaviourManager.UnlockTempBehaviour(this.behaviourCode);
			}
		}
	}

	// Deal with the basic player movement
	void MovementManagement(float horizontal, float vertical)
	{
		// On ground, obey gravity.
		if (behaviourManager.IsGrounded())
			behaviourManager.GetRigidBody.useGravity = true;

        // Call function that deals with player orientation.
        if (!aim) Rotating(horizontal, vertical);

		// Set proper speed.
		Vector2 dir = new Vector2(horizontal, vertical);
		speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
		// This is for PC only, gamepads control speed via analog stick.
		speedSeeker += Input.GetAxis("Mouse ScrollWheel");
		speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
		speed *= speedSeeker;
		if (behaviourManager.IsSprinting())
		{
			speed = sprintSpeed;
		}

		behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
	}

	// Rotate the player to match correct orientation, according to camera and key pressed.
	Vector3 Rotating(float horizontal, float vertical)
	{
		// Get camera forward direction, without vertical component.
		Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

		// Player is moving on ground, Y component of camera facing is not relevant.
		forward.y = 0.0f;
		forward = forward.normalized;

		// Calculate target direction based on camera forward and direction key.
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		Vector3 targetDirection;
		targetDirection = forward * vertical + right * horizontal;

		// Lerp current direction to calculated target direction.
		if((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
		{
			Quaternion targetRotation = Quaternion.LookRotation (targetDirection);

			Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
			behaviourManager.GetRigidBody.MoveRotation (newRotation);
			behaviourManager.SetLastDirection(targetDirection);
		}
		// If idle, Ignore current camera facing and consider last moving direction.
		if(!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
		{
			behaviourManager.Repositioning();
		}

		return targetDirection;
	}


    void AttackManagment()
    {
        var wasAttacking = attack;
        var attackNow = Input.GetButtonDown(attackButton);

        if(attackNow)
        attackNow = attackNow && !behaviourManager.IsCrouching() && behaviourManager.IsGrounded() &&
          behaviourManager.GetAnim.GetCurrentAnimatorStateInfo(0).shortNameHash != jumpIdleState;

        attack = attackNow || attack;

        if (behaviourManager.GetAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == attackState) attack = false;

        behaviourManager.GetAnim.SetBool(attackBool, attack);

        if (!wasAttacking && attack)
        {
            GetAnimator.SetFloat(floatAttackType, player.GetAttackType);

        }

    }
    public void Attack()
    {
        attack = true;
        behaviourManager.GetAnim.SetBool(attackBool, attack);
        GetAnimator.SetFloat(floatAttackType, player.GetAttackType);
    }

    public bool GetAttacks
    {
        get { return (behaviourManager.GetAnim.GetCurrentAnimatorStateInfo(0).shortNameHash == attackState); }
    }
    // Collision detection.
    private void OnCollisionStay(Collision collision)
	{
		isColliding = true;
	}
	private void OnCollisionExit(Collision collision)
	{
		isColliding = false;
	}

    public void SetAnimationController(RuntimeAnimatorController controller)
    {
        if (jump) return;

        Animator a = behaviourManager.GetAnim;
        if (a.runtimeAnimatorController != controller)
        {
            a.runtimeAnimatorController = controller;
            a.SetBool(jumpBool, false);
            a.SetBool(crouchBool, false);
            a.SetFloat(speedFloat, 0);
            
        }
    }
    public void SetAvatar(Avatar avatar)
    {
        behaviourManager.GetAnim.avatar = avatar;
    }
    public Animator GetAnimator
    {
        get { return behaviourManager.GetAnim; }
    }


    //Ponašanje za ciljanje

    public string aimButton = "Aim", shoulderButton = "Aim Shoulder";     // Default aim and switch shoulders buttons.
    public Texture2D crosshair;                                           // Crosshair texture.
    public float aimTurnSmoothing = 0.15f;                                // Speed of turn response when aiming to match camera facing.
    public Vector3 aimPivotOffset = new Vector3(0.5f, 1.2f, 0f);         // Offset to repoint the camera when aiming.
    public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // Offset to relocate the camera when aiming.

    private int aimBool;                                                  // Animator variable related to aiming.
    private bool aim;                                                     // Boolean to determine whether or not the player is aiming.
    private Player player;

    void ManageAim()
    {
        // Activate/deactivate aim by input.
        if (Input.GetAxisRaw(aimButton) != 0 && !aim)
        {
            StartCoroutine(ToggleAimOn());
        }
        else if (aim && Input.GetAxisRaw(aimButton) == 0)
        {
            StartCoroutine(ToggleAimOff());
        }

        // No sprinting while aiming.
        canSprint = !aim;

        // Toggle camera aim position left or right, switching shoulders.
        if (aim && Input.GetButtonDown(shoulderButton))
        {
            aimCamOffset.x = aimCamOffset.x * (-1);
            aimPivotOffset.x = aimPivotOffset.x * (-1);
        }

    }
    // Co-rountine to start aiming mode with delay.
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        // Aiming is not possible.
        if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
            yield return false;

        // Start aiming.
        else
        {
            aim = true;
            int signal = 1;
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourManager.GetAnim.SetFloat(speedFloat, 0);
            // This state overrides the active one.
          //  behaviourManager.OverrideWithBehaviour(this);
        }
    }

    // Co-rountine to end aiming mode with delay.
    private IEnumerator ToggleAimOff()
    {
        aim = false;
        yield return new WaitForSeconds(0.3f);
        behaviourManager.GetCamScript.ResetTargetOffsets();
        behaviourManager.GetCamScript.ResetMaxVerticalAngle();
        yield return new WaitForSeconds(0.05f);
     //   behaviourManager.RevokeOverridingBehaviour(this);
    }
    // LocalLateUpdate: manager is called here to set player rotation after camera rotates, avoiding flickering.
    public override void LocalLateUpdate()
    {
        if(aim) AimManagement();
    }

    // Handle aim parameters when aiming is active.
    void AimManagement()
    {
        // Deal with the player orientation when aiming.
        Rotating();
    }
    // Rotate the player to match correct orientation, according to camera.
    void Rotating()
    {
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
       
        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        float rotationOffset = 0;
        // ako igrač cilja dok dok koristi luk i strijelu, treba ga rotirat 90 stupnjeva udesno
        // kako bi pucanje izgledalo realno
        if (player.Type == Player.PlayerType.Bow)
        {
            rotationOffset = 90;
        }
        

        // Always rotates the player according to the camera horizontal rotation in aim mode.
        Quaternion targetRotation = Quaternion.Euler(0, behaviourManager.GetCamScript.GetH + rotationOffset, 0);
        
        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        // Rotate entire player to face camera.
        behaviourManager.SetLastDirection(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

    }

    // Draw the crosshair when aiming.
    void OnGUI()
    {
        if (crosshair)
        {
            float mag = behaviourManager.GetCamScript.GetCurrentPivotMagnitude(aimPivotOffset);
            if (mag < 0.05f)
                GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
                                         Screen.height / 2 - (crosshair.height * 0.5f),
                                         crosshair.width, crosshair.height), crosshair);
        }
    }
    public bool IsAiming
    {
        get { return aim; }
    }
}
