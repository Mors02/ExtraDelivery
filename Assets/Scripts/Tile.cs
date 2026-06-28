using UnityEngine;

public class Tile : MonoBehaviour
{
   public Tile[] TilesBlacklist;

   public SideConnection Up;
   public SideConnection Down;
   public SideConnection Left;
   public SideConnection Right;



   public int Value;

   public TileType Type;

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
