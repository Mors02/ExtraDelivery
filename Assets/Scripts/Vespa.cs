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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _interact = _inputSystem.FindActionMap("Player").FindAction("Interact");
        _pi = _player.GetComponent<PlayerInput>();
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
        if (_near)
        {
            Debug.Log("Performed");

            _player.transform.rotation = Quaternion.LookRotation(this.transform.forward);

            //disable the auto height component
            _player.GetComponent<RideHeight>().enabled = false;
            _player.GetComponent<CharacterController>().enabled = false;
            _player.GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<CharacterController>().enabled = true;
            //set the position to the attach point
            _player.transform.position = _attachPoint.position;
            //change parent so they move together
            _player.transform.parent = this.transform;
            //send the animation trigger
            _player.GetComponent<Animator>().SetTrigger("OnScooter");
            _isRiding = true;
        }
    }

    private void FixedUpdate()
    {
        //if (_isRiding)
        //    this.transform.position = _attachPoint.position;
    }
}
