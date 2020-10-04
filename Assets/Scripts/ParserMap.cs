using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ParserMap : MonoBehaviour
{
    public static ParserMap Instance;
    [SerializeField] private string nameMap = "map.txt";
    [SerializeField] private GameObject wall = null;
    [SerializeField] private GameObject empty = null;
    [SerializeField] private GameObject exit = null;
    [SerializeField] private GameObject terrain = null;
    [SerializeField] private GameObject teleport = null;
    [SerializeField] private GameObject pills = null;
    private string[] _mapDeserialize;
    [FormerlySerializedAs("_map")] public int[,] map;
    [FormerlySerializedAs("_mapConcrete")] public GameObject[,] mapConcrete;
    [FormerlySerializedAs("_rows")] public int rows = 0;
    [FormerlySerializedAs("_columns")] public int columns = 0;
    public List<Vector2Int> ghostPos;
    public Vector2Int playerPos;
    public GameObject finish;
    private Camera _cam = null;
    private void Awake()
    {
        Instance = this;
        ghostPos = new List<Vector2Int>();
        _cam = Camera.main;
        _mapDeserialize = File.ReadAllLines(Application.streamingAssetsPath + "/" + nameMap);
        rows = _mapDeserialize.Length;
        columns = _mapDeserialize[0].Length;
        map = new int[columns,rows];
        mapConcrete = new GameObject[columns,rows];
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                if (!int.TryParse(_mapDeserialize[j][i].ToString(), out map[i, j]))
                {
                    Debug.LogError("Invalid Map!!!");
                    return;
                }
            }
        }
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                switch (map[i, j])
                {
                    case 0:
                        mapConcrete[i, j] = Instantiate(terrain,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    case 1:
                        mapConcrete[i, j] = Instantiate(wall,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    case 2:
                        mapConcrete[i, j] = Instantiate(empty,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    case 3:
                        ghostPos.Add(new Vector2Int(i, j));
                        map[i, j] = 0;
                        mapConcrete[i, j] = Instantiate(terrain,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    case 4:
                        playerPos = new Vector2Int(i, j);
                        map[i, j] = 0;
                        mapConcrete[i, j] = Instantiate(terrain,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    
                    case 5:
                        map[i, j] = 0;
                        mapConcrete[i, j] = Instantiate(terrain,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        Instantiate(pills,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                    case 6:
                        mapConcrete[i, j] = Instantiate(teleport,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y - 0.5f, -5), Quaternion.identity);
                        break;
                }
            }
        }

        int x = Random.Range(0, columns);
        int y = Random.Range(0, rows);
        while (map[x, y] != 0 && new Vector2Int(x, y) != playerPos)
        {
            x = Random.Range(0, columns);
            y = Random.Range(0, rows);
        }

        finish = Instantiate(exit,
            new Vector3(x * terrain.gameObject.transform.right.x + 0.5f, y * -terrain.gameObject.transform.up.y - 0.5f, -5),
            Quaternion.identity);
        _cam.orthographicSize = rows * terrain.transform.up.y / 2; 
        _cam.gameObject.transform.position = new Vector3(terrain.transform.right.x * columns / 2, -terrain.transform.up.y * rows / 2, -10);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
