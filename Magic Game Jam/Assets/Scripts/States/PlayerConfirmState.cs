﻿using UnityEngine;
using System.Collections;

public class PlayerConfirmState : ByTheTale.StateMachine.State
{
    public GameManagerScript exampleCharacter { get { return (GameManagerScript)machine; } }

    public override void Enter()
    {
        base.Enter();

    }

    public override void Execute()
    {
        base.Execute();
        if (Input.GetKeyDown(KeyCode.Y)) {
            machine.ChangeState<PlayerBattleState>();
            
            //GameManagerScript.instance.currentBattleNumber++;
            //GameManagerScript.instance.queueUpAudioSourcesForNextBattle();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            machine.ChangeState<EnemyBattleState>();
            //GameManagerScript.instance.queueUpAudioSourcesForNextBattle();
        }



    }
}

