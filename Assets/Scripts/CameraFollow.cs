using System.Collections.Generic;
using System.Linq;
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

    List<BuildingTextureManager> _toRemove;
    List<BuildingTextureManager> _hitBuildings;
    List<BuildingTextureManager> _retrievedBuildings;



    private void Start()
    {
        _obstructionMask = LayerMask.GetMask("Building");
        _hitBuildings = new  List<BuildingTextureManager>();
        _retrievedBuildings = new List<BuildingTextureManager>();
        _toRemove = new List<BuildingTextureManager>();
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
            //if we hit something, get all the building around
            RaycastHit[] hits = Physics.SphereCastAll(_player.transform.position, 4f, dir.normalized, _obstructionMask);
            
            //foreach building, fade out the new buildings added
            foreach(RaycastHit currHit in hits)
            {
                BuildingTextureManager fade = currHit.collider.GetComponent<BuildingTextureManager>();  
                //start fade current building
                if (fade != null &&  !_hitBuildings.Contains(fade))
                {
                    fade.FadeOut();
                   //add it to the list of buildings faded out
                   _hitBuildings.Add(fade);
                }
                //add it to a list of all retrieved buildings this frame
                _retrievedBuildings.Add(fade);
            }

            //fade in each building no more in the sphere
            foreach(BuildingTextureManager currHit in _hitBuildings)
            {
                //if a building we previously faded out is not hit this frame
                if (!_retrievedBuildings.Contains(currHit))
                {
                    //fade in and remove it from the list of faded out buildings
                    currHit.FadeIn();
                    _toRemove.Add(currHit);
                }
            }
            //remove the buildings
            _hitBuildings = _hitBuildings.Except(_toRemove).ToList();
            
            _toRemove = new List<BuildingTextureManager>();
            //reset the list of retrieved buildings
            _retrievedBuildings = new List<BuildingTextureManager>();
        } 
        //else fade in last building
        else
        {
            //fade in all the buildings
            foreach(BuildingTextureManager currHit in _hitBuildings)
            {
              
                    currHit.FadeIn();
            }
            _hitBuildings = new List<BuildingTextureManager>();
        }
    }
}
