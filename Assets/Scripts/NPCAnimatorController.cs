using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimatorController : MonoBehaviour
{
   [SerializeField] private TestingNavMesh testingNavMesh;
    private Animator animator;
    private const string IS_WALKING = "IsWalking";
    private const string IS_PICKING = "IsPicking";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        testingNavMesh.OnAnimationSystem += TestingNavMeshOnOnAnimationSystem;
    }

    private void TestingNavMeshOnOnAnimationSystem(object sender, EventArgs e)
    {
        animator.SetBool(IS_WALKING, testingNavMesh.IsWalking());
        animator.SetBool(IS_PICKING, testingNavMesh.IsPicking());
    }
}
