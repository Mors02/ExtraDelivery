using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadDressing : MonoBehaviour
{

    [SerializeField]
    private List<EnvironmentDressing> _dressingSpots;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (EnvironmentDressing spot in _dressingSpots)
        {
            if (spot.CheckExistance())
            {           
                //Instantiate();
                GameObject obj = spot.RandomObject();
                Debug.Log(spot.Position.position);
                if (obj != null)
                {
                    
                    GameObject instance = Instantiate(obj, spot.Position.position, spot.Position.rotation);
                    
                    //instance.transform.localPosition = spot.Position.localPosition;
                }
                    
            }

        }
    }

}


[Serializable]
public class EnvironmentDressing
{
    [SerializeField]
    public Transform Position;
    [SerializeField]
    public EnvironmentDressingType Type;
    [Range(1, 100)]
    [SerializeField]
    public int Probability;

    public bool Active;

    /// <summary>
    /// Rolls a number, if greater than probability it's removed
    /// </summary>
    public bool CheckExistance()
    {
        float value = UnityEngine.Random.Range(0, 99);
        return value < Probability;
    }

    /// <summary>
    /// Returns a random object of the selected type
    /// </summary>
    /// <returns></returns>
    public GameObject RandomObject()
    {
        ObjectBank bank = GameAssets.i.Banks[(int)Type];
        if (bank.Count > 0)
        {
            return bank.RandomObject;
        }

        return null;
    }
}


public enum EnvironmentDressingType
{
    Sidewalk,
    Car,

}
