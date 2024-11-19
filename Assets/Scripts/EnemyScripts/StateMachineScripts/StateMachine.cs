using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public baseState activeState;
    public PatrolState patrolState;

    public void Initialize()
    {
        patrolState = new PatrolState();
        changeState(patrolState);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activeState != null)
        {
            activeState.Perform();
        }
    }

    public void changeState(baseState newState)
    {
        if(activeState != null)
        {
            activeState.Exit();
        }

        activeState = newState;

        if(activeState != null)
        {
            activeState.s = this;
            activeState.enemy = GetComponent<enemyAI>();
            activeState.Enter();
        }
    }
}
