using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{

    private bool _isCausingDamage = false;

    public float DamageRepeatRate = 0.1f;
    public int DamageAmount = 1;
    public bool Repeating = true;

    private void OnTriggerEnter(Collider other)
    {
        _isCausingDamage = true;
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if(player != null)
        {
            if (Repeating)
            {
                StartCoroutine(TakeDamage(player, DamageRepeatRate));
            }
        }
    }

    IEnumerator TakeDamage(PlayerController player, float repeatRate)
    {
        while (_isCausingDamage)
        {
            player.TakeDamage(DamageAmount);
            TakeDamage(player, repeatRate);

            if (player.IsDead)
            {
                _isCausingDamage = false;
            }

            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _isCausingDamage = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
