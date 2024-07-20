using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    GameObject gridManager;
    public Tilemap grass;
    public UnityEngine.Tilemaps.Tile grass1;
    public UnityEngine.Tilemaps.Tile grass2;
    public UnityEngine.Tilemaps.Tile grass3;

    //public UnityEngine.Tilemaps.Tile beton;

    public Vector3Int mapSize = new Vector3Int(3, 3, 0);

    //public bool CityTile=false;
    public Tilemap city;
    public UnityEngine.Tilemaps.Tile[] cityTiles;
    public UnityEngine.Tilemaps.Tile[] numberTiles;



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
    

    
        gridManager = GameObject.Find("GridManager");
    }

    public void CityTile(int nbConnetions)
    {
        grass.color = new Color(0, 0, 0, 0.9f);
        int i = 0;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                //grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), beton);
                city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), cityTiles[i]);
                i++;
            }
        }
        city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[nbConnetions]);
    }

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        grass.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        Debug.Log(gridManager.GetComponent< GridManager>().GetTileAtPosition(transform.position).name);
        //CityTile();
    }

}
