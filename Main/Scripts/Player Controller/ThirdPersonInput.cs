using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonInput : MonoBehaviour
{
    #region Variables       

    [Header("Controller Input")]
    public string horizontalInput = "Horizontal";
    public string verticallInput = "Vertical";
    public KeyCode jumpInput = KeyCode.Space;
    public KeyCode strafeInput = KeyCode.Tab;
    public KeyCode sprintInput = KeyCode.LeftShift;

    [Header("Camera Input")]
    public string rotateCameraXInput = "Mouse X";
    public string rotateCameraYInput = "Mouse Y";

    [HideInInspector] public ThirdPersonController cc;
    [HideInInspector] public ThirdPersonCamera tpCamera;
    [HideInInspector] public Camera cameraMain;

    [HideInInspector] public Health health;
    [HideInInspector] public Utility utility;

    [HideInInspector] public ItemInterract itemInterract;
    public Animator animator;
    public bool AimDebug = true;

    #endregion

    protected virtual void Start()
    {
        InitilizeController();
        InitializeTpCamera();
        InitilizeHealth();
        InitilizeItemInterract();

    }

    protected virtual void FixedUpdate()
    {
        cc.UpdateMotor();               // updates the ThirdPersonMotor methods
        cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
        cc.ControlRotationType();       // handle the controller rotation type
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
    protected virtual void Update()
    {
        if (!health.IsDead && mIsControlEnabled)
        {

            InputHandle();                  // update the input methods


            CheckForArmed();
            HeadRotate();

            cc.UpdateAnimator();            // updates the Animator Parameters
        }
    }

    public virtual void OnAnimatorMove()
    {
        cc.ControlAnimatorRootMotion(); // handle root motion animations 
    }

    #region Basic Locomotion Inputs

    protected virtual void InitilizeController()
    {
        cc = GetComponent<ThirdPersonController>();

        if (cc != null)
            cc.Init();
    }

    protected virtual void InitilizeHealth()
    {
        health = GetComponent<Health>();

        if (health != null)
            health.Init();
    }

    protected virtual void InitilizeItemInterract()
    {
        itemInterract = GetComponent<ItemInterract>();
        itemInterract.Init();
    }




    protected virtual void InitializeTpCamera()
    {
        if (tpCamera == null)
        {
            tpCamera = FindObjectOfType<ThirdPersonCamera>();
            if (tpCamera == null)
                return;
            if (tpCamera)
            {
                tpCamera.SetMainTarget(this.transform);
                tpCamera.Init();
            }
        }
    }

    protected virtual void InputHandle()
    {
        MoveInput();
        CameraInput();
        SprintInput();
        StrafeInput();
        JumpInput();
        AttackInput();
        AimInput();
        SitInput();
        InterractInput();


    }


    private bool isArmed = false;

    public virtual InventoryItemBase GetCurrentItemName()
    {
        return itemInterract.mCurrentItem;
    }

    public virtual void CheckForArmed()
    {
        
        if (itemInterract.mCurrentItem == null)
        {
            animator.SetInteger("WeaponIndex", 0);
        }
        else if (itemInterract.mCurrentItem.Name == "AK74")
        {
            animator.SetInteger("WeaponIndex", 1);
        }


        //InventoryItemBase currentItem = GetCurrentItemName();
        //if (currentItem.ItemType == EItemType.) {
        //    print("EItemType.Weapon");
        //    isArmed = true;
        //    if (currentItem.Name == "AK47")
        //        animator.SetInteger("WeaponIndex", 1);
        //}
    }

    public virtual void MoveInput()
    {
        cc.input.x = Input.GetAxis(horizontalInput);
        cc.input.z = Input.GetAxis(verticallInput);
    }

    protected virtual void CameraInput()
    {
        if (!cameraMain)
        {
            if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
            else
            {
                cameraMain = Camera.main;
                cc.rotateTarget = cameraMain.transform;
            }
        }

        if (cameraMain)
        {
            cc.UpdateMoveDirection(cameraMain.transform);
        }

        if (tpCamera == null)
            return;

        var Y = Input.GetAxis(rotateCameraYInput);
        var X = Input.GetAxis(rotateCameraXInput);

        tpCamera.RotateCamera(X, Y);
    }

    protected virtual void StrafeInput()
    {
        if (Input.GetKeyDown(strafeInput))
            cc.Strafe();
        if (Input.GetMouseButtonDown(1))
        {
            cc.Strafe();
        }
        if (Input.GetMouseButtonUp(1))
        {
            cc.Strafe();
        }
    }

    protected virtual void SprintInput()
    {
        if (Input.GetKeyDown(sprintInput))
            cc.Sprint(true);
        else if (Input.GetKeyUp(sprintInput))
            cc.Sprint(false);
    }

    /// <summary>
    /// Conditions to trigger the Jump animation & behavior
    /// </summary>
    /// <returns></returns>
    protected virtual bool JumpConditions()
    {
        return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
    }

    /// <summary>
    /// Input to trigger the Jump 
    /// </summary>
    protected virtual void JumpInput()
    {
        if (Input.GetKeyDown(jumpInput) && JumpConditions())
            cc.Jump();
    }

    protected virtual void AttackInput()
    {
        if (Input.GetMouseButtonDown(0))
            cc.Attack(true, cameraMain);
        else if (Input.GetMouseButtonUp(0))
            cc.Attack(false, cameraMain);
    }

    protected virtual void AimInput()
    {
        if (AimDebug == true)
        {
            cc.Aim(true, cameraMain);
        }
        else { 
            if (Input.GetMouseButtonDown(1))
                cc.Aim(true, cameraMain);
            else if (Input.GetMouseButtonUp(1))
                cc.Aim(false, cameraMain);
        }
    }

    protected virtual void SitInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
            cc.Sit();
    }


    protected virtual void HeadRotate()
    {
        cc.RotateToCameraDirection(cameraMain);
           
    }


    protected virtual void InterractInput()
    {
        
        if (itemInterract.mInteractItem != null && Input.GetKeyDown(KeyCode.F))
        { 
            itemInterract.mInteractItem.OnInteractAnimation(animator);
            itemInterract.InteractWithItem();

            /*
            inventory.AddItem(mItemToPickup);
            //mItemToPickup.OnPickup();
            animator.SetTrigger("drop_tr");
            Hud.CloseMessagePanel();
            */
        }
    }




    #endregion
}
