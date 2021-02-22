using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public Animator animator;
    public ThirdPersonInput thirdPersonInput;
    public ThirdPersonMotor thirdPersonMotor;
    public ItemInterract itemInterract;
    public Inventory inventory;
    public Transform targetLook;

    public Transform l_Hand;
    public Transform r_Hand;

    public Transform l_Hand_Target;
    public Transform r_Hand_Target;

    public Quaternion lh_rot;
    public Quaternion rh_rot;

    public float rh_Weight;

    public Transform shoulder;
    public Transform aimPivot;

    public GameObject rightHandObject;

    public bool WeaponConfigDebug = false;

    // Start is called before the first frame update
    void Start()
    {
        shoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
        thirdPersonInput = GetComponent<ThirdPersonInput>();
        thirdPersonMotor = GetComponent<ThirdPersonMotor>();
        itemInterract = GetComponent<ItemInterract>();

        aimPivot = new GameObject().transform;
        aimPivot.name = "AimPivot";
        aimPivot.transform.parent = transform;

        r_Hand= new GameObject().transform;
        r_Hand.name = "RightHand";
        r_Hand.transform.parent = aimPivot;

        l_Hand = new GameObject().transform;
        l_Hand.name = "LeftHand";
        l_Hand.transform.parent = aimPivot;

        




    }

    // Update is called once per frame
    void Update()
    {
        if (itemInterract.mCurrentItem != null) {
            var currentItem = rightHandObject.transform.Find(itemInterract.mCurrentItem.Name);

            var AK74Config = currentItem.GetComponent<AK74>();
            
            //Quaternion rotRight = Quaternion.Euler(AK74Config.weaponConfig.rHandPos.x, AK74Config.weaponConfig.rHandPos.y, AK74Config.weaponConfig.rHandPos.z);
            //r_Hand.localRotation = rotRight;



            lh_rot = l_Hand_Target.rotation;
            l_Hand.position = l_Hand_Target.position;


            //if (thirdPersonMotor.isAiming)
            //{
            //    rh_Weight += Time.deltaTime;
            //}
            //else
            //{
            //    rh_Weight -= Time.deltaTime;
            //}
            //rh_Weight = Mathf.Clamp(rh_Weight, 0, 1);

            //rh_rot = r_Hand_Target.rotation;
            //r_Hand.position = r_Hand_Target.position;

            //l_Hand.rotation = lh_rot;

            if (WeaponConfigDebug == false) { 
                r_Hand.localPosition = AK74Config.weaponConfig.rHandPos;
                r_Hand.localRotation = Quaternion.Euler(AK74Config.weaponConfig.rHandRot.x, AK74Config.weaponConfig.rHandRot.y, AK74Config.weaponConfig.rHandRot.z);
            }
        }
    }

    void OnAnimatorIK()
    {
        aimPivot.position = shoulder.position;
        if (itemInterract.mCurrentItem != null)
        {
            
            if (thirdPersonMotor.isAiming)
            {
                rh_Weight = 1;
                aimPivot.LookAt(targetLook);
                animator.SetLookAtWeight(1f, 0.3f, 1f);
                animator.SetLookAtPosition(targetLook.position);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, l_Hand.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, lh_rot);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rh_Weight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rh_Weight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, r_Hand.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, r_Hand.rotation);

            }
            else
            {
                rh_Weight = 0;
                animator.SetLookAtWeight(1.0f, 0.3f, 1.0f);
                animator.SetLookAtPosition(targetLook.position);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, l_Hand.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, lh_rot);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rh_Weight);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rh_Weight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, r_Hand.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, r_Hand.rotation);


            }
        }
        else
        {
            if (thirdPersonMotor.isAiming)
            {
                rh_Weight = 1;
                aimPivot.LookAt(targetLook);
                animator.SetLookAtWeight(1f, 1f, 1f);
                animator.SetLookAtPosition(targetLook.position);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);


                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
            else
            {
                rh_Weight = 1;
                aimPivot.LookAt(targetLook);
                animator.SetLookAtWeight(1f, 1f, 1f);
                animator.SetLookAtPosition(targetLook.position);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);


                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }

        }
    }
}
