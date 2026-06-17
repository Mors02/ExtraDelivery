using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    LayerMask _floorMask;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    private bool _tiltOnTurn;
    [SerializeField]
    private float _tilAngle, _tiltSpeed;

    RaycastHit _rayHit;

    Rigidbody _rb;

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
       HandleMove();
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

        if (_tiltOnTurn)
        {
            float parallel = Vector3.Dot(movement, transform.forward) / _speed;
            //float angle = Vector3.Angle(movement, transform.forward);
           
            // cross product of the movement from the forward
            float num = (movement.x * transform.forward.z) - (movement.z * transform.forward.x);
            
            // dot product of the movement from the forward
            float den = (movement.x * transform.forward.x) + (movement.z * transform.forward.z);

            // Value in radians
            float angleInRadians = Mathf.Atan2(num, den);

            // Converts from -180 to 180 (negative values are left, positive are right)
            float angle =  angleInRadians * Mathf.Rad2Deg;

            //Debug.Log(parallel + ": " + (Mathf.Approximately(parallel, 1f)? "Parellel" : "Not parallel"));

            float clampedAngle = Mathf.Clamp(angle, -_tilAngle, _tilAngle);

            Vector3 euler = transform.localEulerAngles;
            //tilt by how percentage the tilt angle is compared to the max tilt
            Debug.Log(_tiltSpeed * Mathf.Abs(clampedAngle) / _tilAngle);
            euler.z = Mathf.LerpAngle(euler.z, clampedAngle, _tiltSpeed * Mathf.Abs(clampedAngle) / _tilAngle);
            transform.localEulerAngles = euler;
        }
            


        //clamp the acceleration to the max
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);
        _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, _forceScale));

    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();

        bool isWalking = _move.magnitude != 0;

        //check if we are walking and start or stop the particole system
        if (_animator != null)
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
