using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bird_ChaseState : IState
{
    public ChimController bird;
    public NavMeshAgent agent;
    public bird_ChaseState(ChimController bird)
    {
        this.bird = bird;
        agent = bird.agent;

    }
    public void OnEnter()
    {

        if (bird.agent != null && bird.agent.isActiveAndEnabled && bird.agent.isOnNavMesh)
        {
            bird.agent.ResetPath();
            bird.agent.isStopped = false;
        }
        Vector3 pos = bird.GetPatrolPoints();
        bird.agent.SetDestination(pos);

    }
    public void Execute()
    {
        if (bird.agent != null && bird.agent.isActiveAndEnabled && bird.agent.isOnNavMesh)
        {
            Chasing();
        }

    }
    public void Chasing()
    {
        // Nếu đang nhảy → không xử lý chase


        if (!bird.agent.hasPath || bird.agent.remainingDistance <= bird.agent.stoppingDistance)
        {
            bird.ChangeState<bird_PatrolState>();
            return;
        }

    }
    public void OnExit()
    {
        if (bird.agent != null && bird.agent.isActiveAndEnabled && bird.agent.isOnNavMesh)
        {
            bird.agent.ResetPath();
            bird.agent.isStopped = true;
        }
    }
}