using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private AudioSource soundClick;

    GameObject gridManager;
    public Tilemap grass;
    public UnityEngine.Tilemaps.Tile grass1;
    public UnityEngine.Tilemaps.Tile grass2;
    public UnityEngine.Tilemaps.Tile grass3;

    //public UnityEngine.Tilemaps.Tile beton;

    public Vector3Int mapSize = new Vector3Int(3, 3, 0);

    public bool isCity=false;
    public Tilemap city;
    public UnityEngine.Tilemaps.Tile[] cityTiles;
    public UnityEngine.Tilemaps.Tile[] numberTiles;
    public UnityEngine.Tilemaps.Tile[] redCityTiles;
    public UnityEngine.Tilemaps.Tile[] blueCityTiles;
    public UnityEngine.Tilemaps.Tile[] greenCityTiles;

    public UnityEngine.Tilemaps.Tile verticalRoad;
    public UnityEngine.Tilemaps.Tile horizontalRoad;

    [SerializeField] private bool isSelected = false;
    public int numConnections;

    public bool hasGoodRoads = false;
    private bool hasBadRoads = false;

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
        numConnections=nbConnetions;
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
        isCity = true;
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
        //Debug.Log(gridManager.GetComponent< GridManager>().GetTileAtPosition(transform.position).name);
        //CityTile();
        if (isCity)
        {
            string[] pos = name.Split(' ');
            gridManager.GetComponent<GridManager>().CitySelected(new Vector2(int.Parse(pos[1]), int.Parse(pos[2])));
            Debug.Log(pos[1] + ' ' + pos[2]);
        }  
        else
        {
            string[] pos = name.Split(' ');
            gridManager.GetComponent<GridManager>().GrassSelecTed(new Vector2(int.Parse(pos[1]), int.Parse(pos[2])));
        }

        soundClick.Play();
    }


    public void VerticalRoad()
    {
        for (int y = 0; y < mapSize.y; y++)
        {            
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -y + mapSize.y / 2, 0), verticalRoad);            
        }

        
    }

    public void DoubleVerticalRoad()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (x == 1) 
                {
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), null);
                }else
                {
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), verticalRoad);
                }
                
            }
        }
    }

    public void DoubleHorizontalRoad() 
    {
       for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (y == 1)
                {
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), null);
                }
                else
                {
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), horizontalRoad);
                }

            }
        }
    }


    public void HorizontalRoad()
    {
        for (int x = 0; x < mapSize.x; x++)
        {           
            city.SetTile(new Vector3Int(-x + mapSize.x / 2, -1 + mapSize.y / 2, 0), horizontalRoad);                
        }        
    }

    public void IsSelected()
    {
        if (isSelected)
        {
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
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);
        }
        else
        {
            int i = 0;
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    //grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), beton);
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), blueCityTiles[i]);
                    i++;
                }
            }
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);
        }
        isSelected=!isSelected;
    }

    public void ClearRoad() {         
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), null);
            }
        }
    }

    public bool GoodRoads(bool goodRoads)
    {
        hasGoodRoads = goodRoads;
        if (!goodRoads)
        {
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
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);


            return false;
        }
        else
        {
            int i = 0;
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    //grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), beton);
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), greenCityTiles[i]);
                    i++;
                }
            }
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);
            return true;
        }

    }

    public void BadRoads(bool badRoads)
    {
        if (!badRoads)
        {
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
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);
        }
        else
        {
            int i = 0;
            for (int x = 0; x < mapSize.x; x++)
            {
                for (int y = 0; y < mapSize.y; y++)
                {
                    //grass.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), beton);
                    city.SetTile(new Vector3Int(-x + mapSize.x / 2, -y + mapSize.y / 2, 0), redCityTiles[i]);
                    i++;
                }
            }
            city.SetTile(new Vector3Int(-1 + mapSize.x / 2, -1 + mapSize.y / 2, 0), numberTiles[numConnections]);
        }
        hasBadRoads = badRoads;
    }

}
