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
        GameManagerScript.instance.playerAC.SetBool("isSitting", false);
        GameManagerScript.instance.treadmillAC.SetBool("isIdle", false);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManagerScript.instance.samplePadButtonMap['q'].playSample(0f);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameManagerScript.instance.samplePadButtonMap['w'].playSample(0f);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameManagerScript.instance.samplePadButtonMap['e'].playSample(0f);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameManagerScript.instance.samplePadButtonMap['a'].playSample(0f);
        }



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


