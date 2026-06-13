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
   private Tile[] _tileObjects;
   [SerializeField]
   private List<Cell> _gridComponents;
   [SerializeField]
   private Cell _cellObj;

   [SerializeField]
   private Tile _backupTile;

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
                Cell newCell = Instantiate(_cellObj, new Vector3(x, 0, y), Quaternion.identity);
                newCell.name = "Cell " + (int)(x + y*_dimensions);
                newCell.CreateCell(false, _tileObjects);
                _gridComponents.Add(newCell);
            }
        }

        StartCoroutine("CheckEntropy");
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
        //reorder the cells based on how many options they have
        tempGrid.Sort((a, b) => a.TileOptions.Length - b.TileOptions.Length);
        //remove all the cells except the first (and the ones that have the same amount of options)
        tempGrid.RemoveAll(a => a.TileOptions.Length != tempGrid[0].TileOptions.Length);

        yield return new WaitForSeconds(0.075f);

        if (tempGrid.Count > 0)
            CollapseCell(tempGrid); 
    }

    /// <summary>
    /// Retrieve a tile from the options and collapse it
    /// </summary>
    /// <param name="tempGrid"></param>
    private void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);
        //get random cell
        Cell cellToCollapse = tempGrid[randIndex];
        
        //get a random tile from the list
        try
        {
            
            int randomTile = UnityEngine.Random.Range(0, cellToCollapse.TileOptions.Length);
            Tile selectedTile = cellToCollapse.TileOptions[randomTile];
            cellToCollapse.Collapse(selectedTile);
            //Instantiate(selectedTile, cellToCollapse.transform.position, selectedTile.transformHandle.rotation);
        }
        catch
        {
            Debug.Log(int.Parse(cellToCollapse.name.Split(" ")[1]) + " has 0 options");
            Debug.Log(tempGrid.Count);
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
                            Tile[] validOption = _tileObjects[validIndex]._downNeighbours;
                            
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
                            
                            Tile[] validOption = _tileObjects[validIndex]._rightNeighbours;
                            
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
                            Tile[] validOption = _tileObjects[validIndex]._upNeighbours;
                            
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
                            Tile[] validOption = _tileObjects[validIndex]._leftNeighbours;
                            
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
}
