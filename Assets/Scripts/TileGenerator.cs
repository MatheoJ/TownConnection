using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    public Tilemap grass;
    public UnityEngine.Tilemaps.Tile grass1;
    public UnityEngine.Tilemaps.Tile grass2;
    public UnityEngine.Tilemaps.Tile grass3;



    public Vector3Int mapSize = new Vector3Int(3, 3, 0);

    //public bool CityTile=false;


    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                switch (Random.Range(0, 3))
                {
                    case 0:
                        grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), grass1);
                        break;
                    case 1:
                        grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), grass2);
                        break;
                    case 2:
                        grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), grass3);
                        break;
                }

            }
        }
    }

    public void CityTile()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
