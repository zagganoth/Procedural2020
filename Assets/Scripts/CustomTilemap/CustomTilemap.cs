using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTilemap : MonoBehaviour
{
    private Grid<TileBase> grid;
    // Start is called before the first frame update
    void Start()
    {
        grid = new Grid<TileBase>(new Vector3Int(0,0,0),5,10,10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
