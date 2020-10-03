using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ParserMap : MonoBehaviour
{
    [SerializeField] private string nameMap = "map.txt";
    [SerializeField] private GameObject wall = null;
    [SerializeField] private GameObject empty = null;
    [SerializeField] private GameObject exit = null;
    [SerializeField] private GameObject terrain = null;
    [SerializeField] private GameObject teleport = null;
    [SerializeField]
    private string[] _mapDeserialize;
    [SerializeField]
    private int[,] _map;
    private int _rows = 0;
    private int _columns = 0;
    private Camera _cam = null;
    private void Awake()
    {
        _cam = Camera.main;
        _mapDeserialize = File.ReadAllLines(Application.streamingAssetsPath + "/" + nameMap);
        _rows = _mapDeserialize.Length;
        _columns = _mapDeserialize[0].Length;
        _map = new int[_columns,_rows];
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                if (!int.TryParse(_mapDeserialize[j][i].ToString(), out _map[i, j]))
                {
                    Debug.LogError("Invalid Map!!!");
                    return;
                }
            }
        }
        for (int i = 0; i < _columns; i++)
        {
            for (int j = 0; j < _rows; j++)
            {
                switch (_map[i, j])
                {
                    case 0:
                        Instantiate(terrain,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(wall,
                            new Vector3(i * terrain.gameObject.transform.right.x + 0.5f,
                                j * -terrain.gameObject.transform.up.y), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(empty,
                            new Vector3(i * terrain.gameObject.transform.right.x +0.5f,
                                j * -terrain.gameObject.transform.up.y), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(teleport,
                            new Vector3(i * terrain.gameObject.transform.right.x,
                                j * -terrain.gameObject.transform.up.y), Quaternion.identity);
                        break;
                }
            }
        }
        _cam.gameObject.transform.position = new Vector3(terrain.transform.right.x * _columns / 2, -terrain.transform.up.y * _rows / 2, -5);
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
