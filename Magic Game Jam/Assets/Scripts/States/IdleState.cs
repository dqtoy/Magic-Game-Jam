using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IdleState : ByTheTale.StateMachine.State
{
    public GameManagerScript exampleCharacter { get { return (GameManagerScript)machine; } }
    // Use this for initialization
    public override void Enter()
    {

        base.Enter();
        //play drum loop


    }

    public override void Execute()
    {
        base.Execute();

        if (Input.GetKeyDown(KeyCode.N))
        {
            machine.ChangeState<EnemyBattleState>();
        }


    }

    
}


