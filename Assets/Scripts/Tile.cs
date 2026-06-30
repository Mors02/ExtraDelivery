using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
   public Tile[] TilesBlacklist;

   public SideConnection Up;
   public SideConnection Down;
   public SideConnection Left;
   public SideConnection Right;

   [SerializeField]
   private Transform _connectionPointsParent;

   public List<RoadConnectPoint> RoadConnectionsPoints;

   public int Value;

   public TileType Type;

   public void Start()
   {
      if (RoadConnectionsPoints == null)
         RoadConnectionsPoints = new List<RoadConnectPoint>();

      if (Type == TileType.Road)
      {
         foreach (Transform child in _connectionPointsParent)
         {
            if (child.gameObject.activeInHierarchy)
               RoadConnectionsPoints.Add(child.GetComponent<RoadConnectPoint>());
         }
      }      
   }

}

public enum TileType
{
   Building,
   Road
}

public enum SideConnection
{
   Connected,
   Closed
}
