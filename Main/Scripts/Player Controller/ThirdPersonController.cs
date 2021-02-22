using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : ThirdPersonAnimator
{

    public virtual void ControlAnimatorRootMotion()
    {
        if (!this.enabled) return;

        if (inputSmooth == Vector3.zero)
        {
            transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;
        }

        if (useRootMotion)
            MoveCharacter(moveDirection);
    }

    public virtual void ControlLocomotionType()
    {
        if (lockMovement) return;

        if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
        {
            SetControllerMoveSpeed(freeSpeed);
            SetAnimatorMoveSpeed(freeSpeed);
        }
        else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
        {
            isStrafing = true;
            SetControllerMoveSpeed(strafeSpeed);
            SetAnimatorMoveSpeed(strafeSpeed);
        }

        if (!useRootMotion)
            MoveCharacter(moveDirection);
    }

    public virtual void ControlRotationType()
    {
        if (lockRotation) return;

        bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

        if (validInput)
        {
            // calculate input smooth
            inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
            //Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
            Vector3 dir = (isStrafing || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
            RotateToDirection(dir);
        }
    }

    public virtual void UpdateMoveDirection(Transform referenceTransform = null)
    {
        if (input.magnitude <= 0.01)
        {
            moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
            return;
        }

        if (referenceTransform && !rotateByWorld)
        {
            //get the right-facing direction of the referenceTransform
            var right = referenceTransform.right;
            right.y = 0;
            //get the forward direction relative to referenceTransform Right
            var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
            // determine the direction the player will face based on input and the referenceTransform's right and forward directions
            moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
        }
        else
        {
            moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
        }
    }

    public virtual void Sprint(bool value)
    {
        // var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded && !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

        var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded);

        if (value && sprintConditions)
        {
            if (input.sqrMagnitude > 0.1f)
            {
                if (isGrounded && useContinuousSprint)
                {
                    isSprinting = !isSprinting;
                }
                else if (!isSprinting)
                {
                    isSprinting = true;
                }
            }
            else if (!useContinuousSprint && isSprinting)
            {
                isSprinting = false;
            }
        }
        else if (isSprinting)
        {

            isSprinting = false;
        }
    }

    public virtual void Strafe()
    {
        isStrafing = !isStrafing;
    }

    public virtual void Jump()
    {
        // trigger jump behaviour
        jumpCounter = jumpTimer;
        isJumping = true;

        /*
        // trigger jump animations
        if (input.sqrMagnitude < 0.1f)
            animator.CrossFadeInFixedTime("JumpIdleUp", 100f);
        else
            animator.CrossFadeInFixedTime("JumpWalkUp", .2f);
        */
    }

    public virtual void Attack(bool condition, Camera cameraMain)
    {
        isAttacking = condition;
    }

    public virtual void Aim(bool condition, Camera cameraMain)
    {
        isAiming = condition;
    }

    public virtual void Sit()
    {
        isSitting = !isSitting;
    }



    public void RotateToCameraDirection(Camera cameraMain)
    {
        var playerVectorHor = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        var cameraVectorHor = new Vector3(cameraMain.transform.forward.x, 0.0f, cameraMain.transform.forward.z);
        horDiffAngle = Vector3.SignedAngle(playerVectorHor, cameraVectorHor, Vector3.up);

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
        verDiffAngle = Utility.WrapAngle(cameraMain.transform.eulerAngles.x);

    }




}
