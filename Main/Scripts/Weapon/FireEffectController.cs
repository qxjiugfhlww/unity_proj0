using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FireEffectController : MonoBehaviour
{

    public List<ParticleSystem> particleSystem = new List<ParticleSystem>();


    private void Start()
    {

        // particleSystem.Add(new Part() { PartName = "crank arm", PartId = 1234 });

        //Destroy(gameObject);
        //Instantiate(gameObject);
    }
    void Update()
    {
        //ExecuteNitroEffect();
    }

    private IEnumerable ExecuteFireEffect()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            print("Effects On");
            Instantiate(gameObject);
            yield return new WaitForSeconds(1);
            Destroy(gameObject);
            print("Effects Off");
        }

    }
}
