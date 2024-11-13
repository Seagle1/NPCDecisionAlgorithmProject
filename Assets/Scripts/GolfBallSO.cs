using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GolfBallSO", menuName = "GolfBallSO")]
public class GolfBallSO : ScriptableObject
{
    public int level;
    public GameObject ballPrefab;
    public int points;
    public int instantiateCount;
}
