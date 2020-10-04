using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int column = 0;
    public int row = 1;
    Vector2Int _currentDirection = Vector2Int.up;
    Vector2Int _nextPosition = Vector2Int.zero;
    // Start is called before the first frame update
    void Start()
    {
        row -= 1;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, ParserMap.Instance.mapConcrete[column, row].transform.position, Time.deltaTime * 15f);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            _nextPosition = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _nextPosition = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            _nextPosition = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            _nextPosition = Vector2Int.right;
        if (Vector3.Distance(ParserMap.Instance.mapConcrete[column, row].transform.position, transform.position) >
            0.01f)
            return;
        if (ParserMap.Instance.map[column + _nextPosition.x, row + _nextPosition.y] == 0)
        {
            column += _nextPosition.x;
            row += _nextPosition.y;
            _currentDirection = _nextPosition;
        }
        else if (ParserMap.Instance.map[column + _currentDirection.x, row + _currentDirection.y] == 0)
        {
            column += _currentDirection.x;
            row += _currentDirection.y;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("pills"))
            GhostsManager.Instance.StopAllGhosts();
        else if (other.CompareTag("Finish"))
            Destroy(this);
        //load a new scene to continue
        else if (other.CompareTag("enemy"))
            Destroy(this);
        //load a new scene to stop
    }
}
