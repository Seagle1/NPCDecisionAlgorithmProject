using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TestingNavMesh : MonoBehaviour
{
 private NavMeshAgent agent;
 [SerializeField] private Transform target;
 public event EventHandler OnAnimationSystem;
 private bool isWalking;
 private bool isPicking;
 private void Awake()
 {
    agent = GetComponent<NavMeshAgent>();
 }
 
 private void Update()
 {
     NavMeshLogic();
 }

 public void NavMeshLogic()
 {
     agent.destination = target.position;
     isWalking = agent.remainingDistance > agent.stoppingDistance;
     isPicking = agent.remainingDistance <= agent.stoppingDistance;
     OnAnimationSystem?.Invoke(this, EventArgs.Empty);
 }

 public bool IsWalking()
 {
     return isWalking;
 }

 public bool IsPicking()
 {
     return isPicking;
 }
}
