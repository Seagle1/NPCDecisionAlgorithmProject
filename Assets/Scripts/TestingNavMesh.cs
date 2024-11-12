using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TestingNavMesh : MonoBehaviour
{
 private NavMeshAgent agent;
 [SerializeField] private Transform target;
 private void Awake()
 {
    agent = GetComponent<NavMeshAgent>();
 }

 private void Update()
 {
     agent.destination = target.position;
 }
}
