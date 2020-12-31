using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityScript.Steps;

public class PlayerController : MonoBehaviour
{

    private Animator _animator;

    private CharacterController _characterController;

    public float Speed = 2.5f;

    public float RotationSpeed = 350.0f;

    private float Gravity = 20.0f;

    private Vector3 _moveDir = Vector3.zero;

    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (v < 0) v = 0;
        transform.Rotate(0, h * RotationSpeed * Time.deltaTime, 0);
        
        _moveDir.y -= Gravity * Time.deltaTime;
        _characterController.Move(_moveDir * Time.deltaTime);
        if (_characterController.isGrounded)
        {
            bool move = (v > 0) || (h != 0);
            _animator.SetBool("run", move);
            _moveDir = Vector3.forward * v;
            _moveDir = transform.TransformDirection(_moveDir);
            _moveDir *= Speed;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
        if (item != null)
        {
            inventory.AddItem(item);
        }
    }
}
