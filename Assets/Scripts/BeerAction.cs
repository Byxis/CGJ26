using UnityEngine;

public class BeerAction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("Bière bue !");
    }
}
