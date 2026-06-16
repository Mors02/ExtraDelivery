using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    LayerMask _floorMask;

    [SerializeField]
    Animator _animator;

    RaycastHit _rayHit;

    Rigidbody _rb;

    [Header("Rider")]
    [SerializeField]
    float _rideHeight;
    
    [SerializeField]
    float _rideSpringForceStrength, _rideSpringDamper;

    [SerializeField]
    float _rayLength = 0.5f;

    [Header("Movement")]
    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _acceleration;

    private Vector3 _currentMovement;

    //[SerializeField]
    //private AnimationCurve _accelerationFromDot;
    //[SerializeField]
    //private AnimationCurve _maxAccelerationForceFactorFromDot;
    [SerializeField]
    private float _maxAcceleration;
    [SerializeField]
    private Vector3 _forceScale;

    [SerializeField]
    private ParticleSystem _footstepDust;

    private bool _wasWalking;

    private Vector2 _move;

    [Header("Jump")]
    [SerializeField]
    private float _jumpSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       HandleRideHeight();
       HandleMove();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (Vector3.down * _rayLength));
    }

    void HandleRideHeight()
    {
         bool _rayDidHit = Physics.Raycast(this.transform.position, Vector3.down, out _rayHit, _rayLength, _floorMask);

        if (_rayDidHit)
        {
            Vector3 vel = _rb.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(new UnityEngine.Vector3(0, -1, 0));

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = _rayHit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = _rayHit.distance - _rideHeight;
            float springForce = (x * _rideSpringForceStrength) - (relVel * _rideSpringDamper);

            _rb.AddForce(rayDir * springForce);

            if (hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, _rayHit.point);
            }

        }
    }

    private void HandleMove()
    {
        /*Vector3 movement = new Vector3(_move.x, 0f, _move.y);
        if (movement.magnitude != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        
        _rb.AddForce(movement * Time.fixedDeltaTime * _speed, ForceMode.Force);*/

        //calculate the new goal velocity
        Vector3 unitVel = _currentMovement.normalized;
        //calculate dot velocity based on current movement
        float velDot = Vector3.Dot(new Vector3(_move.x, 0f, _move.y), unitVel);
        //and evaluate current acceleration
        float accel = _acceleration; // * _accelerationFromDot.Evaluate(velDot);

        //check what is the ideal movement and rotate to match the camera
        Vector3 movement = Quaternion.Euler(0, 45f, 0) * new Vector3(_move.x, 0f, _move.y) * _speed;
        //checks the ideal direciton and speed and moves it towards it
        _currentMovement = Vector3.MoveTowards(_currentMovement, movement, accel * Time.fixedDeltaTime);

        //retrieve the acceleration needed to reach the desidered movement
        Vector3 neededAccel = (_currentMovement - _rb.linearVelocity) / Time.fixedDeltaTime;

        float maxAccel = _maxAcceleration; // * _maxAccelerationForceFactorFromDot.Evaluate(velDot);

        //if we are moving rotate peppina
        if (movement.magnitude != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);

        //clamp the acceleration to the max
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, _forceScale));

    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();

        bool isWalking = _move.magnitude != 0;

        //check if we are walking and start or stop the particole system
        _animator.SetBool("Walking", isWalking);
        
        /*//started walking
        if (isWalking && !_wasWalking)
        {
            Debug.Log("WALKING");
            var em = _footstepDust.emission;
            em.enabled = true;  
        }
        
        //stopped walking
        if (!isWalking && _wasWalking)
        {
            var em = _footstepDust.emission;
            em.enabled = false;
        }

        _wasWalking = isWalking;*/

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _rb.AddForce(Vector3.up * _jumpSpeed, ForceMode.Impulse);
    }

}
