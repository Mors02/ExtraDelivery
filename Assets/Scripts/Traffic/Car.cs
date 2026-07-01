using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private bool _showDebug;

    private NavMeshAgent _agent;
    [SerializeField]
    private float _turnSpeed;

    private Vector3 _currentDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        this._agent.destination = _target.position;
        _agent.updateRotation = false;
        _agent.updatePosition = true;
        //_agent.updatePosition = false;
    }

    public void FixedUpdate()
    {
        this._agent.destination = _target.position;

        

        //Vector3 nextDirection = Vector3.Lerp((endPos-transform.position).normalized, transform.rotation.ToAngleAxis, _turnSpeed);
        Vector3 nextDirection = (_agent.steeringTarget - _agent.transform.position).normalized;
        
        //save direction if it's greater than 0
        if (nextDirection.sqrMagnitude > 0.01f)
            _currentDirection = nextDirection;


        if (_showDebug) Debug.Log(_agent.velocity);
        //Vector3 nextDirection = _agent.velocity.normalized;
        if (nextDirection != Vector3.zero)
        {   
            //to get the shortest rotation, look at the dot to see if we can actually invert it
            Quaternion end = Quaternion.LookRotation(_currentDirection);
            if (_showDebug) Debug.Log(gameObject.name + " " + Quaternion.Dot(_agent.transform.rotation, end));
            if (Quaternion.Dot(_agent.transform.rotation, end) < 0.0f)
            {
                end = new Quaternion(-end.x, -end.y, -end.z, -end.w);
            }
            //float angle = Mathf.Atan2(nextDirection.x, nextDirection.y) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.AngleAxis(angle, -transform.forward);
            _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, end, _turnSpeed * Time.fixedDeltaTime);   
        }

        if (_agent.isOnOffMeshLink)
        {
            _agent.updatePosition = false;
            
            OffMeshLinkData data = _agent.currentOffMeshLinkData;

            //calculate the final point of the link
            Vector3 endPos = data.endPos + Vector3.up * _agent.baseOffset;

            //Move the agent to the end point
            _agent.transform.position = Vector3.MoveTowards(_agent.transform.position, endPos, _agent.speed * Time.fixedDeltaTime);

            /*
            //nextDirection = (_agent.steeringTarget-_agent.transform.position).normalized;
                    //save direction if it's greater than 0
            if (nextDirection.sqrMagnitude > 0.01f)
                _currentDirection = nextDirection;
                
            if (nextDirection != Vector3.zero)
            {
                //to get the shortest rotation, look at the dot to see if we can actually invert it
                Quaternion end = Quaternion.LookRotation(_currentDirection);
                if (Quaternion.Dot(_agent.transform.rotation, end) < 0.0f)
                {
                    end = new Quaternion(-end.x, -end.y, -end.z, -end.w);
                }
                //float angle = Mathf.Atan2(nextDirection.x, nextDirection.y) * Mathf.Rad2Deg;
                //transform.rotation = Quaternion.AngleAxis(angle, -transform.forward);
                _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, end, _turnSpeed * Time.fixedDeltaTime);   
            }
            */
            //when the agent reach the end point you should tell it, and the agent will "exit" the link and work normally after that
            if (Vector3.Distance(_agent.transform.position, endPos) < 0.5f)
            {
                if (_showDebug) Debug.Log("Exited");
                _agent.velocity = _currentDirection * _agent.speed;
                _agent.CompleteOffMeshLink();
                _agent.nextPosition = endPos;
                _agent.updatePosition = true;
                //_agent.updateRotation = true;
                
                
            }
        }
    }
}
