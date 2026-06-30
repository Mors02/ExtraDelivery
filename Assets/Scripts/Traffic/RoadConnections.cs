using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Android;

public class RoadConnections : MonoBehaviour
{   
    [SerializeField]
    List<RoadInfo> _roadTiles;
    List<Cell> _cells;

    [SerializeField]
    NavMeshLink _linkPrefab;

    private int _dimensions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _roadTiles = new List<RoadInfo>();
        _cells = new List<Cell>();
    }


    public void CreateRoadSystem(List<Cell> grid, int dimensions)
    {   
        //retrieve all the roads
        foreach(Cell cell in grid)
        {
            //we're interested only in roads
            if (cell.SelectedTile.Type == TileType.Road)
            {
                int index = int.Parse(cell.name.Split(" ")[1]);
                _roadTiles.Add(new RoadInfo(index, cell.GetComponentInChildren<Tile>()));
            }
        }

        _dimensions = dimensions;

        _cells = grid;

        //now implement the logic for each tile
        foreach (RoadInfo roadInfo in _roadTiles)
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                ApplyConnections(direction, roadInfo);
            }
            
        }
    }

    public void ApplyConnections(Direction connectingToward, RoadInfo info)
    {   
        List<RoadConnectPoint> connectionPoints = info.Tile.RoadConnectionsPoints;
        Debug.Log(connectingToward + " index: " + info.Index);

        //General rules: 
        // 1) the start of the connection is always Right (except for down which is Left => Left)
        // 2) when connectinToward is Left or Right, you need to connect the opposite of the connection direction (Left => Right or Right => Left)
        // 3) when connectingToward is Up or Down, you need to connect the same side of the connection direction (Left => Left or Right => Right)
        // 4) this function only uses the the start of the connection for each direction (since the opposite side will be taken care by another tile)
        RoadConnectPoint startPoint = null;

        //retrieve the index of the next cell
        int x = info.Index % _dimensions;
        int y = info.Index / _dimensions;
        bool validIndex = false;
        int oppositeCellIndex = OppositeIndexFromDirection(connectingToward, x, y, ref validIndex);
        Side startSide = connectingToward == Direction.Down? Side.Left : Side.Right;
        Side endSide = connectingToward == Direction.Up? Side.Right : Side.Left;

        Debug.Log(connectionPoints.Count);
        //retrieve the points with the correct direction on the starting side of link
        List<RoadConnectPoint> directedPoints = connectionPoints.Where(point => {return point.Side == startSide && point.DirectedToward == OppositeOf(connectingToward);}).ToList();
        if (directedPoints.Count == 0)
            return;
        startPoint = directedPoints[0];
        
        
        
        //check if within bounds
        Debug.Log(info.Index + "  " + connectingToward + " looking for " + oppositeCellIndex);
        if (validIndex)
        {
            Cell oppositeCell = _cells[oppositeCellIndex];

            //ignore everything that isn't a road
            if (oppositeCell.SelectedTile.Type != TileType.Road)
                return;
            List<RoadConnectPoint> oppositeCellPoints = oppositeCell.GetComponentInChildren<Tile>().RoadConnectionsPoints.Where(point => point.Side == endSide && point.DirectedToward == connectingToward).ToList();
            //if the opposite cell doesn't have points in the correct direction
            if (oppositeCellPoints.Count == 0)
                return;
            //retrieve the respective point on the other cell (it should always be one)
            RoadConnectPoint oppositePoint = oppositeCellPoints[0];
            //create the link 
            NavMeshLink link = Instantiate(_linkPrefab, this.transform);   
            //set up start and end
            link.startPoint = startPoint.transform.position;
            link.endPoint = oppositePoint.transform.position;        
        }
    } 

    /// <summary>
    /// Returns opposite direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    private Direction OppositeOf(Direction direction)
    {
        switch(direction)
        {
            case Direction.Down:
                return Direction.Up;
            
            case Direction.Up:
                return Direction.Down;

            case Direction.Left:
                return Direction.Right;

            case Direction.Right:
                return Direction.Left;
            default:
            return Direction.Up;
        }
    }

    /// <summary>
    /// Returns index of cell in the direction from this
    /// </summary>
    /// <param name="direction">Which direction to look</param>
    /// <param name="x">x of the starting cell</param>
    /// <param name="y">y of the starting cell</param>
    /// <param name="validIndex">true if it's a valid index in the grid, false otherwise</param>
    /// <returns></returns>
    private int OppositeIndexFromDirection(Direction direction, int x, int y, ref bool validIndex)
    {
        switch(direction)
        {
            case Direction.Down:
                validIndex = y > 0;
                return x + (y-1) * _dimensions;
            
            case Direction.Up:
                validIndex = y < _dimensions-1;
                return x + (y+1) * _dimensions;

            case Direction.Left:
                validIndex = x < _dimensions-1;
                return x + 1 + y*_dimensions;

            case Direction.Right:
                validIndex = x > 0;
                return x - 1 + y*_dimensions;
            default:
            return 0;
        }
        
    }
}

[Serializable]
public class RoadInfo
{
    public int Index;
    public Tile Tile;

    public RoadInfo(int index, Tile tile)
    {
        this.Index = index;
        this.Tile = tile;
    }
}
