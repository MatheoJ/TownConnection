using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

struct City
{
    public Point position;
    //hashmap of connected cities and the number of bridges between them
    public Dictionary<Point, int> connections;

    public City(Point position)
    {
        this.position = position;
        connections = new Dictionary<Point, int>();
    }
}

struct Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 toVector2() { return new Vector2(x, y); }
}

public class mapGenerator : MonoBehaviour
{

    

    

    private Queue<City> pointsToVisit;
    private City[,] cityMap;
    private List<Point>[,] pointsTakenByConnection;
    private int cityNumberLeft;
    private System.Random rand;


    private void Start()
    {
        int width = 10;
        int height = 10;
        int seed = 0;
        int cityNumber = 5;

        int[,] map = generateHashiMap(width, height, seed, cityNumber);

        Debug.Log("here -----------------");
        Debug.Log(map);

/*        for (int i = 0; i < width; i++)
        {
            string line = "";
            for (int j = 0; j < height; j++)
            {
                line += map[i, j] + " ";
            }
            Debug.Log(line);
        }*/
    }



    // Generate the matrice of the map
    public int[,] generateHashiMap(int width, int height, int seed, int cityNumber)
    {
        int[,] map = new int[width, height];
        rand = new System.Random(seed);

        //Queue to store the points to visit
        pointsToVisit = new Queue<City>();
        cityMap = new City[width, height];
        pointsTakenByConnection = new List<Point>[width, height];
        cityNumberLeft = cityNumber;
          
        //Create the first city
        int startX = rand.Next(0, width);
        int startY = rand.Next(0, height);
        City firstCity = new City(new Point(startX, startY));
        cityMap[startX, startY] = firstCity;
        cityNumberLeft--;
        pointsToVisit.Enqueue(firstCity);

        City currentCity;
        float probabilityToCreateConnection = 0.3f;

        //while points to visit are not empty
        while (pointsToVisit.Count > 0 && cityNumber > 0)
        {
            currentCity = pointsToVisit.Dequeue();

            for(int i = 0; i < 4; i++)
            {
                // probability to create a new city in this direction
                if( rand.NextDouble() < probabilityToCreateConnection)
                {
                    createConnectionInThisDirection(currentCity, i);
                }                    
            }                 
        }   

        map = getNbConnectionMapFromCityMap();

        return map;
    }

    private bool createConnectionInThisDirection(City currentCity, int direction)
    {
        int width = cityMap.GetLength(0);
        int height = cityMap.GetLength(1);

        Point currentPosition = new Point(currentCity.position.x, currentCity.position.y);

        int xIncrement = 0;
        int yIncrement = 0;

        switch (direction)
        {
            case 0:
                yIncrement = 1;
                break;
            case 1:
                xIncrement = 1;
                break;
            case 2:
                yIncrement = -1;
                break;
            case 3:
                xIncrement = -1;
                break;
        }


        int nextX = currentPosition.x + xIncrement;
        int nextY = currentPosition.y + yIncrement;

        //Check if the new position is not in the border of the map
        if (nextX <= 0 
            || nextX >= width-1 
            || nextY <= 0 
            || nextY >= height-1)
            return false;

        //Check if a connection already exists in this direction
        if (pointsTakenByConnection[nextX, nextY] != null)
        {
            if (pointsTakenByConnection[nextX, nextY].Contains(currentPosition))
                return addConnectionToExistingConnection(currentCity, new Point(nextX, nextY));
        }
        else
        {
            return createNewConnectionInThisDirection(currentCity, xIncrement, yIncrement);
        }

        return false;
    }

    private bool addConnectionToExistingConnection(City currentCity, Point nextPositionInDirection)
    {
        Point pointOfOtherCity= new Point();
        bool otherCityFound = false;

        foreach (Point point in pointsTakenByConnection[nextPositionInDirection.x, nextPositionInDirection.y])
        {
            if (!point.Equals(currentCity.position))
            {
                pointOfOtherCity = point;
                otherCityFound = true;
            }
        }

        if (!otherCityFound)
        {
            //Other city should be found, if not, there is a problem Log error
            Debug.LogError("Other city not found");
        }

        if (currentCity.connections.ContainsKey(pointOfOtherCity))
        {
            if (currentCity.connections[pointOfOtherCity]>1) 
            {
                return false;
            }

            currentCity.connections[pointOfOtherCity]++;
            return true;
        }
        else
        {
            Debug.LogError("Connection was not added in city  connection dictionary");
            return false;
        }
    }   

    private bool createNewConnectionInThisDirection(City currentCity, int xIncrement, int yIncrement)
    {
        int width = cityMap.GetLength(0);
        int height = cityMap.GetLength(1);

        List<int> distInDirectionPossible = new List<int>();
        int distInDirection = 1;
        //add the first point to the list 
        distInDirectionPossible.Add(distInDirection);
        bool canContinuInThisDirection = true;
        Point point = new Point(currentCity.position.x + xIncrement, currentCity.position.y + yIncrement);


        while (canContinuInThisDirection)
        {
            point = new Point(point.x + xIncrement, point.y + yIncrement);
            //Check if the new position is notout of the map
            if (point.x <= 0 || point.x >= width - 1 || point.y <= 0 || point.y >= height - 1)
            {
                canContinuInThisDirection = false;
            }
            //If there is already a city in this position
            else if (cityMap[point.x, point.y].connections.Count > 0)
            {
                canContinuInThisDirection = false;
                distInDirection++;
                //Change last distInDirectionPossible to the new distance
                distInDirectionPossible[distInDirectionPossible.Count - 1] = distInDirection;
            }
            else if (pointsTakenByConnection[point.x, point.y].Count>0)
            {
                canContinuInThisDirection = false;
                if (pointsTakenByConnection[point.x, point.y].Contains(currentCity.position))
                {
                    Debug.LogError("There should not be current city here as we already check if a connection existed");
                }
            }
            else
            {
                distInDirection++;
                distInDirectionPossible.Add(distInDirection);
            }
        }

        if (distInDirectionPossible.Count == 0)
        {
            Debug.LogError("No possible distance in this direction when there should at least be one");
            return false;
        }

        //Choose a random distance in the list
        int dist = distInDirectionPossible[rand.Next(0, distInDirectionPossible.Count)];

        //Create the new city
        Point newPoint = new Point(currentCity.position.x + xIncrement * dist, currentCity.position.y + yIncrement * dist);
        City newCity = new City(newPoint);
        newCity.connections.Add(currentCity.position, 1);
        currentCity.connections.Add(newPoint, 1);
        cityMap[newPoint.x, newPoint.y] = newCity;
        pointsToVisit.Enqueue(newCity);
        cityNumberLeft--;

        //Add the new city to the list of points taken by the connection
        for (int i = 1; i < dist; i++)
        {
            pointsTakenByConnection[currentCity.position.x + xIncrement * i, currentCity.position.y + yIncrement * i].Add(newPoint);
            pointsTakenByConnection[currentCity.position.x + xIncrement * i, currentCity.position.y + yIncrement * i].Add(currentCity.position);
        }

        return true;
    }

    private int[,] getNbConnectionMapFromCityMap()
    {
        int width = cityMap.GetLength(0);
        int height = cityMap.GetLength(1);
        int[,] map = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                foreach (KeyValuePair<Point, int> connection in cityMap[i, j].connections)
                {
                    map[i, j] += connection.Value;
                }
            }
        }

        return map;
    }
}


