using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CityTile
{ 
    public int nbConnectionNeeded;
    public Dictionary<CityTile, int> connections;

    public CityTile()
    {
        nbConnectionNeeded = 0;
        connections = new Dictionary<CityTile, int>();
    }
    public CityTile(int nbConnection)
    {
        nbConnectionNeeded = nbConnection;
        connections = new Dictionary<CityTile, int>();
    }
}

public class RoadTile
{
    public List<CityTile> connectedTile;
    public int nbRoad;

    public RoadTile()
    {
        connectedTile = new List<CityTile>();
        nbRoad = 0;
    }
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    [SerializeField] private mapGenerator mapGenerator;

    private Dictionary<Vector2, Tile> _tiles;

    [SerializeField] private List<City> cities=new();

    private CityTile[,] cityTileMap;
    private RoadTile[,] roadTileMap;


    void Start()
    {
        GenerateGrid();

        //get random seed
        int seed = UnityEngine.Random.Range(0, 1000);
        int cityNumber = 12;

        cities = mapGenerator.generateHashiMap(_width, _height, seed, cityNumber);
        cityTileMap = new CityTile[_width, _height];
        roadTileMap = new RoadTile[_width, _height];

        PlaceCities(cities);

    }

    void Update()
    {
        //If R is pressed, reload the scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }

    void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }

    private void PlaceCities(List<City> cities)
    {
        for (int i = 0; i < cities.Count; i++) {
            int nbConnections = 0;
            foreach (KeyValuePair<Point, int> connection in cities[i].connections)
            {
                nbConnections += connection.Value;
            }
            GetTileAtPosition(cities[i].position.toVector2()).CityTile(nbConnections);
            cityTileMap[(int)cities[i].position.x, (int)cities[i].position.y] = new CityTile();

        }        
    }

    private Vector2 currentSelection=new Vector2(-1,-1);

    public void CitySelected(Vector2 pos)
    {
        if (currentSelection == new Vector2(-1, -1))
        {
            currentSelection = pos;
        }
        else
        {
            if (cityTileMap[(int)pos.x, (int)pos.y].connections.ContainsKey(cityTileMap[(int)currentSelection.x, (int)currentSelection.y]))
            {
                if (cityTileMap[(int)pos.x, (int)pos.y].connections[cityTileMap[(int)currentSelection.x, (int)currentSelection.y]]>1)
                {
                    currentSelection = new Vector2(-1, -1);
                    return;
                }
            }
            else if (!checkedForRoadsOrCity(currentSelection,pos))
            {
                currentSelection = new Vector2(-1, -1);
                return;
            }

            CreateRoad(pos);
        }

    }

    private bool checkedForRoadsOrCity(Vector2 currentSelection, Vector2 pos2)
    {
        bool res = true;

        if (pos2.x == currentSelection.x)
        {
            Debug.Log("creer route verticale");
            for (int y = (int)Mathf.Min(pos2.y, currentSelection.y) + 1; y < (int)Mathf.Max(pos2.y, currentSelection.y); y++)
            {
                if (roadTileMap[(int)pos2.x, y]!=null || cityTileMap[(int)pos2.x, y] != null)
                {
                    res = false;
                }
            }
        }
        if (pos2.y == currentSelection.y)
        {
            Debug.Log("creer route horizontale");
            for (int x = (int)Mathf.Min(pos2.x, currentSelection.x) + 1; x < (int)Mathf.Max(pos2.x, currentSelection.x); x++)
            {
                if (roadTileMap[x, (int)pos2.y] != null || cityTileMap[x, (int)pos2.y] != null)
                {
                    res = false;
                }
            }
        }

        return res;
    }

    private List<Vector2[]> createdRoads = new List<Vector2[]>();
    public void CreateRoad(Vector2 pos2)
    {

        CityTile selecTedTile = cityTileMap[(int)currentSelection.x, (int)currentSelection.y];
        CityTile pos2Tile = cityTileMap[(int)pos2.x, (int)pos2.y];

        if (pos2.x == currentSelection.x)
        {
            Debug.Log("creer route verticale");
            for(int y =(int)Mathf.Min(pos2.y, currentSelection.y)+1;y< (int)Mathf.Max(pos2.y, currentSelection.y); y++)
            {

                if (roadTileMap[(int)pos2.x, y] == null)
                {
                    RoadTile roadTile = new RoadTile();
                    roadTile.connectedTile.Add(selecTedTile);
                    roadTile.connectedTile.Add(pos2Tile);
                    roadTile.nbRoad = 1;
                    roadTileMap[(int)pos2.x, y] = roadTile;
                    GetTileAtPosition(new Vector2(pos2.x, y)).VerticalRoad();
                }
                else
                {
                    roadTileMap[(int)pos2.x, y].nbRoad++;
                    GetTileAtPosition(new Vector2(pos2.x, y)).DoubleVerticalRoad();
                }                
            }         

        }
        if(pos2.y == currentSelection.y)
        {
            Debug.Log("creer route horizontale");
            for (int x = (int)Mathf.Min(pos2.x, currentSelection.x) + 1; x < (int)Mathf.Max(pos2.x, currentSelection.x); x++)
            {
                if (roadTileMap[x, (int)pos2.y] == null)
                {
                    RoadTile roadTile = new RoadTile();
                    roadTile.connectedTile.Add(selecTedTile);
                    roadTile.connectedTile.Add(pos2Tile);
                    roadTile.nbRoad = 1;
                    roadTileMap[x, (int)pos2.y] = roadTile;
                    GetTileAtPosition(new Vector2(x, pos2.y)).HorizontalRoad();
                }
                else
                {
                    roadTileMap[x, (int)pos2.y].nbRoad++;
                    GetTileAtPosition(new Vector2(x, pos2.y)).DoubleHorizontalRoad();
                }

            }
        }
        //Debug.Log("reset");

        if (pos2Tile.connections.ContainsKey(selecTedTile))
        {
            pos2Tile.connections[selecTedTile]++;
            selecTedTile.connections[pos2Tile]++;
        }
        else
        {
            pos2Tile.connections.Add(selecTedTile, 1);
            selecTedTile.connections.Add(pos2Tile, 1);
        }

        currentSelection = new Vector2(-1, -1);
    }


}