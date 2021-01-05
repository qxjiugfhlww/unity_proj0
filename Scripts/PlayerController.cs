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

	private HealthBar mHealthBar;

	

	void Start()
	{
		animator = GetComponent<Animator>();
		//cameraT = Camera.main.transform;
		controller = GetComponent<CharacterController>();
        inventory.ItemUsed += Inventory_ItemUsed;
		inventory.ItemRemoved += Inventory_ItemRemoved;

		mHealthBar = Hud.transform.Find("HealthBar").GetComponent<HealthBar>();
		mHealthBar.Min = 0;
		mHealthBar.Max = Health;
	}

	public int Health = 100;

	public bool IsDead
    {
		get
        {
			return Health == 0;
        }
    }

	public void TakeDamage(int amount)
    {
		Health -= amount;
		if (Health < 0)
        {
			Health = 0;
        }
		mHealthBar.SetHealth(Health);

		if (IsDead)
        {
			animator.SetTrigger("die_tr");
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


			// input
			Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			Vector2 inputDir = input.normalized;
			bool running = Input.GetKey(KeyCode.LeftShift);

			Move(inputDir, running);

			if (Input.GetKeyDown(KeyCode.Space))
			{
				animator.SetBool("jump", true);
				Jump();
			
			
			}
			// animator
			float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
			animator.SetFloat("speed", animationSpeedPercent, speedSmoothTime, Time.deltaTime);

		}
	}

	private void Move(Vector2 inputDir, bool running)
	{
		
		
		if (inputDir != Vector2.zero)
		{
			float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}

		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;
		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

		controller.Move(velocity * Time.deltaTime);
		currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

		if (controller.isGrounded)
		{
			
			velocityY = 0;
			animator.SetBool("jump", false);
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