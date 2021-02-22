using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Vector3 lastPos;
    public GameObject decal;
    public bool test;
    // Update is called once per frame
    void Start()
    {
        lastPos = transform.position;
        Invoke("DeleteBullet", 3.0f);
    }
    private void Update()
    {
        RaycastHit hit;
        if(Physics.Linecast(lastPos, transform.position, out hit))
        {
            GameObject d = Instantiate<GameObject>(decal);
            d.transform.position = hit.point + hit.normal * 0.001f;
            d.transform.rotation = Quaternion.LookRotation(-hit.normal);
            Destroy(d, 5);
            DeleteBullet();
        }
    }

    private void DeleteBullet()
    {
        Destroy(gameObject);
    }
}
