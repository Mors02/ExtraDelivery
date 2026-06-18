using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;


public class Vespa : MonoBehaviour
{
    [SerializeField]
    private Transform _attachPoint;
    [SerializeField]
    InputActionAsset _inputSystem;
    
    [SerializeField]
    private GameObject _player;

    private PlayerInput _pi;
    InputAction _interact;
    private bool _near;

    private bool _isRiding;

    [SerializeField]
    private Rigidbody _rb;
    private Rigidbody _playerRb;
    private Animator _animator;

    [Header("Dismount")]
    [SerializeField]
    private float _verticalOffForce = 10f;
    [SerializeField]
    private float _momentumMultiplier = 10f;
    [SerializeField]
    private float _maxMagnitude = 10f;

    private Vector3 _lastVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _interact = _inputSystem.FindActionMap("Player").FindAction("Interact");
        _pi = _player.GetComponent<PlayerInput>();
        _playerRb = _player.GetComponent<Rigidbody>();
        _animator = _player.GetComponent<Animator>();
        _interact.Enable();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           // Debug.Log("Entered");
            //_interact.performed += RideVespa;
            //_pi.actions["Interact"].performed += RideVespa;
            //_player = other.gameObject;
            _near = true;
        }
    }

    private void OnTriggerExit(Collider other) {
    
        if (other.gameObject.CompareTag("Player"))
        {
            //_interact.performed -= RideVespa;
            //_pi.actions["Interact"].performed -= RideVespa;
            _near = false;
        }
    }


    public void RideVespa(InputAction.CallbackContext context)
    {
        Debug.Log("Entered " + _isRiding);
        if (_near && context.started)
        {
            //get in
            if (!_isRiding)
            {
                //reset the velocity
                _rb.linearVelocity = Vector3.zero;

                _player.transform.rotation = Quaternion.LookRotation(this.transform.forward);
                //disable the auto height component
                _player.GetComponent<RideHeight>().enabled = false;
                _player.GetComponent<CharacterController>().enabled = false;
                _playerRb.isKinematic = true;
                GetComponent<CharacterController>().enabled = true;
                //set the position to the attach point
                _player.transform.position = _attachPoint.position;
                //change parent so they move together
                _player.transform.parent = this.transform;
                //send the animation trigger
                _animator.ResetTrigger("OffScooter");
                _animator.SetTrigger("OnScooter");
                _isRiding = true;
            } else
            {
               Dismount(false);
            }
            
        }
    }

    private void Dismount(bool fromImpact)
    {
         //enable the scripts
        _player.GetComponent<RideHeight>().enabled = true;
        _player.GetComponent<CharacterController>().enabled = true;
        _playerRb.isKinematic = false;
        GetComponent<CharacterController>().enabled = false;

        //remove the parenting
        _player.transform.parent = null;
        _animator.SetTrigger("OffScooter");
        _isRiding = false;

        
        //little jump when getting out
        _playerRb.AddForce(Vector3.up * (fromImpact? _verticalOffForce * 1.5f : _verticalOffForce), ForceMode.Impulse);
        //keep the momentum if we are going fast
        _playerRb.AddForce(_lastVelocity * (fromImpact? _momentumMultiplier * 1.5f : _momentumMultiplier), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        //Vector3 contactPoint = collision.contacts[0].point;
        //Vector3 contactDirection = contactPoint - this.transform.position;
        
        //float parallel = Vector3.Dot(contactDirection, this.transform.forward);
        
        //if we collide at over a certain magnitude
        if (_lastVelocity.magnitude > _maxMagnitude)
        {
            //we dismount from a collision
            Dismount(true);
            _player.GetComponent<CharacterController>().StartFlying();
        }
    }

    private void FixedUpdate()
    {
        _lastVelocity = _rb.linearVelocity;
    }
}
