using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
   [SerializeField]
   private int _dimensions;
   [SerializeField]
   private int _tileSize = 10;
   [SerializeField]
   private Tile[] _tileObjects;
   [SerializeField]
   private List<Cell> _gridComponents;
   [SerializeField]
   private Cell _cellObj;

   [SerializeField]
   private Tile _backupTile;

   [SerializeField]
   private int _startingTile;   

    private void Awake()
    {
        _gridComponents = new List<Cell>();
        InitializeGrid();
    }

    /// <summary>
    /// Defines the grid and populates it
    /// </summary>
    private void InitializeGrid()
    {
        for (int y = 0; y < _dimensions; y++)
        {
            for (int x = 0; x < _dimensions; x++)
            {
                Cell newCell = Instantiate(_cellObj, new Vector3(x*_tileSize, 0, y*_tileSize), Quaternion.identity);
                newCell.name = "Cell " + (int)(x + y*_dimensions);
                newCell.CreateCell(false, _tileObjects);
                _gridComponents.Add(newCell);
            }
        }

        _gridComponents[_startingTile].Collapse(_backupTile);
        UpdateGeneration(_startingTile);

        //StartCoroutine("CheckEntropy");
    }

    /// <summary>
    /// Returns the item with the lowest entropy
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckEntropy()
    {
        
        List<Cell> tempGrid = new List<Cell>(_gridComponents);
        //remove all the collapsed cells
        tempGrid.RemoveAll(c => c.Collapsed);

        if (tempGrid.Count != 0)
        {
        
            //reorder the cells based on how many options they have
            
            //tempGrid.Sort((a, b) =>  b.AverageValue() - a.AverageValue());
            tempGrid = tempGrid.OrderByDescending((a) => a.AverageValue()).ThenBy((a) => a.TileOptions.Length).ToList();
            Debug.Log(tempGrid[0].AverageValue());
            if (tempGrid[0].AverageValue() == 0)
                Debug.LogError(tempGrid[0].name + " has value 0");
            //tempGrid.Sort((a, b) => a.TileOptions.Length - b.TileOptions.Length);
            //remove all the cells except the first (and the ones that have the same amount of options)
            //tempGrid.RemoveAll(a => a.TileOptions.Length != tempGrid[0].TileOptions.Length);
            tempGrid.RemoveAll(a => a.AverageValue() != tempGrid[0].AverageValue());

            //yield return new WaitForSeconds(0.001f);
            yield return new WaitForSeconds(0.075f);
            CollapseCell(tempGrid); 
            
        }
    }

    /// <summary>
    /// Retrieve a tile from the options and collapse it
    /// </summary>
    /// <param name="tempGrid"></param>
    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        //get random cell
        Cell cellToCollapse = tempGrid[0];
        
        //get a random tile from the list
        try
        {
            Debug.Log( cellToCollapse.name + " value " + cellToCollapse.AverageValue());
            int randomTile = UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length);
            Tile selectedTile = cellToCollapse.TileOptions[randomTile];
            cellToCollapse.Collapse(selectedTile);
            //Instantiate(selectedTile, cellToCollapse.transform.position, selectedTile.transformHandle.rotation);
        }
        catch
        {
            Debug.LogError(int.Parse(cellToCollapse.name.Split(" ")[1]) + " has 0 options");
            Tile selectedTile = _backupTile;
            cellToCollapse.Collapse(selectedTile);
            //Instantiate(selectedTile, cellToCollapse.transform.position, selectedTile.transformHandle.rotation);
        }
        //collapse the cell with the selected tile and instantiate
        
        
        UpdateGeneration(int.Parse(cellToCollapse.name.Split(" ")[1]));
    }

    /// <summary>
    /// Update what possible tiles are left in a cell and validates that a tile can exist there
    /// </summary>   
    private void UpdateGeneration(int lastCollapsedCellIndex)
    {
        
        List<Cell> newGenerationCell = new List<Cell>(_gridComponents);
        // for each cell in the grid
        for (int y = 0; y < _dimensions; y++)
        {
            for (int x = 0; x < _dimensions; x++)
            {
                //get the array index from the matrix
                int index = x + y * _dimensions;

                //if the cell is collapsed
                if (_gridComponents[index].Collapsed)
                {
                    //then add it to the list
                    newGenerationCell[index] = _gridComponents[index];
                } 
                else
                {
                    //else update the option list
                    List<Tile> options = _tileObjects.ToList();
                    
                    //only if we're not in the first row
                    if (y > 0)
                    {   
                        //check the cell above
                        Cell up = _gridComponents[x + (y-1) * _dimensions];

                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in up.TileOptions)
                        {
                            //retrive the valid options from the tile above
                            int validIndex = Array.FindIndex(_tileObjects, obj => obj == possibleOption);

                            Tile[] validOption = RetrieveCorrectTilesOptions(Direction.Down, _tileObjects[validIndex]);/*should pass the valid options of this cell considering all the possible options of the cell above*/
                            //_tileObjects[validIndex]._downNeighbours;
                            
                            //update this tile with what can be valid from above
                            validOptions = validOptions.Concat(validOption).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //only if we're not in the last column
                    if (x < _dimensions -1)
                    {
                        Cell left = _gridComponents[x + 1 + y*_dimensions];

                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in left.TileOptions)
                        {
                            //retrive the valid options from the tile on the left
                            
                            int validIndex = Array.FindIndex(_tileObjects, obj => obj == possibleOption);
                            
                            Tile[] validOption = RetrieveCorrectTilesOptions(Direction.Right, _tileObjects[validIndex]);
                            //_tileObjects[validIndex]._rightNeighbours;
                            
                            //update this tile with what can be valid from the left
                            validOptions = validOptions.Concat(validOption).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //only if we're not in the last row
                    if (y < _dimensions - 1)
                    {   
                        //check the cell above
                        Cell down = _gridComponents[x + (y+1) * _dimensions];
   
                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in down.TileOptions)
                        {
                            //retrive the valid options from the tile below
                            int validIndex = Array.FindIndex(_tileObjects, obj => obj == possibleOption);
                            Tile[] validOption = RetrieveCorrectTilesOptions(Direction.Up, _tileObjects[validIndex]);
                            //_tileObjects[validIndex]._upNeighbours;
                            
                            //update this tile with what can be valid from below
                            validOptions = validOptions.Concat(validOption).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //only if we're not in the first column
                    if (x > 0)
                    {
                        Cell right = _gridComponents[x - 1 + y*_dimensions];

                        List<Tile> validOptions = new List<Tile>();
                        foreach (Tile possibleOption in right.TileOptions)
                        {
                            //retrive the valid options from the tile on the left
                            int validIndex = Array.FindIndex(_tileObjects, obj => obj == possibleOption);
                            Tile[] validOption = RetrieveCorrectTilesOptions(Direction.Left, _tileObjects[validIndex]);
                            //_tileObjects[validIndex]._leftNeighbours;
                            
                            //update this tile with what can be valid from the left
                            validOptions = validOptions.Concat(validOption).ToList();
                        }

                        CheckValidity(options, validOptions);
                    }

                    //we create the new valid tiles list
                    Tile[] newTileList = new Tile[options.Count];

                    for (int i = 0; i < options.Count; i++)
                    {
                        newTileList[i] = options[i];
                    }
                        

                    //recreate the cell from the list 
                    newGenerationCell[index].Recreatecell(newTileList);
                }
            }
        }

        _gridComponents = newGenerationCell;

        StartCoroutine(CheckEntropy());
    }

    private void CheckValidity(List<Tile> optionList, List<Tile> validOptions)
    {
        //for each tile in the option list
        for (int x = optionList.Count-1; x >= 0; x--)
        {
            Tile element = optionList[x];
            //if the valid options (from neighbours) dont contain it then discard the option
            if (!validOptions.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }

    private void PrintOptions(List<Tile> options)
    {
        string opts = "";
        foreach (Tile option in options)
        {
            if (option != null)
                opts += ", " + option.name;
        }

        UnityEngine.Debug.Log(opts);
    }

    private Tile[] RetrieveCorrectTilesOptions(Direction directionToCheck, Tile tileToCheck)
    {
        List<Tile> allTiles = _tileObjects.ToList();
        switch (directionToCheck)
        {
            case Direction.Down:
                //remove all tiles that doesn't have the same tipe of connection (closed/open)
                allTiles.RemoveAll(tile => tile.Up != tileToCheck.Down);
                //if tileToCheck has a closed down, then retrieve all closed up. Same for open
                //then remove all the blacklisted tiles
                allTiles.RemoveAll(tile => tileToCheck.TilesBlacklist.Contains(tile));

                //if the connection is closed remove also every other road
                if (tileToCheck.Type == TileType.Road && tileToCheck.Down == SideConnection.Closed)
                    allTiles.RemoveAll(tile => tile.Type == TileType.Road);
                break;
            case Direction.Up:
                //remove all tiles that doesn't have the same tipe of connection (closed/open)
                allTiles.RemoveAll(tile => tile.Down != tileToCheck.Up);
                //if tileToCheck has a closed down, then retrieve all closed up. Same for open
                //then remove all the blacklisted tiles
                allTiles.RemoveAll(tile => tileToCheck.TilesBlacklist.Contains(tile));
                //if the connection is closed remove also every other road
                if (tileToCheck.Type == TileType.Road && tileToCheck.Up == SideConnection.Closed)
                    allTiles.RemoveAll(tile => tile.Type == TileType.Road);
                break;
            case Direction.Left:
                //remove all tiles that doesn't have the same tipe of connection (closed/open)
                allTiles.RemoveAll(tile => tile.Right != tileToCheck.Left);
                //if tileToCheck has a closed down, then retrieve all closed up. Same for open
                //then remove all the blacklisted tiles
                allTiles.RemoveAll(tile => tileToCheck.TilesBlacklist.Contains(tile));

                //if the connection is closed remove also every other road
                if (tileToCheck.Type == TileType.Road && tileToCheck.Left == SideConnection.Closed)
                    allTiles.RemoveAll(tile => tile.Type == TileType.Road);
                break;
            case Direction.Right:
                //remove all tiles that doesn't have the same tipe of connection (closed/open)
                allTiles.RemoveAll(tile => tile.Left != tileToCheck.Right);
                //if tileToCheck has a closed down, then retrieve all closed up. Same for open
                //then remove all the blacklisted tiles
                allTiles.RemoveAll(tile => tileToCheck.TilesBlacklist.Contains(tile));

                //if the connection is closed remove also every other road
                if (tileToCheck.Type == TileType.Road && tileToCheck.Right == SideConnection.Closed)
                    allTiles.RemoveAll(tile => tile.Type == TileType.Road);
                break;
        }

        return allTiles.ToArray();
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
