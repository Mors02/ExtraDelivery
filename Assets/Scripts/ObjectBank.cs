using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectBank", menuName = "Scriptable Objects/ObjectBank")]
public class ObjectBank : ScriptableObject
{
    public List<GameObject> ObjectList;
    public EnvironmentDressingType Type;

    /// <summary>
    /// how many objects are present inside the bank
    /// </summary>
    public int Count => ObjectList.Count;
    
    /// <summary>
    /// returns a random object from the list
    /// </summary>
    public GameObject RandomObject {
        get
        {
            int idx = Random.Range(0, Count);
            Debug.Log(idx);
            return ObjectList[idx];
        }
    }
}
