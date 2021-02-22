using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponConfig weaponConfig;
    public Transform shotPoint;
    public Transform targetLook;
    public Transform attackPoint;

    public GameObject cameraMain;
    public GameObject decal;
    public Camera _camera;
    public Transform crossHairManager;

    // Update is called once per frame
    void Update()
    {


        Ray ray = new Ray(attackPoint.transform.position, attackPoint.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction*1000, Color.green, 0.05f);
        RaycastHit hit;
        decal.SetActive(false);


        //Vector3 screenSpaceCenter = new Vector3(0.5f, 0.5f, 0.5f);
        //Vector3 laserEnd = _camera.ViewportToWorldPoint(screenSpaceCenter);
        //Vector3 pos = origin;
        //Vector3 dir1 = (pos - laserEnd).normalized;
        //Vector3 laserEnd_1 = new Vector3(laserEnd.x, laserEnd.y, laserEnd.z);
        //Debug.DrawLine(pos, laserEnd_1, Color.red);

        if (Physics.Linecast(ray.origin, ray.direction, out hit))
        {
            decal.SetActive(true);
            decal.transform.position = hit.point + hit.normal * 0.01f;

            crossHairManager.localPosition = _camera.WorldToViewportPoint(transform.TransformPoint(hit.point));

            //print(hit.point + " " + transform.TransformPoint(hit.point) + " " + crossHairManager.localPosition); 
            Ray ray1 = new Ray(hit.point, ray.direction);


            decal.transform.rotation = Quaternion.LookRotation(-hit.normal);
            //targetLook.localPosition = new Vector3(0, 0, transform.InverseTransformPoint(hit.point).z)

        }
    }
}
