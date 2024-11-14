using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private Transform handTransform; 
    [SerializeField] private Transform golfCartTransform; 

    private NavMeshAgent agent;
    private GameObject currentTarget;
    private GameObject carriedBall;
    private bool isPickingAnimationComplete = true;

    [Header("Game Over UI")] 
    [SerializeField] private GameObject gameOverBackgroundPanel;
    [SerializeField] private GameObject gameOverAnimation;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI pointsText;
    private int totalPoints = 0;
    
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
        
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError("Can't find Health Component!!!");
        }
        UpdatePointsUI();
    }

    private void Start()
    {
        InvokeRepeating(nameof(DecideAndMove), 0f, updateInterval);
    }

    private void DecideAndMove()
    {
        if (!isPickingAnimationComplete)
        {
            return;
        }

        if (currentTarget == null || HasReachedTarget())
        {
            if (currentTarget != null)
            {
                if (carriedBall == null)
                {
                    StartCoroutine(PickUpBall(currentTarget));
                }
                else
                {
                    StartCoroutine(DeliverBallToCart());
                }
            }
            else
            {
                GameObject bestBall = DecideTarget();
                if (bestBall != null)
                {
                    currentTarget = bestBall;
                    agent.stoppingDistance = 1f;
                    agent.SetDestination(currentTarget.transform.position);
                }
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

        float changeDistanceWeight = distanceWeight;
        float adjustedLevelWeight = levelWeight;

        if (currentHealth <= 50 && carriedBall == null)
        {
            changeDistanceWeight *= 1.5f;
            adjustedLevelWeight *= 0.5f;
        }

        if (currentHealth <= 20 && carriedBall == null)
        {
            changeDistanceWeight *= 2f;
            adjustedLevelWeight *= 0.2f;
        }

        float utilityScore = (adjustedLevelWeight * levelFactor) - (changeDistanceWeight * distanceFactor) - (healthWeight * healthFactor);
        return utilityScore;
    }

    private void Update()
    {
        if (health.GetCurrentHealth() <= 0)
        {
            DeadHealth();
            return;
        }
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
        if (health.GetCurrentHealth() <= 0)
        {
            return false;
        }
        return agent.velocity.magnitude > 0.1f && !IsPicking();
    }

    public bool IsPicking()
    {
        if (health.GetCurrentHealth() <= 0)
        {
            return false;
        }
        return HasReachedTarget() && !isPickingAnimationComplete;
    }

    private IEnumerator PickUpBall(GameObject ball)
    {
        isPickingAnimationComplete = false;
        carriedBall = ball;
        carriedBall.transform.SetParent(handTransform);
        carriedBall.transform.localPosition = Vector3.zero; 
        yield return new WaitForSeconds(2.5f); 
        currentTarget = golfCartTransform.gameObject;
        agent.stoppingDistance = 2.5f;
        agent.SetDestination(golfCartTransform.position);
        isPickingAnimationComplete = true;
    }

    private IEnumerator DeliverBallToCart()
    {
        isPickingAnimationComplete = false;
        GolfBallGetInfo ballInfo = carriedBall.GetComponent<GolfBallGetInfo>();
        if (ballInfo != null)
        {
            EarnPoints(ballInfo.GetLevel());
        }
        Destroy(carriedBall);
        carriedBall = null;
        yield return new WaitForSeconds(2.5f); 
        agent.stoppingDistance = 1f;
        currentTarget = null;
        isPickingAnimationComplete = true;
    }

    private void EarnPoints(int level)
    {
        int pointsEarned = GetPointsAccordingToLevel(level);
        totalPoints += pointsEarned;
        UpdatePointsUI();
        Debug.Log("Earned points for level: " + level + "Total points:" + totalPoints);
    }

    public int GetPointsAccordingToLevel(int level)
    {
        if (carriedBall != null)
        {
            GolfBallGetInfo ballInfo = carriedBall.GetComponent<GolfBallGetInfo>();
            
            if (ballInfo != null && ballInfo.golfBallSO != null)
            {
                return ballInfo.golfBallSO.points;
            }
        }
        return 0;
    }

    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + totalPoints.ToString();
        }
    }

    private void DeadHealth()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        isPickingAnimationComplete = true;
        //StopAllCoroutines();
        ActivateGameOverUI();
    }

    private void ActivateGameOverUI()
    {
        if (gameOverBackgroundPanel != null && gameOverAnimation != null)
        {
            gameOverBackgroundPanel.SetActive(true);
            gameOverAnimation.SetActive(true);
        }
    }
}
