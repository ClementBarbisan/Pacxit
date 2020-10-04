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
    private float[] _distancesGhosts;
    private List<Ghost> _ghosts;

    private AudioSource _source;

    private Player _player;
    
    private float _sampling_frequency = 48000;

    [Range(0f, 1f)]
    public float noiseRatio = 0.5f;

    public float frequency = 440f;

    private float _increment;
    private float _phase;

    private float tmpDistancePlayer;
    System.Random rand = new System.Random();
    private AudioHighPassFilter _audioHighPassFilter;
    private AudioDistortionFilter _distortion;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        _source = GetComponent<AudioSource>();
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
                Debug.LogError("Duplicate Ghosts!Parameters won't change correctly!");
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
        _distancesGhosts = new float[_ghosts.Count];
    }
    
    void Start()
    {
        _sampling_frequency = AudioSettings.outputSampleRate;
        _audioHighPassFilter = GetComponent<AudioHighPassFilter>();
        _distortion = GetComponent<AudioDistortionFilter>();
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        float tonalPart = 0;
        float noisePart = 0;
        // update increment in case frequency has changed
        _increment = frequency * 2f * Mathf.PI / _sampling_frequency;

        for (int i = 0; i < data.Length; i++)
        {
            
            //noise
            noisePart = noiseRatio * (float)(rand.NextDouble() * _distancesGhosts[1] * 2.0);

            _phase = _phase + _increment;
            if (_phase > 2 * Mathf.PI) _phase = 0;


            //tone
            tonalPart = (1f - noiseRatio) * (float)(_distancesGhosts[2] * Mathf.Sin(_phase) / 10f);

            //together
            data[i] += noisePart / 25 + tonalPart;

            // if we have stereo, we copy the mono data to each channel
            if (channels == 2)
            {
                data[i + 1] = data[i];
                i++;
            }

            
        }

       
    }
    public void StopAllGhosts()
    {
        for (int i = 0; i < _ghosts.Count; i++)
            _ghosts[i].Stop();
    }


    // Update is called once per frame
    void Update()
    {
        tmpDistancePlayer = Vector3.Distance(_player.transform.position / 2f,
            ParserMap.Instance.finish.transform.position / 2f);
        _distortion.distortionLevel = Vector3.Distance(_ghosts[3].gameObject.transform.position.normalized * 1.5f,
            ParserMap.Instance.finish.transform.position.normalized * 1.5f);
        _audioHighPassFilter.cutoffFrequency = _distancesGhosts[0] * 250f;
        _source.volume = 1f / Vector3.Distance(_player.transform.position / 2f,
            ParserMap.Instance.finish.transform.position / 2f);
        for (int i= 0; i < _ghosts.Count; i++)
            _distancesGhosts[i] = Vector3.Distance(_ghosts[i].gameObject.transform.position,
                ParserMap.Instance.finish.gameObject.transform.position);
    }
}
