using UnityEngine;
using System.Collections;

public class GameWonState : ByTheTale.StateMachine.State
{
    public GameManagerScript exampleCharacter { get { return (GameManagerScript)machine; } }

    public override void Enter()
    {
        base.Enter();
        GameManagerScript.instance.playerAC.SetBool("isSitting", false);
        GameManagerScript.instance.treadmillAC.SetBool("isIdle", false);

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

