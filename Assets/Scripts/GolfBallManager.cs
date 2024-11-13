using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBallManager : MonoBehaviour
{
    public static GolfBallManager Instance { get; private set; }
    private List<GameObject> golfBalls = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        { 
            Debug.LogError("More than one GolfBallManager in scene.");
        }
        Instance = this;
    }

    public void RegisterGolfBall(GameObject golfBall)
    {
        if (!golfBalls.Contains(golfBall))
        {
            golfBalls.Add(golfBall);
        }
    }
    
    public void UnregisterGolfBall(GameObject golfBall)
    {
        if (golfBalls.Contains(golfBall))
        {
            golfBalls.Remove(golfBall);
        }
    }
    
    public List<GameObject> GetAllGolfBalls()
    {
        return new List<GameObject>(golfBalls);
    }
}
