using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostsManager : MonoBehaviour
{
    public static GhostsManager Instance;
    [SerializeField] private List<GameObject> ghostPref;
    private List<Ghost> _ghosts;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        _ghosts = new List<Ghost>();
        for (int i = 0; i < ghostPref.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            int x = 0;
            int y = 0;
            while (pos == Vector3.zero)
            {
                x = Random.Range(0, ParserMap.Instance.columns);
                y = Random.Range(0, ParserMap.Instance.rows);
                if (ParserMap.Instance.map[x, y] == 0)
                    pos = ParserMap.Instance.mapConcrete[x, y].transform.position;
            }
            pos = new Vector3(pos.x, pos.y, -5);
            GameObject go = Instantiate(ghostPref[i], pos, Quaternion.identity);
            _ghosts.Add(go.GetComponent<Ghost>());
            _ghosts[_ghosts.Count - 1].column = x;
            _ghosts[_ghosts.Count - 1].row = y;
        }

    }

    public void StopAllGhosts()
    {
        for (int i = 0; i < _ghosts.Count; i++)
            _ghosts[i].Stop();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
