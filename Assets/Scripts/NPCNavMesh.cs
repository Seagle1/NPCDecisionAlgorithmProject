using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavMesh : MonoBehaviour, IHealth
{
    private Health health;
    [Header("Decision Weights")]
    [SerializeField] private float levelWeight = 1f;
    [SerializeField] private float distanceWeight = 1f;
    [SerializeField] private float healthWeight = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float maxConsiderationDistance = 100f;
    [SerializeField] private float updateInterval = 1f;

    private NavMeshAgent agent;
    private GameObject currentTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Can't find Health Component!!!");
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(DecideAndMove), 0f, updateInterval);
    }
    
    private void DecideAndMove()
    {
        if (currentTarget == null || HasReachedTarget())
        {
            if (currentTarget != null)
            {
                // Simulate picking up the ball
                PickUpBall(currentTarget);
            }

            GameObject bestBall = DecideTarget();
            if (bestBall != null)
            {
                currentTarget = bestBall;
                agent.SetDestination(currentTarget.transform.position);
            }
        }
    }

    private bool HasReachedTarget()
    {
        if (currentTarget == null)
        {
            return false;
        }
        return agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;
    }

    private GameObject DecideTarget()
    {
        List<GameObject> allGolfBalls = GolfBallManager.Instance.GetAllGolfBalls();
        GameObject bestBall = null;
        float bestUtility = float.MinValue;

        foreach (GameObject golfBall in allGolfBalls)
        {
            if (golfBall == null)
            {
                continue;
            }
            GolfBallGetInfo ballInfo = golfBall.GetComponent<GolfBallGetInfo>();
            if (ballInfo == null)
            {
                continue;
            }

            float utility = CalculateUtility(golfBall.transform.position, ballInfo.GetLevel());
            if (utility > bestUtility)
            {
                bestUtility = utility;
                bestBall = golfBall;
            }
        }

        return bestBall;
    }

    private float CalculateUtility(Vector3 ballPosition, int ballLevel)
    {
        float distance = Vector3.Distance(transform.position, ballPosition);
        if (distance > maxConsiderationDistance)
        {
            return float.MinValue;
        } 
        float distanceFactor = distance / maxConsiderationDistance; 
        
        float currentHealth = health.GetCurrentHealth();
        float maxHealth = health.GetMaxHealth();
        float healthFactor = (maxHealth - currentHealth) / maxHealth; 

        int maxLevel = 3;
        float levelFactor = (float)ballLevel / maxLevel; 

        float utilityScore = (levelWeight * levelFactor) - (distanceWeight * distanceFactor) - (healthWeight * healthFactor);
        return utilityScore;
    }

    private void Update()
    {
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    public void DecreaseHealth(float amount)
    {
        float currentHealth = health.GetCurrentHealth();
        float maxHealth = health.GetMaxHealth();
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public bool IsWalking()
    {
        return agent.velocity.magnitude > 0.1f && !IsPicking();
    }

    public bool IsPicking()
    {
        return HasReachedTarget();
    }

    private void PickUpBall(GameObject ball)
    {
        GolfBallManager.Instance.UnregisterGolfBall(ball);
        Destroy(ball);
    }
}
