using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Transform camTrans;
    public Transform pivotTrans;
    public Transform charTrans;
    public Transform mTrans;
    public Transform targetLook;

    public float turnSmooth = 0.1f;
    public float pivotSpeed = 9f;
    public float Y_rot_speed = 7f;
    public float X_rot_speed = 7f;
    public float minAngle = -89f;
    public float maxAngle = 89f;
    public float normalZ = -2f;
    public float normalX = 0.4f;
    public float aimZ = -1f;
    public float aimX = 0.4f;
    public float normalY = 1.5f;


    public bool leftPivot;

    public float mouseX;
    public float mouseY;
    public float smoothX;
    public float smoothY;

    public float smoothVelocityX;
    public float smoothVelocityY;
    public float lookAngle;
    public float titleAngle;
    public bool isAiming;
    public ThirdPersonMotor thirdPersonMotor;

    private void FixedUpdate()
    {
        //RaycastHit hit;
        
        //Debug.DrawLine(camTrans.position, targetLook.TransformPoint(new Vector3(0, 0, targetLook.localPosition.z + 1)), Color.red);
        
        //if (Physics.Linecast(camTrans.position, targetLook.TransformPoint(new Vector3(0, 0, targetLook.localPosition.z + 1)), out hit))
        //{
        //    if (hit.point.z < 5) {
        //        targetLook.localPosition = new Vector3(0, 0, 5.0f);
        //    }
        //    else
        //    {
        //        targetLook.localPosition = new Vector3(0, 0, hit.point.z);
        //    }
            
        //    //targetLook.position.Set(0, 0, hit.point.z);
        //    print("hit: " + hit.point + " " + hit.normal);
        //}
    }

    private void Update()
    {
        isAiming = thirdPersonMotor.isAiming;

        

        FixedTick();
    }

    void FixedTick()
    {


        HandlePosition();
        HandleRotation();

        Vector3 targetPosition = Vector3.Lerp(mTrans.position, charTrans.position, 1);
        mTrans.position = targetPosition;
    }

    void HandlePosition()
    {
        float targetX = normalX;
        float targetY = normalY;
        float targetZ = normalZ;

        if (thirdPersonMotor.isAiming)
        {
            targetX = aimX;
            targetZ = aimZ;
        }

        if (leftPivot)
        {
            targetX = -targetX;
        }

        Vector3 newPivotPosition = pivotTrans.localPosition;
        newPivotPosition.x = targetX;
        newPivotPosition.y = targetY;

        Vector3 newCameraPosition = camTrans.localPosition;
        newCameraPosition.z = targetZ;
        float t = Time.deltaTime * pivotSpeed;
        pivotTrans.localPosition = Vector3.Lerp(pivotTrans.localPosition, newPivotPosition, t);
        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCameraPosition, t);

    }

    void HandleRotation()
    {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (turnSmooth > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothVelocityX, turnSmooth);
            smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothVelocityY, turnSmooth);
        } else
        {
            smoothX = mouseX;
            smoothY = mouseY;
        }

        lookAngle += smoothX * Y_rot_speed;
        Quaternion targetRot = Quaternion.Euler(0, lookAngle, 0);
        mTrans.rotation = targetRot;

        titleAngle -= smoothY * X_rot_speed;
        titleAngle = Mathf.Clamp(titleAngle, minAngle, maxAngle);
        pivotTrans.localRotation = Quaternion.Euler(titleAngle, 0, 0);

    }

}
