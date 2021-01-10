using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityScript.Steps;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
	public Inventory inventory;

	public float walkSpeed = 2;
	public float runSpeed = 6;
	public float gravity = -12.0f;
	public float jumpHeight = 1;
	//[Range(0, 1)]
	public float airControlPercent = 2;

	public float turnSmoothTime = 0.2f;
	float turnSmoothVelocity;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float velocityY;

	public Animator animator;
	public Transform cameraT;
	public CharacterController controller;

	public GameObject Hand;

	public HUD Hud;
	public LayerMask _aimLayerMask;


	[Tooltip("Amount of health")]
	public int Health = 100;

	[Tooltip("Amount of food")]
	public int Food = 100;

	[Tooltip("Rate in seconds in which the hunger increases")]
	public float HungerRate = 0.5f;


	private HealthBar mHealthBar;

	private HealthBar mFoodBar;

	private int startHealth;

	private int startFood;

	private bool mCanTakeDamage = true;

	public event EventHandler PlayerDied;


	void Start()
	{
		animator = GetComponent<Animator>();
		//cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController>();
		inventory.ItemUsed += Inventory_ItemUsed;
		inventory.ItemRemoved += Inventory_ItemRemoved;

		mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
		mHealthBar.Min = 0;
		mHealthBar.Max = Health;
		startHealth = Health;
		mHealthBar.SetValue(Health);

		mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
		mFoodBar.Min = 0;
		mFoodBar.Max = Food;
		startFood = Food;
		mFoodBar.SetValue(Food);

		InvokeRepeating("IncreaseHunger", 0, HungerRate);
	}



	public void IncreaseHunger()
	{
		Food--;
		if (Food < 0)
			Food = 0;

		mFoodBar.SetValue(Food);

		if (Food == 0)
		{
			CancelInvoke();
			Die();
		}
	}

	public bool IsDead
	{
		get
		{
			return Health == 0 || Food == 0;
		}
	}

	public bool CarriesItem(string itemName)
	{
		if (mCurrentItem == null)
			return false;

		return (mCurrentItem.Name == itemName);
	}





	public void Eat(int amount)
	{
		Food += amount;
		if (Food > startFood)
		{
			Food = startFood;
		}

		mFoodBar.SetValue(Food);

	}

	public void Rehab(int amount)
	{
		Health += amount;
		if (Health > startHealth)
		{
			Health = startHealth;
		}

		mHealthBar.SetValue(Health);
	}

	public void TakeDamage(int amount)
	{
		if (!mCanTakeDamage)
			return;

		Health -= amount;
		if (Health < 0)
			Health = 0;

		mHealthBar.SetValue(Health);

		if (IsDead)
		{
			Die();
		}

	}


	private void Die()
	{
		animator.SetTrigger("die_tr");

		if (PlayerDied != null)
		{
			PlayerDied(this, EventArgs.Empty);
		}
	}




	private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
	{
		//IInventoryItem
		InventoryItemBase item = e.Item;

		GameObject goItem = (item as MonoBehaviour).gameObject;
		goItem.SetActive(true);
		goItem.transform.parent = null;
	}

	private void SetItemActive(InventoryItemBase item, bool active)
	{
		GameObject currentItem = (item as MonoBehaviour).gameObject;
		currentItem.SetActive(active);
		currentItem.transform.parent = active ? Hand.transform : null;
	}


	private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
	{

		//if (e.Item.ItemType != EItemType.Consumable)
		//{
		print("Inventory_ItemUsed");
		print("mCurrentItem = " + mCurrentItem);
		if (mCurrentItem != null)
		{
			SetItemActive(mCurrentItem, false);
		}
		InventoryItemBase item = e.Item;
		SetItemActive(item, true);
		mCurrentItem = e.Item;
		//mCurrentItem.transform.parent = Hand.transform;
		//}

		/*
			//if (mCurrentItem != null)
			//{
			//	SetItemActive(mCurrentItem, false);
			//}
			//InventoryItemBase item = e.Item;

			//SetItemActive(item, true);

			///
			//GameObject goItem = (item as MonoBehaviour).gameObject;
			//goItem.SetActive(true);
			//goItem.transform.parent = Hand.transform;
			///

			//mCurrentItem = e.Item;
			///
			////goItem.transform.position = Hand.transform.position;
			//goItem.transform.localPosition = (item as InventoryItemBase).PickPosition;
			//goItem.transform.localEulerAngles = (item as InventoryItemBase).PickRotation;
			//Rigidbody item_rb = (item as InventoryItemBase).GetComponent<Rigidbody>();
			//item_rb.freezeRotation = true;
			//item_rb.useGravity = false;
			///

		*/

	}


	private InventoryItemBase mCurrentItem = null;
	private bool mLockPickup = false;
	private void DropCurrentItem()
	{
		mLockPickup = true;
		animator.SetTrigger("drop_tr");
		GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;
		inventory.RemoveItem(mCurrentItem);
		mCurrentItem.OnDrop();

		//Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
		Rigidbody rbItem = goItem.GetComponent<Rigidbody>();
		if (rbItem != null)
		{
			print("gdfgdfgdfg " + transform.forward);
			rbItem.AddForce(transform.forward * 1.0f, ForceMode.Impulse);
			Invoke("DoDropItem", 0.25f);
		}
	}
	public void DoDropItem()
	{
		mLockPickup = false;
		if (mCurrentItem != null)
		{

			print("can pickup " + mCurrentItem.Name);
			//Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());
			mCurrentItem = null;
		}
	}


	private bool mIsControlEnabled = true;
	public void EnableControl()
	{
		mIsControlEnabled = true;
	}
	public void DisableControl()
	{
		mIsControlEnabled = false;
	}
	void FixedUpdate()
	{
		if (!IsDead)
		{
			//DropCurrentItem();
		}

	}




	void Update()
	{
		if (!IsDead && mIsControlEnabled)
		{


			HeadRotate();

			if (Input.GetKeyDown(KeyCode.LeftControl))
				animator.SetBool("sit", true);
			else if (Input.GetKeyUp(KeyCode.LeftControl))
				animator.SetBool("sit", false);


			if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
			{
				print("mInteractItem = " + mInteractItem);
				mInteractItem.OnInteractAnimation(animator);
				InteractWithItem();

				/*
				inventory.AddItem(mItemToPickup);
				//mItemToPickup.OnPickup();
				animator.SetTrigger("drop_tr");
				Hud.CloseMessagePanel();
				*/
			}

			if (mCurrentItem != null && Input.GetMouseButtonDown(0))
			{
				print("mCurrentItem " + mCurrentItem);
				if (!EventSystem.current.IsPointerOverGameObject())
				{
					animator.SetTrigger("punch_tr");
				}
			}

			if (mCurrentItem != null && Input.GetKeyDown(KeyCode.R))
			{
				print("R pressed");
				DropCurrentItem();

			}


			if (Input.GetKeyDown(KeyCode.Space))
			{
				animator.SetBool("jump", true);
				Jump();
			}


			// input
			Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			Vector2 inputDir = input.normalized;
			bool running = Input.GetKey(KeyCode.LeftShift);


			if (mCurrentItem == null && Input.GetMouseButton(1))
			{

				//AimTowardMouse();
				animator.SetBool("RBM_down", true);
			}
			else if (Input.GetMouseButtonUp(1))
				animator.SetBool("RBM_down", false);



			MoveAiming(inputDir, running);
			//Move(inputDir, running);


			// animator
			float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);


			var dirSpeed = inputDir * animationSpeedPercent;
			animator.SetFloat("dirSpeedHor", dirSpeed.x);
			animator.SetFloat("dirSpeedVer", dirSpeed.y);
			animator.SetFloat("speed", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

		}
	}

	public static float AngleBetweenVectors_rad(Vector3 vec1, Vector3 vec2)
	{
		return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
	}

	public static float AngleBetweenVectors_deg(Vector3 vec1, Vector3 vec2)
	{
		return AngleBetweenVectors_rad(vec1, vec2) * 180 / Mathf.PI;
	}

	Vector3 GetLocalDirection(Transform transform, Vector3 destination)
	{
		return transform.InverseTransformDirection((destination - transform.position).normalized);
	}

	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
	{
		// angle in [0,180]
		float angle = Vector3.Angle(a, b);
		float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));

		// angle in [-179,180]
		float signed_angle = angle * sign;

		// angle in [0,360] (not used but included here for completeness)
		//float angle360 =  (signed_angle + 180) % 360;

		return signed_angle;
	}

	float signedAngleBetween(Vector3 a, Vector3 b, bool clockwise)
	{
		float angle = Vector3.Angle(a, b);

		//clockwise
		if (Mathf.Sign(angle) == -1 && clockwise)
			angle = 360 + angle;

		//counter clockwise
		else if (Mathf.Sign(angle) == 1 && !clockwise)
			angle = -angle;
		return angle;
	}

	public static float Clamp0360(float eulerAngles)
	{
		float result = eulerAngles - Mathf.CeilToInt(eulerAngles / 360f) * 360f;
		if (result < 0)
		{
			result += 360f;
		}
		return result;
	}

	private static float WrapAngle(float angle)
	{
		angle %= 360;
		if (angle > 180)
			return angle - 360;

		return angle;
	}

	private static float UnwrapAngle(float angle)
	{
		if (angle >= 0)
			return angle;

		angle = -angle % 360;

		return 360 - angle;
	}

	private void HeadRotate()
	{
		var playerVectorHor = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
		var cameraVectorHor = new Vector3(cameraT.transform.forward.x, 0.0f, cameraT.transform.forward.z);
		var horDiffAngle = Vector3.SignedAngle(playerVectorHor, cameraVectorHor, Vector3.up);

		//var playerVectorVer = new Vector3(0.0f, transform.forward.y, transform.forward.z);
		//var cameraVectorVer = new Vector3(0.0f, cameraT.transform.forward.y, cameraT.transform.forward.z);
		//var verDiffAngle = Vector3.SignedAngle(new Vector3(0,1,0), cameraVectorVer, Vector3.right);
		//var verDiffAngle = SignedAngleBetween(playerVectorVer, cameraVectorVer, Vector3.right);
		//var verDiffAngle = signedAngleBetween(playerVectorVer, cameraVectorVer, true);
		//print(verDiffAngle);

		//var playerVectorVer = new Vector3(0.0f, cameraT.transform.forward.y, cameraT.transform.forward.z);
		//var cameraVectorVer = new Vector3(0.0f, cameraT.transform.position.y, cameraT.transform.position.z);

		//Vector3 targetDir = cameraT.transform.position - transform.position;
		//float angle = Vector3.SignedAngle(playerVectorVer, cameraVectorVer, Vector3.right);
		//print(cameraT.transform.rotation.x);
		var verDiffAngle = WrapAngle(cameraT.eulerAngles.x);
		animator.SetFloat("rotate_hor", horDiffAngle, speedSmoothTime, Time.deltaTime);
		animator.SetFloat("rotate_ver", verDiffAngle, speedSmoothTime, Time.deltaTime);
	}




	private void MoveAiming(Vector2 inputDir, bool running)
	{

		Vector3 direction = new Vector3(inputDir.x, 0f, inputDir.y);






		float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
		//print("targetRotation:" + targetRotation);
		//transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		//AimTowardMouse();

		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		//print("inputDir.magnitude:" + inputDir.magnitude + " inputDir:" + inputDir);
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		if (direction.magnitude >= 0.1f)
		{
			controller.Move(direction * currentSpeed * Time.deltaTime);

		}

		velocityY += Time.deltaTime * gravity;

		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
		//print("velocityY = " + velocityY + " velocity = " + velocity);
		//controller.Move(velocity * Time.deltaTime);
		currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
		//print("currentSpeed:" + currentSpeed);
		if (controller.isGrounded)
		{

			velocityY = 0;
			animator.SetBool("jump", false);
		}

	}


	private void Move(Vector2 inputDir, bool running)
	{


		if (inputDir != Vector2.zero)
		{
			//print("inputDir = " + inputDir);
			if (inputDir.y <= 0)
			{
				//print(inputDir.y);
			}
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			//print("targetRotation:" + targetRotation);
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

		}


		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		//print("inputDir.magnitude:" + inputDir.magnitude + " inputDir:" + inputDir);
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;

		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
		//print("velocityY = " + velocityY + " velocity = " + velocity);
		controller.Move(velocity * Time.deltaTime);
		currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
		//print("currentSpeed:" + currentSpeed);
		if (controller.isGrounded)
		{

			velocityY = 0;
			animator.SetBool("jump", false);
		}

	}

	private void AimTowardMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out var hitInfo)) // , Mathf.Infinity, _aimLayerMask
		{
			var _direction = hitInfo.point - transform.position;
			_direction.y = 0f;
			_direction.Normalize();
			transform.forward = _direction;
		}
	}

	private void Jump()
	{
		if (controller.isGrounded)
		{
			float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;

		}
	}

	private float GetModifiedSmoothTime(float smoothTime)
	{
		if (controller.isGrounded)
		{
			return smoothTime;
		}

		if (airControlPercent == 0)
		{
			return float.MaxValue;
		}
		return smoothTime / airControlPercent;
	}


	public void InteractWithItem()
	{
		print("public void InteractWithItem()");
		if (mInteractItem != null)
		{
			mInteractItem.OnInteract();

			if (mInteractItem is InventoryItemBase)
			{
				InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;
				inventory.AddItem(inventoryItem);
				inventoryItem.OnPickup();

				if (inventoryItem.UseItemAfterPickup)
				{
					inventory.UseItem(inventoryItem);
					//inventoryItem.OnUse();


				}
				Hud.CloseMessagePanel();
				mInteractItem = null;
			}
			//else
			//{
			//    if (mInteractItem.ContinueInteract())
			//    {
			//        Hud.OpenMessagePanel(mInteractItem);
			//    }
			//    else
			//    {
			//        Hud.CloseMessagePanel();
			//        mInteractItem = null;
			//    }
			//}
		}
	}

	private InteractableItemBase mInteractItem = null;


	private void OnTriggerEnter(Collider other)
	{
		TryInteraction(other);
	}

	private void TryInteraction(Collider other)
	{
		InteractableItemBase item = other.GetComponent<InteractableItemBase>();

		if (item != null)
		{
			if (item.CanInteract(other))
			{
				mInteractItem = item;

				Hud.OpenMessagePanel(mInteractItem);
			}
		}

		/*
		if (item != null)
		{
			if (mLockPickup)
				return;
			mItemToPickup = item;
			//inventory.AddItem(item);
			//item.OnPickup();
			Hud.OpenMessagePanel("");
		}
		*/
	}

	private void OnTriggerExit(Collider other)
	{
		InventoryItemBase item = other.GetComponent<InventoryItemBase>();
		if (item != null)
		{
			Hud.CloseMessagePanel();
			mInteractItem = null;
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		/*
		if (mLockPickup)
			return;
		IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
		if (item != null)
		{
			inventory.AddItem(item);
		}
		*/
	}
}





//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using UnityEngine;
//using UnityScript.Steps;

//public class PlayerController : MonoBehaviour
//{

//    private Animator animator;
//    private Rigidbody rigidbody;

//    public float speed = 2.5f;

//    public float RotationSpeed = 350.0f;

//    private float gravity = 1.0f;
//    private float verticalVelocity;

//    private Vector3 _moveDir = Vector3.zero;

//    public Inventory inventory;

//    public float jumpForce = 10.0f;

//    public float groundDistance = 0.3f;
//    public LayerMask whatIsGround;

//    public CharacterController controller;

//    public float turnSmoothTime = 0.1f;
//    float turnSmoothVelocity;
//    public Transform cam;

//    private Vector3 velocity;

//    public float jumpHight = 2f;

//    void Start()
//    {
//        animator = GetComponent<Animator>();
//        rigidbody = GetComponent<Rigidbody>();
//    }

//    //// Start is called before the first frame update
//    //void Start()
//    //{
//    //    _animator = GetComponent<Animator>();
//    //    _characterController = GetComponent<CharacterController>();
//    //}

//    // Update is called once per frame
//    void Update()
//    {
//        //    float h = Input.GetAxis("Horizontal");
//        //    float v = Input.GetAxis("Vertical");
//        //    if (v < 0) v = 0;
//        //    print(v);
//        //    transform.Rotate(0, h * RotationSpeed * Time.deltaTime, 0);

//        //    _moveDir.y -= Gravity * Time.deltaTime;
//        //    _characterController.Move(_moveDir * Time.deltaTime);
//        //    if (_characterController.isGrounded)
//        //    {
//        //        bool move = (v > 0) || (h != 0);
//        //        _animator.SetBool("run", move);
//        //        if (v > 0)
//        //            _moveDir = Vector3.forward * v;
//        //        else
//        //            _moveDir = Vector3.back * -v;
//        //        _moveDir = transform.TransformDirection(_moveDir);
//        //        _moveDir *= Speed;
//        //    }
//        //}


//        float h = Input.GetAxis("Horizontal");
//        float v = Input.GetAxis("Vertical");
//        //bool move = (v != 0) || (h != 0);
//        //animator.SetBool("move", move);
//        Vector3 direction = new Vector3(h, 0f, v).normalized;
//        print(velocity);


//        if (!controller.isGrounded) {
//            velocity.y -= gravity * Time.deltaTime;
//            controller.Move(velocity * Time.deltaTime);
//        }




//        if (Input.GetButtonDown("Jump") && controller.isGrounded)
//        {
//            //rigidbody.AddForce(Vector3.up * 500);
//            velocity.y += gravity * Time.deltaTime; //Mathf.Sqrt(0.1f * -2 * gravity);
//            animator.SetTrigger("jump");
//        }




//        if (controller.isGrounded && velocity.y > 0)
//        {
//            print(" -2 velocity.y");
//            velocity.y = -2f;
//        }





//        if (controller.isGrounded)
//        {

//            if (direction.magnitude >= 0.1f)
//            {

//                if (Input.GetKey(KeyCode.LeftShift))
//                {
//                    speed = 5;
//                    animator.SetFloat("speed", 2f, 0.1f, Time.deltaTime);
//                }
//                else
//                {
//                    animator.SetFloat("speed", 1f, 0.1f, Time.deltaTime);
//                    speed = 2;
//                }

//                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
//                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
//                transform.rotation = Quaternion.Euler(0f, angle, 0f);
//                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

//                controller.Move(moveDir.normalized * speed * Time.deltaTime);
//            }
//            else
//            {
//                animator.SetFloat("speed", 0f, 0.1f, Time.deltaTime);
//            }
//        }

//    }


//    private void Run()
//    {

//    }

//    private void OnControllerColliderHit(ControllerColliderHit hit)
//    {
//        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
//        if (item != null)
//        {
//            inventory.AddItem(item);
//        }
//    }
//}