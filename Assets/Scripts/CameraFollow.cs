using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _moveSpeed;
    [SerializeField]
    private Vector3 _offset;
    [SerializeField]
    private float _followDistance;

    LayerMask _obstructionMask;

    BuildingTextureManager _currentBuilding;    


    private void Start()
    {
        _obstructionMask = LayerMask.GetMask("Building");
    }
    private void FixedUpdate()
    {
        Vector3 pos = Vector3.Lerp(transform.position, _player.position + _offset + -transform.forward * _followDistance, _moveSpeed * Time.fixedDeltaTime);
        transform.position = pos;
    }

    /// <summary>
    /// Check if there are buildings between the camera and peppina
    /// </summary>
    private void LateUpdate()
    {
        //get direction
        Vector3 dir = transform.position - _player.position;

        //Raycast from camera to peppina
        if (Physics.Raycast(_player.position, dir.normalized, out RaycastHit hit, dir.magnitude, _obstructionMask))
        {
            //if we hit something
            BuildingTextureManager fade = hit.collider.GetComponent<BuildingTextureManager>();

            //start fade current building
            if (fade != null && fade != _currentBuilding)
            {
                //if we were behind another building then fade in the other building
                if (_currentBuilding != null)
                {
                    _currentBuilding.FadeIn();
                }

                fade.FadeOut();
                _currentBuilding = fade;
            }
        } 
        //else fade in last building
        else
        {
            if (_currentBuilding != null)
            {
                _currentBuilding.FadeIn();
                _currentBuilding = null;    
            }
        }
    }
}
