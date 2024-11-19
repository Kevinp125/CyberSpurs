using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class baseState : MonoBehaviour
{
    public enemyAI enemy;
    public StateMachine s;
    public abstract void Enter();
    public abstract void Perform();
    public abstract void Exit();
}
