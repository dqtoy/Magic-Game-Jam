using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class EnemyBattleState : ByTheTale.StateMachine.State
{
    public GameManagerScript gameManager { get { return (GameManagerScript)machine; } }
    private float timeInBattle;
    float timeLeftInFrame;
    // Use this for initialization
    public override void Enter()
    {
        base.Enter();
        spawnEnemies();
        //wait until the beginning of a beat to play this
        timeLeftInFrame = GameManagerScript.instance.timePerMeasure - GameManagerScript.instance.timeInMeasure;
        generateEnemyBeat();
        GameManagerScript.instance.playerAC.SetBool("isSitting", true);
        GameManagerScript.instance.treadmillAC.SetBool("isIdle", true);


        if (GameManagerScript.instance.currentEnemy == null) {
            GameObject spawnedEnemy = GameManagerScript.instance.spawnEnemy();
            spawnedEnemy.transform.localPosition = new Vector3(Random.value*40f -20f, -20f, 0);
            spawnedEnemy.transform.DOLocalMove(new Vector3(7f, 2f, 0f), 1f);
        }
    }
    float wiggleroom = 0.5f;
    public override void Execute()
    {
        base.Execute();

        timeInBattle += Time.deltaTime;

        if (timeInBattle + wiggleroom > timeLeftInFrame + (GameManagerScript.instance.timePerMeasure * GameManagerScript.instance.currentBattleMeasureLengths[GameManagerScript.instance.currentBattleNumber])) {
            machine.ChangeState<PlayerBattleState>();
        }


        

    }

    public override void Exit()
    {
        base.Exit();
        
        timeInBattle = 0;
    }


    //spawn the pattern based on the current battle number

    public void spawnEnemies() {

        Debug.Log(GameManagerScript.instance.bossDatas[GameManagerScript.instance.currentBossNumber]);
    }

    //generate single battle beat
    public void generateEnemyBeat() {
        foreach (prepBossAudio preppedAudio in GameManagerScript.instance.currentBossAudioSources) {
            preppedAudio.audioSource.Stop();
            
            preppedAudio.audioSource.PlayScheduled(AudioSettings.dspTime + preppedAudio.delay + timeLeftInFrame);
            GameManagerScript.instance.samplePadButtonMap[preppedAudio.assocChar].animateButton(preppedAudio.delay + timeLeftInFrame);
        }
    }

    

    
}
