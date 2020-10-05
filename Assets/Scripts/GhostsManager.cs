using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GhostsManager : MonoBehaviour
{
    public static GhostsManager Instance;
    [SerializeField] private List<GameObject> ghostPref;
    [SerializeField] private GameObject playerPrefab;
    private float[] _distancesGhosts;
    private List<Ghost> _ghosts;

    private AudioSource _source;

    public Player player;
    
    private float _sampling_frequency = 48000;

    [Range(0f, 1f)]
    public float noiseRatio = 0.1f;

    public float frequency = 440f;

    private float _increment;
    private float _phase;

    private float tmpDistancePlayer;
    System.Random rand = new System.Random();
    private AudioDistortionFilter _distortion;

    public GameObject buttonRestart;

    [SerializeField] private TextMeshProUGUI levels;
    [SerializeField] private TextMeshProUGUI score;
    private Coroutine _stopFilters = null;
    [FormerlySerializedAs("_play")] public bool play = true;
    private float[] _randNoise;

    [SerializeField] private float timeStopGhosts = 2;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        _randNoise = new float[44100];
        for (int j = 0; j < _randNoise.Length; ++j) {
            _randNoise[j] = Random.Range(0f, 1f);
        }
        _source = GetComponent<AudioSource>();
        Vector2 playerPos = ParserMap.Instance.mapConcrete[ParserMap.Instance.playerPos.x, ParserMap.Instance.playerPos.y].transform.position;
        GameObject playerGo = Instantiate(playerPrefab, new Vector3(playerPos.x, playerPos.y, -5), Quaternion.identity);
        player = playerGo.GetComponent<Player>();
        player.column = ParserMap.Instance.playerPos.x;
        player.row = ParserMap.Instance.playerPos.y;
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

            float fade(float t) {
                return t*t*t*(t*(t*6.0f - 15.0f) + 10.0f);
            }

            float grad(float p) 
            {
                
                float v =  _randNoise[Mathf.FloorToInt(p) % _randNoise.Length];
                return v > 0.5f ? 1.0f : -1.0f;
            }

            float noise(float p) {
                float p0 = Mathf.Floor(p);
                float p1 = p0 + 1.0f;

                float t = p - p0;
                float fade_t = fade(t);

                float g0 = grad(p0);
                float g1 = grad(p1);

                return (1.0f - fade_t)*g0*(p - p0) + fade_t*g1*(p - p1);
            }

            noisePart = noiseRatio * noise(_distancesGhosts[1]) * _distancesGhosts[1];

            _phase = _phase + _increment;
            if (_phase > 2 * Mathf.PI) _phase = 0;


            //tone
            tonalPart = (1f - noiseRatio) * (float)(_distancesGhosts[2] * (Mathf.Sin(_phase) * Mathf.Cos(_phase)));

            //together
            if (play)
            {
                data[i] += tonalPart / 20f;
                data[i] += noisePart / 5f;
                data[i] /= tmpDistancePlayer;
            }

            // // if we have stereo, we copy the mono data to each channel
            // if (channels == 2)
            // {
            //     data[i + 1] = data[i];
            //     i++;
            // }

            
        }

       
    }

    IEnumerator StopAllFilters()
    {
        play = false;
        _source.pitch = 1f;
        _source.volume = 1f;
        _distortion.enabled = false;
        yield return new WaitForSeconds(timeStopGhosts);
        play = true;
        _distortion.enabled = true;
    }

    public void StopAllFiltersEnd()
    {
        play = false;
        _source.pitch = 1f;
        _source.volume = 1f;
        _distortion.enabled = false;
    }

    public void StopAllGhosts()
    {
        if (_stopFilters != null)
        {
            StopCoroutine(_stopFilters);
            _stopFilters = null;
        }

        _stopFilters = StartCoroutine(StopAllFilters());
        for (int i = 0; i < _ghosts.Count; i++)
            _ghosts[i].Stop(timeStopGhosts);
        
    }


    // Update is called once per frame
    void Update()
    {
        if (!play)
            return;
        tmpDistancePlayer = Vector3.Distance(player.transform.position / 2.5f,
            ParserMap.Instance.finish.transform.position / 2.5f);
        _distortion.distortionLevel = Vector3.Distance(_ghosts[3].gameObject.transform.position.normalized * 1.2f,
            ParserMap.Instance.finish.transform.position.normalized * 1.2f);
        _source.volume = 1f / tmpDistancePlayer;
        _source.pitch = Mathf.Clamp(1f / Vector3.Distance(_ghosts[0].transform.position / 2.5f,
            ParserMap.Instance.finish.transform.position / 2.5f), 0, 1f);
        for (int i= 0; i < _ghosts.Count; i++)
            _distancesGhosts[i] = Vector3.Distance(_ghosts[i].gameObject.transform.position,
                ParserMap.Instance.finish.gameObject.transform.position);
        levels.text = "Level : " + Score.nbLevels;
        score.text = "Score : " + Score.GetScore();
    }
}
