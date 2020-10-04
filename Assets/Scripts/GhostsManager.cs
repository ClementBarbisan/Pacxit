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

    }
    public double bpm = 140.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;

    private double nextTick = 0.0F;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0F;
    private int accent;
    private bool running = false;

    void Start()
    {
        accent = signatureHi;
        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;
        nextTick = startTick * sampleRate;
        running = true;
    }
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;

        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            int i = 0;
            while (i < channels)
            {
                data[n * channels + i] += x;
                i++;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
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
        _source.volume = 1f / (Vector3.Distance(_player.transform.position / 20f,
            ParserMap.Instance.finish.transform.position / 20f) * Vector3.Distance(_player.transform.position / 20f,
            ParserMap.Instance.finish.transform.position / 20f));
    }
}
