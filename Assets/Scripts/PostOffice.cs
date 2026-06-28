using UnityEngine;

public class PostOffice : MonoBehaviour
{
    [SerializeField]
    private Transform _playerTransform, _vespaTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject.FindGameObjectWithTag("Player").transform.position = _playerTransform.position;
        GameObject.FindGameObjectWithTag("Vespa").transform.position = _vespaTransform.position;
    }    

}
