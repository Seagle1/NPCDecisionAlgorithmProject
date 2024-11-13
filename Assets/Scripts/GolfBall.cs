using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GolfBall : MonoBehaviour
{
    [SerializeField] private GolfBallSO[] golfBallSO;
    [SerializeField] private Transform[] easyTransforms;
    [SerializeField] private Transform[] mediumTransforms;
    [SerializeField] private Transform[] hardTransforms;
    [SerializeField] private Terrain terrain;
    [SerializeField] private float yOffset = 0.5f;
    private void Awake()
    {
        foreach (var golfBallSO in golfBallSO)
        {
            InstantiateGolfBalls(golfBallSO);
        }
    }

    private void InstantiateGolfBalls(GolfBallSO golfBallSO)
    {
        Transform[] currentSpawnTransforms = null;

        switch (golfBallSO.level)
        {
            case 1:
                currentSpawnTransforms = easyTransforms;
                break;
            case 2:
                currentSpawnTransforms = mediumTransforms;
                break;
            case 3:
                currentSpawnTransforms = hardTransforms;
                break;
            default:
                Debug.LogError("GolfBallSO level is invalid");
                return;
        }
        
        foreach (var spawnTransform in currentSpawnTransforms)
        {
            BoxCollider boxCollider = spawnTransform.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                Debug.LogError("I cannot reach BoxCollider");
                continue; 
            }


            for (int i = 0; i < golfBallSO.instantiateCount; i++)
            {
                Vector3 randomPos = GetRandomPosition(boxCollider.bounds);
                float terrainHeight = terrain.SampleHeight(new Vector3(randomPos.x, 0, randomPos.z));
                randomPos.y += terrainHeight + yOffset;
                Instantiate(golfBallSO.ballPrefab, randomPos, Quaternion.identity);
            }
        }
    }

    private Vector3 GetRandomPosition(Bounds bounds)
    {
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
      
        return new Vector3(x, 0, z);
    }
}