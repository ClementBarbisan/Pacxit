using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Ghost
{
    private int _oldColumn;
    private int _oldRow;

    [SerializeField]
    private float chasingDistance;


    // Start is called before the first frame update
    void Start()
    {
        _oldColumn = Random.Range(0,2);
        _oldRow = 1 - _oldColumn;
    }

    // Update is called once per frame
    void Update()
    {
        if (stop || !GhostsManager.Instance.play)
            return;
        base.Update();
        float distanceToGo = Vector3.Distance(ParserMap.Instance.mapConcrete[column, row].transform.position,
            transform.position);
        if (distanceToGo > 0.01f)
        {
            return;
        }
        if (Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) < chasingDistance)
        {
            if (ParserMap.Instance.map[column + _oldColumn, row + _oldRow] == 0 && 
                Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) >=
                Vector3.Distance(ParserMap.Instance.mapConcrete[column + _oldColumn, row + _oldRow].transform.position,
                    GhostsManager.Instance.player.transform.position))
            {
                column += _oldColumn;
                row += _oldRow;
            }
            else
            {
                _oldColumn = 0;
                _oldRow = 0;
           
                if (ParserMap.Instance.map[column + 1, row] == 0 &&
                    Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) >=
                    Vector3.Distance(ParserMap.Instance.mapConcrete[column + 1, row].transform.position,
                        GhostsManager.Instance.player.transform.position))
                {
                    column += 1;
                    _oldColumn = 1;
                    _oldRow = 0;
                }
                else if (ParserMap.Instance.map[column - 1, row] == 0 &&
                     Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) >=
                     Vector3.Distance(ParserMap.Instance.mapConcrete[column - 1, row].transform.position,
                         GhostsManager.Instance.player.transform.position))
                {
                    column -= 1;
                    _oldColumn = -1;
                    _oldRow = 0;
                }

            
                else if (ParserMap.Instance.map[column, row - 1] == 0 &&
                    Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) >=
                    Vector3.Distance(ParserMap.Instance.mapConcrete[column, row - 1].transform.position,
                        GhostsManager.Instance.player.transform.position))
                {
                    row -= 1;
                    _oldColumn = 0;
                    _oldRow = -1;
                }

           
                else if (ParserMap.Instance.map[column, row + 1] == 0 &&
                    Vector3.Distance(transform.position, GhostsManager.Instance.player.transform.position) >=
                    Vector3.Distance(ParserMap.Instance.mapConcrete[column, row + 1].transform.position,
                        GhostsManager.Instance.player.transform.position))
                {
                    row += 1;
                    _oldColumn = 0;
                    _oldRow = 1;
                }
            }
        }
        else
        {
            if (ParserMap.Instance.map[column + _oldColumn, row + _oldRow] == 0)
            {
                column += _oldColumn;
                row += _oldRow;
            }
            else
            {
                _oldColumn = 0;
                _oldRow = 0;
                while (_oldColumn == 0 && _oldRow == 0)
                {
                    switch (Random.Range(0, 4))
                    {

                        case 0:
                            if (ParserMap.Instance.map[column + 1, row] == 0)
                            {
                                column += 1;
                                _oldColumn = 1;
                                _oldRow = 0;
                            }

                            break;
                        case 1:
                            if (ParserMap.Instance.map[column - 1, row] == 0)
                            {
                                column -= 1;
                                _oldColumn = -1;
                                _oldRow = 0;
                            }

                            break;
                        case 2:
                            if (ParserMap.Instance.map[column, row - 1] == 0)
                            {
                                row -= 1;
                                _oldColumn = 0;
                                _oldRow = -1;
                            }

                            break;
                        case 3:
                            if (ParserMap.Instance.map[column, row + 1] == 0)
                            {
                                row += 1;
                                _oldColumn = 0;
                                _oldRow = 1;
                            }

                            break;
                    }
                }
            }
        }
    }
}
