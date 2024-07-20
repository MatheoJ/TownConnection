using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;

    [SerializeField] private Tile _tilePrefab;

    [SerializeField] private Transform _cam;

    [SerializeField] private mapGenerator mapGenerator;

    private Dictionary<Vector2, Tile> _tiles;

    [SerializeField] private List<City> cities=new();

    void Start()
    {
        GenerateGrid();

        //get random seed
        int seed = UnityEngine.Random.Range(0, 1000);
        int cityNumber = 5;

        cities = mapGenerator.generateHashiMap(_width, _height, seed, cityNumber);

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
        }        
    }

}
