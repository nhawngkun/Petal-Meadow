using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class bird_PatrolState : IState
{
    public ChimController bird;
    public NavMeshAgent agent;
    float timer;
    public bird_PatrolState(ChimController bird)
    {
        this.bird = bird;
        agent = bird.agent;

    }
    public void OnEnter()
    {
        timer = Random.Range(1, 2);

    }
    public virtual void Execute()
    {

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            bird.ChangeState<bird_ChaseState>();
        }
    }



    public void OnExit()
    {

    }
}