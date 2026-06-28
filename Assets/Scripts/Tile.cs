using UnityEngine;

public class Tile : MonoBehaviour
{
   public Tile[] _upNeighbours;
   public Tile[] _leftNeighbours;
   public Tile[] _rightNeighbours;
   public Tile[] _downNeighbours;

   public int Value;

   public TileType Type;

}

public enum TileType
{
   Building,
   Road
}
