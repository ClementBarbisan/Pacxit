using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public bool stop = false;
    public float freezeTime = 2f;
    private Coroutine stopForAwhile = null;
    public int column = 0;
    public int row = 0;
    IEnumerator StopAsync()
    {
        yield return new WaitForSeconds(freezeTime);
        stop = false;
    }

    public void Stop()
    {
        stop = true;
        if (stopForAwhile != null)
        {
            StopCoroutine(stopForAwhile);
            stopForAwhile = null;
        }

        stopForAwhile = StartCoroutine(StopAsync());
    }

    protected void Update()
    {
        if (stop)
            return;
        transform.position = Vector3.Lerp(transform.position, ParserMap.Instance.mapConcrete[column, row].transform.position, Time.deltaTime * 15f);
    }
}
