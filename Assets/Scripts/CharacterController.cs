using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    LayerMask _floorMask;

    RaycastHit _rayHit;

    Rigidbody _rb;

    [SerializeField]
    float _rideHeight, _rideSpringForceStrength, _rideSpringDamper;

    [SerializeField]
    float _rayLength = 0.5f;

    [SerializeField]
    private float _speed;

    private Vector2 _move;
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
        Vector3 movement = new Vector3(_move.x, 0f, _move.y);
        if (movement.magnitude != 0)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15f);
        
        _rb.AddForce(movement * Time.fixedDeltaTime * _speed, ForceMode.Force);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
        if (_move.magnitude == 0)
            _rb.linearVelocity = Vector3.zero;
    }
}
