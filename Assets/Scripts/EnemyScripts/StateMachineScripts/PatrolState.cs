using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : baseState
{
    public int waypointIndex;
    public float waitTimer;
    
    public override void Enter()
    {
        if (enemy.path.waypoints.Count > 0)
        {
            enemy.Agent.SetDestination(enemy.path.waypoints[0].position);
        }

    }

    public override void Perform()
    {
        PatrolCycle();
    }

    public override void Exit()
    {

    }

    public void PatrolCycle()
    {
        if(enemy.Agent.remainingDistance < 0.2)
        {
            waitTimer += Time.deltaTime;


                if (waypointIndex < enemy.path.waypoints.Count - 1)
                {
                    waypointIndex++;
                }

                else
                {
                    waypointIndex = 0;
                }

                enemy.Agent.SetDestination(enemy.path.waypoints[waypointIndex].position);
                waitTimer = 0;
            

            
        }
    }
}
