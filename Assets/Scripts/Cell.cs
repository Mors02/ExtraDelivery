using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class Cell : MonoBehaviour
{

    [SerializeField]
    private bool _collapsed;
    
    public bool Collapsed => _collapsed;
    [SerializeField]
    private Tile[] _tileOptions;

    public Tile[] TileOptions => _tileOptions;

    [SerializeField]
    private Tile _selectedTile;

    public Tile SelectedTile => _selectedTile;

    [SerializeField]
    private int _totalValue, _averageValue;


    public void CreateCell(bool collapseState, Tile[] tiles)
    {
        _collapsed = collapseState;
        _tileOptions = tiles;

    }

    public void Recreatecell(Tile[] tiles)
    {
        if (tiles.Length == 0)
            Debug.LogError("No tiles on " + this.name);
        _tileOptions = tiles;
    }

    public void Collapse(Tile selectedTile)
    {
        this._collapsed = true;
        this._tileOptions = new Tile[] {selectedTile};
        _selectedTile = selectedTile;
        Tile obj = Instantiate(_selectedTile, transform.position, _selectedTile.transform.rotation);
        //this.transform.localScale = obj.transform.localScale;
        obj.transform.SetParent(this.transform);

    }

    public int AverageValue()
    {
        if (_tileOptions.Length == 0)
        {
            _averageValue = 0;
            return 0;
        }
            
        _averageValue = _tileOptions.Sum(tile => tile.Value) / _tileOptions.Length;
        return _averageValue;
    }

    public int TotalValue()
    {
        return _tileOptions.Sum(tile => tile.Value);
    }


}
