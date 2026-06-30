using UnityEngine;

public class RoadConnectPoint : MonoBehaviour
{
    
    public Side Side;

    [SerializeField]
    private bool _connected;


    public Direction DirectedToward;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Connect()
    {
        _connected = true;
    }
}

public enum Side
{
    Left,
    Right
}
