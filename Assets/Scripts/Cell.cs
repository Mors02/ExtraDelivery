using UnityEngine;
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

    public void CreateCell(bool collapseState, Tile[] tiles)
    {
        _collapsed = collapseState;
        _tileOptions = tiles;
    }

    public void Recreatecell(Tile[] tiles)
    {
        _tileOptions = tiles;
    }

    public void Collapse(Tile selectedTile)
    {
        this._collapsed = true;
        this._tileOptions = new Tile[] {selectedTile};
        _selectedTile = selectedTile;
        Tile obj = Instantiate(_selectedTile, transform.position, _selectedTile.transform.rotation);
        this.transform.localScale = obj.transform.localScale;
        obj.transform.parent = this.transform;

    }
}
