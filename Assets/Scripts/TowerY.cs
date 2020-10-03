using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerY : Ghost
{
    private bool _up = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stop)
            return;
        base.Update();
        if (Vector3.Distance(ParserMap.Instance.mapConcrete[column, row].transform.position, transform.position) >
            0.01f)
            return;
        if (ParserMap.Instance.map[column - 1, row] == 0 && _up)
        {
            column -= 1;
        }
        else if (ParserMap.Instance.map[column + 1, row] == 0)
        {
            _up = false;
            column += 1;
        }
        else
        {
            column -= 1;
            _up = true;
        }
    }
}
