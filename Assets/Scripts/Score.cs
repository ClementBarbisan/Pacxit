using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static int movements = 0;
    public static int nbLevels = 1;

    public static int GetScore()
    {
        return (10 * nbLevels - movements);
    }

    public static void Reset()
    {
        movements = 0;
        nbLevels = 1;
    }
}
