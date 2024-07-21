using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CityTile
{ 
    public int nbConnectionNeeded;
    public Dictionary<CityTile, int> connections;
    public Vector2Int position;

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

    [SerializeField] private Camera _camera;

    [SerializeField] private mapGenerator mapGenerator;

    private Dictionary<Vector2, Tile> _tiles;

    [SerializeField] private List<City> cities=new();



    private CityTile[,] cityTileMap;
    private RoadTile[,] roadTileMap;

    private Vector3 Origin;
    private Vector3 Difference;

    private bool drag = false;


    public GameObject helpText;
    public GameObject VictoryScreen;
    public Button startMenuButton;
    void Start()
    {
        GenerateGrid();
        ChangeCameraZoomToSeeAllGrid();

        //get random seed
        int seed = UnityEngine.Random.Range(0, 1000);
        int cityNumber = 12;

        cities = mapGenerator.generateHashiMap(_width, _height, seed, cityNumber);
        cityTileMap = new CityTile[_width, _height];
        roadTileMap = new RoadTile[_width, _height];

        PlaceCities(cities);

        startMenuButton.onClick.AddListener(LauncheStartMenu);

    }

    void Update()
    {
        //If R is pressed, reload the scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }


        //If wheel scroll is detected, zoom in or out
        if (Input.mouseScrollDelta.y != 0)
        {
            _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - Input.mouseScrollDelta.y, 1, 100);
        }

        //Moove camera if right click is dragged
        if (Input.GetMouseButton(1))
        {
            Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - _cam.position;
            if (drag == false)
            {
                drag = true;
                Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Difference = new Vector3(0, 0, 0);
            }
            else
            {
                Difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Origin;
            }

        }
        else
        {
            drag = false;
        }
        if (drag)
        {
            _cam.position = _cam.position - Difference;
            Origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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

    void ChangeCameraZoomToSeeAllGrid()
    {
        _camera.orthographicSize = Mathf.Max(_width, _height) / 2;
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
            cityTileMap[(int)cities[i].position.x, (int)cities[i].position.y].position = new Vector2Int(cities[i].position.x, cities[i].position.y);
        }        
    }

    private Vector2 currentSelection=new Vector2(-1,-1);

    public void CitySelected(Vector2 pos)
    {
        if (currentSelection == new Vector2(-1, -1))
        {
            currentSelection = pos;
            GetTileAtPosition(currentSelection).IsSelected(true);
        }
        else if (currentSelection != pos)
        {
            GetTileAtPosition(currentSelection).IsSelected(false);
            Debug.Log(CountConnections(cityTileMap[(int)pos.x, (int)pos.y]));
            if (!GetTileAtPosition(currentSelection).GoodRoads(CountConnections(cityTileMap[(int)currentSelection.x, (int)currentSelection.y]) == GetTileAtPosition(currentSelection).numConnections))
            {
                GetTileAtPosition(currentSelection).BadRoads(CountConnections(cityTileMap[(int)currentSelection.x, (int)currentSelection.y]) > GetTileAtPosition(currentSelection).numConnections);
            }
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
        bool roadCreated = false;
        CityTile selecTedTile = cityTileMap[(int)currentSelection.x, (int)currentSelection.y];
        CityTile pos2Tile = cityTileMap[(int)pos2.x, (int)pos2.y];

        if (pos2.x == currentSelection.x)
        {
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
            roadCreated = true;
        }
        if(pos2.y == currentSelection.y)
        {
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
            roadCreated = true;
        }

        if (roadCreated)
        {
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
        }

        if (!GetTileAtPosition(pos2).GoodRoads(CountConnections(pos2Tile) == GetTileAtPosition(pos2).numConnections))
        {
            GetTileAtPosition(pos2).BadRoads(CountConnections(pos2Tile) > GetTileAtPosition(pos2).numConnections);
        }
        else
        {
            CheckVictory();
        }

        if (!GetTileAtPosition(currentSelection).GoodRoads(CountConnections(selecTedTile) == GetTileAtPosition(currentSelection).numConnections))
        {
            GetTileAtPosition(currentSelection).BadRoads(CountConnections(selecTedTile) > GetTileAtPosition(currentSelection).numConnections);
        }
        else
        {
            CheckVictory();
        }
        
        currentSelection = new Vector2(-1, -1);
    }

    private int CountConnections(CityTile tile)
    {
        int s = 0;
        foreach (var c in tile.connections)
        {
            s += c.Value;
        }
        return s;
    }

    internal void GrassSelecTed(Vector2 pos)
    {
        if (roadTileMap[(int)pos.x, (int)pos.y] != null)
        {
            CityTile city1 = roadTileMap[(int)pos.x, (int)pos.y].connectedTile[0];
            CityTile city2 = roadTileMap[(int)pos.x, (int)pos.y].connectedTile[1];

            if (roadTileMap[(int)pos.x, (int)pos.y].nbRoad == 1)
            {

                city1.connections.Remove(city2);
                city2.connections.Remove(city1);

                if (city1.position.x == city2.position.x)
                {
                    for (int y = Math.Min(city1.position.y, city2.position.y) + 1; y < Math.Max(city1.position.y, city2.position.y); y++)
                    {
                        roadTileMap[city1.position.x, y] = null;
                        GetTileAtPosition(new Vector2(city1.position.x, y)).ClearRoad();
                    }
                }
                else
                {
                    for (int x = Math.Min(city1.position.x, city2.position.x) + 1; x < Math.Max(city1.position.x, city2.position.x); x++)
                    {
                        roadTileMap[x, city1.position.y] = null;
                        GetTileAtPosition(new Vector2(x, city1.position.y)).ClearRoad();
                    }
                }
            }
            else
            {
                city1.connections[city2]--;
                city2.connections[city1]--;

                if (city1.position.x == city2.position.x)
                {
                    for (int y = Math.Min(city1.position.y, city2.position.y) + 1; y < Math.Max(city1.position.y, city2.position.y); y++)
                    {
                        roadTileMap[city1.position.x, y].nbRoad--;
                        GetTileAtPosition(new Vector2(city1.position.x, y)).ClearRoad();
                        GetTileAtPosition(new Vector2(city1.position.x, y)).VerticalRoad();
                    }
                }
                else
                {
                    for (int x = Math.Min(city1.position.x, city2.position.x) + 1; x < Math.Max(city1.position.x, city2.position.x); x++)
                    {
                        roadTileMap[x, city1.position.y].nbRoad--;
                        GetTileAtPosition(new Vector2(x, city1.position.y)).ClearRoad();
                        GetTileAtPosition(new Vector2(x, city1.position.y)).HorizontalRoad();

                    }
                }
            }
            if (!GetTileAtPosition(city1.position).GoodRoads(CountConnections(city1) == GetTileAtPosition(city1.position).numConnections))
            {
                GetTileAtPosition(city1.position).BadRoads(CountConnections(city1) > GetTileAtPosition(city1.position).numConnections);
                helpText.SetActive(false);
            }
            else
            {
                CheckVictory();
            }

            if (!GetTileAtPosition(city2.position).GoodRoads(CountConnections(city2) == GetTileAtPosition(city2.position).numConnections))
            {
                GetTileAtPosition(city2.position).BadRoads(CountConnections(city2) > GetTileAtPosition(city2.position).numConnections);
                helpText.SetActive(false);
            }
            else
            {
                CheckVictory();
            }
        }
    }

    public bool areAllCitiesConnected()
    {
        List<CityTile> visited = new List<CityTile>();
        List<CityTile> toVisit = new List<CityTile>();

        toVisit.Add(cityTileMap[cities[0].position.x, cities[0].position.y]);

        while (toVisit.Count > 0)
        {
            CityTile current = toVisit[0];
            toVisit.RemoveAt(0);
            visited.Add(current);

            foreach (KeyValuePair<CityTile, int> connection in current.connections)
            {
                if (!visited.Contains(connection.Key) && !toVisit.Contains(connection.Key))
                {
                    toVisit.Add(connection.Key);
                }
            }
        }

        return visited.Count == cities.Count;
    }

    private void CheckVictory()
    {

        foreach (City city in cities)
        {
            if (!GetTileAtPosition(city.position.toVector2()).hasGoodRoads)
            {
                return;
            }
        }
        if (!areAllCitiesConnected())
        {
            //afficher le pb
            helpText.SetActive(true);
            return;
        }

        Debug.Log("victory!!!");
        VictoryScreen.SetActive(true);

    }

    public void LauncheStartMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
    }
}
