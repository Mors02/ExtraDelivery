using UnityEngine;

public class RideHeight : MonoBehaviour
{

    [SerializeField]
    LayerMask _floorMask;
    RaycastHit _rayHit;

    Rigidbody _rb;

    [Header("Rider")]
    [SerializeField]
    float _rideHeight;
    
    [SerializeField]
    float _rideSpringForceStrength, _rideSpringDamper;

    [SerializeField]
    float _rayLength = 0.5f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
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

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleRideHeight();
    }
}
