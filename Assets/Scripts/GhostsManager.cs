using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostsManager : MonoBehaviour
{
    public static GhostsManager Instance;
    [SerializeField] private List<GameObject> ghostPref;
    [SerializeField] private GameObject playerPrefab;
    private List<Ghost> _ghosts;

    private AudioSource _source;

    private Player _player;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        _source = FindObjectOfType<AudioSource>();
        Vector2 playerPos = ParserMap.Instance.mapConcrete[ParserMap.Instance.playerPos.x, ParserMap.Instance.playerPos.y].transform.position;
        GameObject playerGo = Instantiate(playerPrefab, new Vector3(playerPos.x, playerPos.y, -5), Quaternion.identity);
        _player = playerGo.GetComponent<Player>();
        _player.column = ParserMap.Instance.playerPos.x;
        _player.row = ParserMap.Instance.playerPos.y;
        _ghosts = new List<Ghost>();
        int index = 0;
        for (int i = 0; i < ParserMap.Instance.ghostPos.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            int x = ParserMap.Instance.ghostPos[i].x;
            int y = ParserMap.Instance.ghostPos[i].y;
            pos = ParserMap.Instance.mapConcrete[x, y].transform.position;
            pos = new Vector3(pos.x, pos.y, -5);
            if (ghostPref.Count <= i)
            {
                Debug.LogWarning("Duplicate Ghosts!");
            }
            else
            {
                index = i;
            }
            GameObject ghostGo = Instantiate(ghostPref[index], pos, Quaternion.identity);
            _ghosts.Add(ghostGo.GetComponent<Ghost>());
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
