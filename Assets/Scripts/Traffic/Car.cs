using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Car : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private NavMeshAgent _agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        this._agent.destination = _target.position;
    }

    public void Update()
    {
        this._agent.destination = _target.position;

        if (_agent.isOnOffMeshLink)
        {
            OffMeshLinkData data = _agent.currentOffMeshLinkData;

            //calculate the final point of the link
            Vector3 endPos = data.endPos + Vector3.up * _agent.baseOffset;

            //Move the agent to the end point
            _agent.transform.position = Vector3.MoveTowards(_agent.transform.position, endPos, _agent.speed * Time.deltaTime);

            //when the agent reach the end point you should tell it, and the agent will "exit" the link and work normally after that
            if (_agent.transform.position == endPos)
            {
                _agent.CompleteOffMeshLink();
            }
        }
    }
}
