//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using UnityEngine;
//using UnityScript.Steps;
//using UnityEngine.Events;
//using UnityEngine.EventSystems;

//public class PlayerController : MonoBehaviour
//{
//	public Inventory inventory;

//	public float walkSpeed = 2;
//	public float runSpeed = 6;
//	public float gravity = -12.0f;
//	public float jumpHeight = 1;
//	//[Range(0, 1)]
//	public float airControlPercent = 2;

//	public float turnSmoothTime = 0.2f;
//	float turnSmoothVelocity;

//	public float speedSmoothTime = 0.1f;
//	float speedSmoothVelocity;
//	float currentSpeed;
//	float velocityY;

//	public Animator animator;
//	public Transform cameraT;
//	public CharacterController controller;

//	public GameObject Hand;

//	public HUD Hud;
//	public LayerMask _aimLayerMask;


//	[Tooltip("Amount of health")]
//	public int Health = 100;

//	[Tooltip("Amount of food")]
//	public int Food = 100;

//	[Tooltip("Rate in seconds in which the hunger increases")]
//	public float HungerRate = 0.5f;


//	private HealthBar mHealthBar;

//	private HealthBar mFoodBar;

//	private int startHealth;

//	private int startFood;

//	private bool mCanTakeDamage = true;

//	public event EventHandler PlayerDied;


//	void Start()
//	{

//		// slides the character through walls and edges
//		frictionPhysics = new PhysicMaterial();
//		frictionPhysics.name = "frictionPhysics";
//		frictionPhysics.staticFriction = .25f;
//		frictionPhysics.dynamicFriction = .25f;
//		frictionPhysics.frictionCombine = PhysicMaterialCombine.Multiply;

//		// prevents the collider from slipping on ramps
//		maxFrictionPhysics = new PhysicMaterial();
//		maxFrictionPhysics.name = "maxFrictionPhysics";
//		maxFrictionPhysics.staticFriction = 1f;
//		maxFrictionPhysics.dynamicFriction = 1f;
//		maxFrictionPhysics.frictionCombine = PhysicMaterialCombine.Maximum;





//		slippyPhysics = new PhysicMaterial();
//		slippyPhysics.name = "slippyPhysics";
//		slippyPhysics.staticFriction = 0f;
//		slippyPhysics.dynamicFriction = 0f;
//		slippyPhysics.frictionCombine = PhysicMaterialCombine.Minimum;




//		animator = GetComponent<Animator>();
//		animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
//		_rigidbody = GetComponent<Rigidbody>();
//		print(_rigidbody);
//		_capsuleCollider = GetComponent<CapsuleCollider>();




//		colliderCenter = GetComponent<CapsuleCollider>().center;
//		colliderRadius = GetComponent<CapsuleCollider>().radius;
//		colliderHeight = GetComponent<CapsuleCollider>().height;

//		isGrounded = true;




//		//cameraT = Camera.main.transform;
//		controller = GetComponent<CharacterController>();
//		inventory.ItemUsed += Inventory_ItemUsed;
//		inventory.ItemRemoved += Inventory_ItemRemoved;

//		mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
//		mHealthBar.Min = 0;
//		mHealthBar.Max = Health;
//		startHealth = Health;
//		mHealthBar.SetValue(Health);

//		mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
//		mFoodBar.Min = 0;
//		mFoodBar.Max = Food;
//		startFood = Food;
//		mFoodBar.SetValue(Food);

//		InvokeRepeating("IncreaseHunger", 0, HungerRate);


//		InitializeTpCamera();
//	}



//	public void IncreaseHunger()
//	{
//		Food--;
//		if (Food < 0)
//			Food = 0;

//		mFoodBar.SetValue(Food);

//		if (Food == 0)
//		{
//			CancelInvoke();
//			Die();
//		}
//	}

//	public bool IsDead
//	{
//		get
//		{
//			return Health == 0 || Food == 0;
//		}
//	}

//	public bool CarriesItem(string itemName)
//	{
//		if (mCurrentItem == null)
//			return false;

//		return (mCurrentItem.Name == itemName);
//	}





//	public void Eat(int amount)
//	{
//		Food += amount;
//		if (Food > startFood)
//		{
//			Food = startFood;
//		}

//		mFoodBar.SetValue(Food);

//	}

//	public void Rehab(int amount)
//	{
//		Health += amount;
//		if (Health > startHealth)
//		{
//			Health = startHealth;
//		}

//		mHealthBar.SetValue(Health);
//	}

//	public void TakeDamage(int amount)
//	{
//		if (!mCanTakeDamage)
//			return;

//		Health -= amount;
//		if (Health < 0)
//			Health = 0;

//		mHealthBar.SetValue(Health);

//		if (IsDead)
//		{
//			Die();
//		}

//	}


//	private void Die()
//	{
//		animator.SetTrigger("die_tr");

//		if (PlayerDied != null)
//		{
//			PlayerDied(this, EventArgs.Empty);
//		}
//	}




//	private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
//	{
//		//IInventoryItem
//		InventoryItemBase item = e.Item;

//		GameObject goItem = (item as MonoBehaviour).gameObject;
//		goItem.SetActive(true);
//		goItem.transform.parent = null;
//	}

//	private void SetItemActive(InventoryItemBase item, bool active)
//	{
//		GameObject currentItem = (item as MonoBehaviour).gameObject;
//		currentItem.SetActive(active);
//		currentItem.transform.parent = active ? Hand.transform : null;
//	}


//	private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
//	{

//		//if (e.Item.ItemType != EItemType.Consumable)
//		//{
//		print("Inventory_ItemUsed");
//		print("mCurrentItem = " + mCurrentItem);
//		if (mCurrentItem != null)
//		{
//			SetItemActive(mCurrentItem, false);
//		}
//		InventoryItemBase item = e.Item;
//		SetItemActive(item, true);
//		mCurrentItem = e.Item;
//		//mCurrentItem.transform.parent = Hand.transform;
//		//}

//		/*
//			//if (mCurrentItem != null)
//			//{
//			//	SetItemActive(mCurrentItem, false);
//			//}
//			//InventoryItemBase item = e.Item;

//			//SetItemActive(item, true);

//			///
//			//GameObject goItem = (item as MonoBehaviour).gameObject;
//			//goItem.SetActive(true);
//			//goItem.transform.parent = Hand.transform;
//			///

//			//mCurrentItem = e.Item;
//			///
//			////goItem.transform.position = Hand.transform.position;
//			//goItem.transform.localPosition = (item as InventoryItemBase).PickPosition;
//			//goItem.transform.localEulerAngles = (item as InventoryItemBase).PickRotation;
//			//Rigidbody item_rb = (item as InventoryItemBase).GetComponent<Rigidbody>();
//			//item_rb.freezeRotation = true;
//			//item_rb.useGravity = false;
//			///

//		*/

//	}


//	private InventoryItemBase mCurrentItem = null;
//	private bool mLockPickup = false;
//	private void DropCurrentItem()
//	{
//		mLockPickup = true;
//		animator.SetTrigger("drop_tr");
//		GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;
//		inventory.RemoveItem(mCurrentItem);
//		mCurrentItem.OnDrop();

//		//Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
//		Rigidbody rbItem = goItem.GetComponent<Rigidbody>();
//		if (rbItem != null)
//		{
//			print("gdfgdfgdfg " + transform.forward);
//			rbItem.AddForce(transform.forward * 1.0f, ForceMode.Impulse);
//			Invoke("DoDropItem", 0.25f);
//		}
//	}
//	public void DoDropItem()
//	{
//		mLockPickup = false;
//		if (mCurrentItem != null)
//		{

//			print("can pickup " + mCurrentItem.Name);
//			//Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());
//			mCurrentItem = null;
//		}
//	}


//	private bool mIsControlEnabled = true;
//	public void EnableControl()
//	{
//		mIsControlEnabled = true;
//	}
//	public void DisableControl()
//	{
//		mIsControlEnabled = false;
//	}


//	public virtual void OnAnimatorMove()
//	{
//		ControlAnimatorRootMotion(); // handle root motion animations 
//	}


//	#region Basic Locomotion Inputs

//	protected virtual void InitializeTpCamera()
//	{
//		if (tpCamera == null)
//		{
//			tpCamera = FindObjectOfType<PlayerCamera>();
//			if (tpCamera == null)
//				return;
//			if (tpCamera)
//			{
//				tpCamera.SetMainTarget(this.transform);
//				tpCamera.Init();
//			}
//		}
//	}

//	protected virtual void InputHandle()
//	{
//		MoveInput();
//		CameraInput();
//		SprintInput();
//		StrafeInput();
//		JumpInput();
//	}

//	public virtual void MoveInput()
//	{
//		input.x = Input.GetAxis("Horizontal");
//		input.z = Input.GetAxis("Vertical");
//	}

//	protected virtual void CameraInput()
//	{
//		if (!cameraMain)
//		{
//			if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
//			else
//			{
//				cameraMain = Camera.main;
//				rotateTarget = cameraMain.transform;
//			}
//		}

//		if (cameraMain)
//		{
//			UpdateMoveDirection(cameraMain.transform);
//		}

//		if (tpCamera == null)
//			return;

//		var Y = Input.GetAxis("Mouse Y");
//		var X = Input.GetAxis("Mouse X");

//		tpCamera.RotateCamera(X, Y);
//	}

//	protected virtual void StrafeInput()
//	{
//		if (Input.GetKeyDown(KeyCode.Tab))
//			Strafe();
//	}

//	protected virtual void SprintInput()
//	{
//		if (Input.GetKeyDown(KeyCode.LeftShift))
//			Sprint(true);
//		else if (Input.GetKeyUp(KeyCode.LeftShift))
//			Sprint(false);
//	}

//	/// <summary>
//	/// Conditions to trigger the Jump animation & behavior
//	/// </summary>
//	/// <returns></returns>
//	protected virtual bool JumpConditions()
//	{
//		return isGrounded && GroundAngle() < slopeLimit && !isJumping && !stopMove;
//	}

//	/// <summary>
//	/// Input to trigger the Jump 
//	/// </summary>
//	protected virtual void JumpInput()
//	{
//		if (Input.GetKeyDown(KeyCode.Space) && JumpConditions())
//			Jump();
//	}

//	#endregion



//	void FixedUpdate()
//	{
//		UpdateMotor();               // updates the ThirdPersonMotor methods
//		ControlLocomotionType();     // handle the controller locomotion type and movespeed
//		ControlRotationType();



//		if (!IsDead)
//		{
//			//DropCurrentItem();
//		}

//	}



//	#region Animator  

//	public virtual void UpdateAnimator()
//	{
//		if (animator == null || !animator.enabled) return;

//		animator.SetBool(vAnimatorParameters.IsStrafing, isStrafing); ;
//		animator.SetBool(vAnimatorParameters.IsSprinting, isSprinting);
//		animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
//		animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);

//		if (isStrafing)
//		{
//			animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
//			animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
//		}
//		else
//		{
//			animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
//		}

//		animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
//	}

//	public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
//	{
//		Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
//		verticalSpeed = relativeInput.z;
//		horizontalSpeed = relativeInput.x;

//		var newInput = new Vector2(verticalSpeed, horizontalSpeed);

//		if (speed.walkByDefault)
//			inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
//		else
//			inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
//	}



//	public static partial class vAnimatorParameters
//	{
//		public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
//		public static int InputVertical = Animator.StringToHash("InputVertical");
//		public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
//		public static int IsGrounded = Animator.StringToHash("IsGrounded");
//		public static int IsStrafing = Animator.StringToHash("IsStrafing");
//		public static int IsSprinting = Animator.StringToHash("IsSprinting");
//		public static int GroundDistance = Animator.StringToHash("GroundDistance");
//	}
//	#endregion


//	void Update()
//	{
//		if (!IsDead && mIsControlEnabled)
//		{


//			HeadRotate();

//			if (Input.GetKeyDown(KeyCode.LeftControl))
//				animator.SetBool("sit", true);
//			else if (Input.GetKeyUp(KeyCode.LeftControl))
//				animator.SetBool("sit", false);


//			if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
//			{
//				print("mInteractItem = " + mInteractItem);
//				mInteractItem.OnInteractAnimation(animator);
//				InteractWithItem();

//				/*
//				inventory.AddItem(mItemToPickup);
//				//mItemToPickup.OnPickup();
//				animator.SetTrigger("drop_tr");
//				Hud.CloseMessagePanel();
//				*/
//			}

//			if (mCurrentItem != null && Input.GetMouseButtonDown(0))
//			{
//				print("mCurrentItem " + mCurrentItem);
//				if (!EventSystem.current.IsPointerOverGameObject())
//				{
//					animator.SetTrigger("punch_tr");
//				}
//			}

//			if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
//			{
//				print("R pressed");
//				DropCurrentItem();

//			}


//			InputHandle();                 
//			UpdateAnimator();
//		}
//	}

//	public static float AngleBetweenVectors_rad(Vector3 vec1, Vector3 vec2)
//	{
//		return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
//	}

//	public static float AngleBetweenVectors_deg(Vector3 vec1, Vector3 vec2)
//	{
//		return AngleBetweenVectors_rad(vec1, vec2) * 180 / Mathf.PI;
//	}

//	Vector3 GetLocalDirection(Transform transform, Vector3 destination)
//	{
//		return transform.InverseTransformDirection((destination - transform.position).normalized);
//	}

//	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
//	{
//		// angle in [0,180]
//		float angle = Vector3.Angle(a, b);
//		float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

//		// angle in [-179,180]
//		float signed_angle = angle * sign;

//		// angle in [0,360] (not used but included here for completeness)
//		//float angle360 =  (signed_angle + 180) % 360;

//		return signed_angle;
//	}

//	float signedAngleBetween(Vector3 a, Vector3 b, bool clockwise)
//	{
//		float angle = Vector3.Angle(a, b);

//		//clockwise
//		if (Mathf.Sign(angle) == -1 && clockwise)
//			angle = 360 + angle;

//		//counter clockwise
//		else if (Mathf.Sign(angle) == 1 && !clockwise)
//			angle = -angle;
//		return angle;
//	}

//	public static float Clamp0360(float eulerAngles)
//	{
//		float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
//		if (result < 0)
//		{
//			result += 360f;
//		}
//		return result;
//	}

//	private static float WrapAngle(float angle)
//	{
//		angle %= 360;
//		if (angle > 180)
//			return angle - 360;

//		return angle;
//	}

//	private static float UnwrapAngle(float angle)
//	{
//		if (angle >= 0)
//			return angle;

//		angle = -angle % 360;

//		return 360 - angle;
//	}

//	private void HeadRotate()
//	{
//		var playerVectorHor = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
//		var cameraVectorHor = new Vector3(cameraT.transform.forward.x, 0.0f, cameraT.transform.forward.z);
//		var horDiffAngle = Vector3.SignedAngle(playerVectorHor, cameraVectorHor, Vector3.up);

//		//var playerVectorVer = new Vector3(0.0f, transform.forward.y, transform.forward.z);
//		//var cameraVectorVer = new Vector3(0.0f, cameraT.transform.forward.y, cameraT.transform.forward.z);
//		//var verDiffAngle = Vector3.SignedAngle(new Vector3(0,1,0), cameraVectorVer, Vector3.right);
//		//var verDiffAngle = SignedAngleBetween(playerVectorVer, cameraVectorVer, Vector3.right);
//		//var verDiffAngle = signedAngleBetween(playerVectorVer, cameraVectorVer, true);
//		//print(verDiffAngle);

//		//var playerVectorVer = new Vector3(0.0f, cameraT.transform.forward.y, cameraT.transform.forward.z);
//		//var cameraVectorVer = new Vector3(0.0f, cameraT.transform.position.y, cameraT.transform.position.z);

//		//Vector3 targetDir = cameraT.transform.position - transform.position;
//		//float angle = Vector3.SignedAngle(playerVectorVer, cameraVectorVer, Vector3.right);
//		//print(cameraT.transform.rotation.x);
//		var verDiffAngle = WrapAngle(cameraT.eulerAngles.x);
//		animator.SetFloat("rotate_hor", horDiffAngle, speedSmoothTime, Time.deltaTime);
//		animator.SetFloat("rotate_ver", verDiffAngle, speedSmoothTime, Time.deltaTime);
//	}





//	public virtual void UpdateMotor()
//	{
//		CheckGround();
//		CheckSlopeLimit();
//		ControlJumpBehaviour();
//		AirControl();
//	}

//	#region Locomotion

//	public virtual void SetControllerMoveSpeed(vMovementSpeed speed)
//	{
//		if (speed.walkByDefault)
//			moveSpeed = Mathf.Lerp(moveSpeed, isSprinting ? speed.runningSpeed : speed.walkSpeed, speed.movementSmooth * Time.deltaTime);
//		else
//			moveSpeed = Mathf.Lerp(moveSpeed, isSprinting ? speed.sprintSpeed : speed.runningSpeed, speed.movementSmooth * Time.deltaTime);
//	}

//	public virtual void MoveCharacter(Vector3 _direction)
//	{
//		// calculate input smooth
//		inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

//		if (!isGrounded || isJumping) return;

//		_direction.y = 0;
//		_direction.x = Mathf.Clamp(_direction.x, -1f, 1f);
//		_direction.z = Mathf.Clamp(_direction.z, -1f, 1f);
//		// limit the input
//		if (_direction.magnitude > 1f)
//			_direction.Normalize();

//		Vector3 targetPosition = (useRootMotion ? animator.rootPosition : _rigidbody.position) + _direction * (stopMove ? 0 : moveSpeed) * Time.deltaTime;
//		Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

//		bool useVerticalVelocity = true;
//		if (useVerticalVelocity) targetVelocity.y = _rigidbody.velocity.y;
//		_rigidbody.velocity = targetVelocity;
//	}



//	public virtual void CheckSlopeLimit()
//	{
//		if (input.sqrMagnitude < 0.1) return;

//		RaycastHit hitinfo;
//		var hitAngle = 0f;

//		if (Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), transform.position + moveDirection.normalized * (_capsuleCollider.radius + 0.2f), out hitinfo, groundLayer))
//		{
//			hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

//			var targetPoint = hitinfo.point + moveDirection.normalized * _capsuleCollider.radius;
//			if ((hitAngle > slopeLimit) && Physics.Linecast(transform.position + Vector3.up * (_capsuleCollider.height * 0.5f), targetPoint, out hitinfo, groundLayer))
//			{
//				hitAngle = Vector3.Angle(Vector3.up, hitinfo.normal);

//				if (hitAngle > slopeLimit && hitAngle < 85f)
//				{
//					stopMove = true;
//					return;
//				}
//			}
//		}
//		stopMove = false;
//	}

//	public virtual void RotateToPosition(Vector3 position)
//	{
//		Vector3 desiredDirection = position - transform.position;
//		RotateToDirection(desiredDirection.normalized);
//	}

//	public virtual void RotateToDirection(Vector3 direction)
//	{
//		RotateToDirection(direction, isStrafing ? strafeSpeed.rotationSpeed : freeSpeed.rotationSpeed);
//	}

//	public virtual void RotateToDirection(Vector3 direction, float rotationSpeed)
//	{
//		if (!jumpAndRotate && !isGrounded) return;
//		direction.y = 0f;
//		Vector3 desiredForward = Vector3.RotateTowards(transform.forward, direction.normalized, rotationSpeed * Time.deltaTime, .1f);
//		Quaternion _newRotation = Quaternion.LookRotation(desiredForward);
//		transform.rotation = _newRotation;
//	}

//	#endregion

//	#region Jump Methods

//	protected virtual void ControlJumpBehaviour()
//	{
//		if (!isJumping) return;

//		jumpCounter -= Time.deltaTime;
//		if (jumpCounter <= 0)
//		{
//			jumpCounter = 0;
//			isJumping = false;
//		}
//		// apply extra force to the jump height   
//		var vel = _rigidbody.velocity;
//		vel.y = jumpHeight;
//		_rigidbody.velocity = vel;
//	}

//	public virtual void AirControl()
//	{
//		if ((isGrounded && !isJumping)) return;
//		if (transform.position.y > heightReached) heightReached = transform.position.y;
//		inputSmooth = Vector3.Lerp(inputSmooth, input, airSmooth * Time.deltaTime);

//		if (jumpWithRigidbodyForce && !isGrounded)
//		{
//			_rigidbody.AddForce(moveDirection * airSpeed * Time.deltaTime, ForceMode.VelocityChange);
//			return;
//		}

//		moveDirection.y = 0;
//		moveDirection.x = Mathf.Clamp(moveDirection.x, -1f, 1f);
//		moveDirection.z = Mathf.Clamp(moveDirection.z, -1f, 1f);

//		Vector3 targetPosition = _rigidbody.position + (moveDirection * airSpeed) * Time.deltaTime;
//		Vector3 targetVelocity = (targetPosition - transform.position) / Time.deltaTime;

//		targetVelocity.y = _rigidbody.velocity.y;
//		_rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, targetVelocity, airSmooth * Time.deltaTime);
//	}

//	protected virtual bool jumpFwdCondition
//	{
//		get
//		{
//			Vector3 p1 = transform.position + _capsuleCollider.center + Vector3.up * -_capsuleCollider.height * 0.5F;
//			Vector3 p2 = p1 + Vector3.up * _capsuleCollider.height;
//			return Physics.CapsuleCastAll(p1, p2, _capsuleCollider.radius * 0.5f, transform.forward, 0.6f, groundLayer).Length == 0;
//		}
//	}

//	#endregion

             

//	protected virtual void CheckGround()
//	{
//		CheckGroundDistance();
//		ControlMaterialPhysics();

//		if (groundDistance <= groundMinDistance)
//		{
//			isGrounded = true;
//			if (!isJumping && groundDistance > 0.05f)
//				_rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);

//			heightReached = transform.position.y;
//		}
//		else
//		{
//			if (groundDistance >= groundMaxDistance)
//			{
//				// set IsGrounded to false 
//				isGrounded = false;
//				// check vertical velocity
//				verticalVelocity = _rigidbody.velocity.y;
//				print("verticalVelocity:" + verticalVelocity);
//				// apply extra gravity when falling
//				if (!isJumping)
//				{
//					_rigidbody.AddForce(transform.up * extraGravity * Time.deltaTime, ForceMode.VelocityChange);
//				}
//			}
//			else if (!isJumping)
//			{
//				_rigidbody.AddForce(transform.up * (extraGravity * 2 * Time.deltaTime), ForceMode.VelocityChange);
//			}
//		}
//	}

//	protected virtual void ControlMaterialPhysics()
//	{
//		// change the physics material to very slip when not grounded
//		_capsuleCollider.material = (isGrounded && GroundAngle() <= slopeLimit + 1) ? frictionPhysics : slippyPhysics;

//		if (isGrounded && input == Vector3.zero)
//			_capsuleCollider.material = maxFrictionPhysics;
//		else if (isGrounded && input != Vector3.zero)
//			_capsuleCollider.material = frictionPhysics;
//		else
//			_capsuleCollider.material = slippyPhysics;
//	}

//	protected virtual void CheckGroundDistance()
//	{
//		if (_capsuleCollider != null)
//		{
//			// radius of the SphereCast
//			float radius = _capsuleCollider.radius * 0.9f;
//			var dist = 10f;
//			// ray for RayCast
//			Ray ray2 = new Ray(transform.position + new Vector3(0, colliderHeight / 2, 0), Vector3.down);
//			// raycast for check the ground distance
//			if (Physics.Raycast(ray2, out groundHit, (colliderHeight / 2) + dist, groundLayer) && !groundHit.collider.isTrigger)
//				dist = transform.position.y - groundHit.point.y;
//			// sphere cast around the base of the capsule to check the ground distance
//			if (dist >= groundMinDistance)
//			{
//				Vector3 pos = transform.position + Vector3.up * (_capsuleCollider.radius);
//				Ray ray = new Ray(pos, -Vector3.up);
//				if (Physics.SphereCast(ray, radius, out groundHit, _capsuleCollider.radius + groundMaxDistance, groundLayer) && !groundHit.collider.isTrigger)
//				{
//					Physics.Linecast(groundHit.point + (Vector3.up * 0.1f), groundHit.point + Vector3.down * 0.15f, out groundHit, groundLayer);
//					float newDist = transform.position.y - groundHit.point.y;
//					if (dist > newDist) dist = newDist;
//				}
//			}
//			groundDistance = (float)System.Math.Round(dist, 2);
//			print("groundDistance:" + groundDistance);
//		}
//	}

//	public virtual float GroundAngle()
//	{
//		var groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);
//		return groundAngle;
//	}

//	public virtual float GroundAngleFromDirection()
//	{
//		var dir = isStrafing && input.magnitude > 0 ? (transform.right * input.x + transform.forward * input.z).normalized : transform.forward;
//		var movementAngle = Vector3.Angle(dir, groundHit.normal) - 90;
//		return movementAngle;
//	}




//	internal Vector3 inputSmooth;
//	public bool useRootMotion = false;
//	internal Vector3 moveDirection;
//	internal bool lockMovement = false;
//	internal bool lockRotation = false;
//	public enum LocomotionType
//	{
//		FreeWithStrafe,
//		OnlyStrafe,
//		OnlyFree,
//	}
//	public LocomotionType locomotionType = LocomotionType.FreeWithStrafe;

//	internal bool _isStrafing;
//	internal bool isStrafing
//	{
//		get
//		{
//			return _isStrafing;
//		}
//		set
//		{
//			_isStrafing = value;
//		}
//	}

//	[System.Serializable]
//	public class vMovementSpeed
//	{
//		[Range(1f, 20f)]
//		public float movementSmooth = 6f;
//		[Range(0f, 1f)]
//		public float animationSmooth = 0.2f;
//		[Tooltip("Rotation speed of the character")]
//		public float rotationSpeed = 16f;
//		[Tooltip("Character will limit the movement to walk instead of running")]
//		public bool walkByDefault = false;
//		[Tooltip("Rotate with the Camera forward when standing idle")]
//		public bool rotateWithCamera = false;
//		[Tooltip("Speed to Walk using rigidbody or extra speed if you're using RootMotion")]
//		public float walkSpeed = 2f;
//		[Tooltip("Speed to Run using rigidbody or extra speed if you're using RootMotion")]
//		public float runningSpeed = 4f;
//		[Tooltip("Speed to Sprint using rigidbody or extra speed if you're using RootMotion")]
//		public float sprintSpeed = 6f;
//	}

//	internal float moveSpeed;
//	public vMovementSpeed freeSpeed, strafeSpeed;
//	internal bool isSprinting { get; set; }
//	internal Vector3 input;
//	internal float inputMagnitude;                      // sets the inputMagnitude to update the animations in the animator controller
//	internal float verticalSpeed;                       // set the verticalSpeed based on the verticalInput
//	internal float horizontalSpeed;
//	internal bool isGrounded { get; set; }
//	internal bool isJumping;
//	internal Rigidbody _rigidbody;
//	internal CapsuleCollider _capsuleCollider;
//	public bool stopMove { get; protected set; }
//	public LayerMask groundLayer = 1 << 0;
//	[Range(30, 80)] public float slopeLimit = 75f;
//	public bool jumpAndRotate = true;
//	internal float jumpCounter;
//	internal float heightReached;
//	public float airSmooth = 6f;
//	public bool jumpWithRigidbodyForce = false;
//	public float airSpeed = 5f;
//	internal float groundDistance;
//	public float groundMinDistance = 0.25f;
//	public float extraGravity = -10f;
//	public float groundMaxDistance = 0.5f;
//	internal float verticalVelocity;
//	internal PhysicMaterial frictionPhysics, maxFrictionPhysics, slippyPhysics;
//	internal float colliderRadius, colliderHeight;
//	internal Vector3 colliderCenter;
//	internal RaycastHit groundHit;
//	public float runningSpeed = 4f;
//	public float sprintSpeed = 6f;
//	public bool sprintOnlyFree = true;
//	public bool rotateByWorld = false;
//	internal Transform rotateTarget;
//	public bool useContinuousSprint = true;
//	public float jumpTimer = 0.3f;
//	public PlayerCamera tpCamera;
//	public Camera cameraMain;





//	public virtual void ControlAnimatorRootMotion()
//	{
//		//if (!this.enabled) return;

//		if (inputSmooth == Vector3.zero)
//		{
//			transform.position = animator.rootPosition;
//			transform.rotation = animator.rootRotation;
//		}

//		if (useRootMotion)
//			MoveCharacter(moveDirection);
//	}

//	public virtual void ControlLocomotionType()
//	{
//		if (lockMovement) return;

//		if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
//		{
//			SetControllerMoveSpeed(freeSpeed);
//			SetAnimatorMoveSpeed(freeSpeed);
//		}
//		else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
//		{
//			isStrafing = true;
//			SetControllerMoveSpeed(strafeSpeed);
//			SetAnimatorMoveSpeed(strafeSpeed);
//		}

//		if (!useRootMotion)
//			MoveCharacter(moveDirection);
//	}


//	public virtual void ControlRotationType()
//	{
//		if (lockRotation) return;

//		bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

//		if (validInput)
//		{
//			// calculate input smooth
//			inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

//			Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
//			RotateToDirection(dir);
//		}
//	}

//	public virtual void UpdateMoveDirection(Transform referenceTransform = null)
//	{
//		if (input.magnitude <= 0.01)
//		{
//			moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
//			return;
//		}

//		if (referenceTransform && !rotateByWorld)
//		{
//			//get the right-facing direction of the referenceTransform
//			var right = referenceTransform.right;
//			right.y = 0;
//			//get the forward direction relative to referenceTransform Right
//			var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
//			// determine the direction the player will face based on input and the referenceTransform's right and forward directions
//			moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
//		}
//		else
//		{
//			moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
//		}
//	}

//	public virtual void Sprint(bool value)
//	{
//		var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
//			!(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

//		if (value && sprintConditions)
//		{
//			if (input.sqrMagnitude > 0.1f)
//			{
//				if (isGrounded && useContinuousSprint)
//				{
//					isSprinting = !isSprinting;
//				}
//				else if (!isSprinting)
//				{
//					isSprinting = true;
//				}
//			}
//			else if (!useContinuousSprint && isSprinting)
//			{
//				isSprinting = false;
//			}
//		}
//		else if (isSprinting)
//		{
//			isSprinting = false;
//		}
//	}

//	public virtual void Strafe()
//	{
//		isStrafing = !isStrafing;
//	}

//	public virtual void Jump()
//	{
//		// trigger jump behaviour
//		jumpCounter = jumpTimer;
//		isJumping = true;

//		// trigger jump animations
//		if (input.sqrMagnitude < 0.1f)
//			animator.CrossFadeInFixedTime("Jump", 0.1f);
//		else
//			animator.CrossFadeInFixedTime("JumpMove", .2f);
//	}


//	public void InteractWithItem()
//	{
//		print("public void InteractWithItem()");
//		if (mInteractItem != null)
//		{
//			mInteractItem.OnInteract();

//			if (mInteractItem is InventoryItemBase)
//			{
//				InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
//				inventory.AddItem(inventoryItem);
//				inventoryItem.OnPickup();

//				if (inventoryItem.UseItemAfterPickup)
//				{
//					inventory.UseItem(inventoryItem);
//					//inventoryItem.OnUse();


//				}
//				Hud.CloseMessagePanel();
//				mInteractItem = null;
//			}
//			//else
//			//{
//			//    if (mInteractItem.ContinueInteract())
//			//    {
//			//        Hud.OpenMessagePanel(mInteractItem);
//			//    }
//			//    else
//			//    {
//			//        Hud.CloseMessagePanel();
//			//        mInteractItem = null;
//			//    }
//			//}
//		}
//	}

//	private InteractableItemBase mInteractItem = null;


//	private void OnTriggerEnter(Collider other)
//	{
//		TryInteraction(other);
//	}

//	private void TryInteraction(Collider other)
//	{
//		InteractableItemBase item = other.GetComponent<InteractableItemBase>();

//		if (item != null)
//		{
//			if (item.CanInteract(other))
//			{
//				mInteractItem = item;

//				Hud.OpenMessagePanel(mInteractItem);
//			}
//		}

//		/*
//		if (item != null)
//		{
//			if (mLockPickup)
//				return;
//			mItemToPickup = item;
//			//inventory.AddItem(item);
//			//item.OnPickup();
//			Hud.OpenMessagePanel("");
//		}
//		*/
//	}

//	private void OnTriggerExit(Collider other)
//	{
//		InventoryItemBase item = other.GetComponent<InventoryItemBase>();
//		if (item != null)
//		{
//			Hud.CloseMessagePanel();
//			mInteractItem = null;
//		}
//	}

//	private void OnControllerColliderHit(ControllerColliderHit hit)
//	{
//		/*
//		if (mLockPickup)
//			return;
//		IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
//		if (item != null)
//		{
//			inventory.AddItem(item);
//		}
//		*/
//	}

//}


