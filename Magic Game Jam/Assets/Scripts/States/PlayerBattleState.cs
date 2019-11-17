using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerBattleState : ByTheTale.StateMachine.State
{
    public GameManagerScript exampleCharacter { get { return (GameManagerScript)machine; } }
    // Use this for initialization

    //set a timeout of the time to the next measure
    float timeUntilNextMeasure;
    float timeInBattle = 0f;
    bool startedPuzzle = false;
    bool battleFinished = false;
    Dictionary<char, List<float>> battleInfo = new Dictionary<char, List<float>>();

    public override void Enter()
    {

        base.Enter();
        timeInBattle = 0f;
        timeUntilNextMeasure = GameManagerScript.instance.timePerMeasure - GameManagerScript.instance.timeInMeasure;
        battleInfo = new Dictionary<char, List<float>>();
        //GameManagerScript.instance.currentBattleNumber++;
        if (GameManagerScript.instance.currentBattleNumber >= GameManagerScript.instance.currentFightData.Count) {
            GameManagerScript.instance.currentBattleNumber = 0;
            machine.ChangeState<IdleState>();
            return;
        }

        //make the audio for the next battle
        

    }
    bool hasStartedPuzzle = false;
    public override void Execute()
    {
        base.Execute();
        
        if (timeInBattle > (GameManagerScript.instance.timePerMeasure * GameManagerScript.instance.currentBattleMeasureLengths[GameManagerScript.instance.currentBattleNumber]) + wiggleRoom)
        {
            battleFinished = true;
            if (parseBattle())
            {
                

                GameManagerScript.instance.currentBattleNumber++;
                GameManagerScript.instance.timeInBattleDebug.text = "success";
                foreach (char c in battleInfo.Keys) {
                    if (c == 'z' || c == 'x' || c == 'c' || c == 'd') {
                        GameManagerScript.instance.loopingSamples.Add(c);
                    }
                }

                
                if (GameManagerScript.instance.currentBattleNumber >= GameManagerScript.instance.currentFightData.Count)
                {
                    
                    //change the boss and prep the first battle
                    GameManagerScript.instance.currentBattleNumber = 0;
                    GameManagerScript.instance.currentBossNumber += 1;
                    if (GameManagerScript.instance.currentBossNumber >= GameManagerScript.instance.bossDatas.Count) {
                        machine.ChangeState<GameWonState>();
                        return;
                    }
                    GameManagerScript.instance.setNewBoss();

                    //queue up next boss battle
                    //could create all of them all at once
                    GameManagerScript.instance.clearPrevBattleAudioSources();

                    GameManagerScript.instance.queueUpAudioSourcesForNextBattle();
                    machine.ChangeState<IdleState>();

                    return;
                }
                //GameManagerScript.instance.loopingSamples.Add()

                //prep the next battle if they won
                GameManagerScript.instance.clearPrevBattleAudioSources();
                GameManagerScript.instance.queueUpAudioSourcesForNextBattle();


                machine.ChangeState<EnemyBattleState>();
                


            }
            else {
                machine.ChangeState<PlayerConfirmState>();
                GameManagerScript.instance.timeInBattleDebug.text = "failure";
            }

            
            return;
        }
        GameManagerScript.instance.timeInBattleDebug.text = timeInBattle.ToString();
        if (hasStartedPuzzle != startedPuzzle) {
            Debug.Log("puzzle started");
        }
        if (startedPuzzle) {
            timeInBattle += Time.deltaTime;
        }

        handleBattleInput(timeInBattle);


        hasStartedPuzzle = startedPuzzle;
    }

    public override void Exit()
    {
        base.Exit();
        timeInBattle = 0;
        startedPuzzle = false;
        battleFinished = false;
        

    }

    float wiggleRoom = 1f;
    public void handleBattleInput(float time) {
        
        if (battleFinished) {

            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            handleButtonPress('q', time);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            handleButtonPress('w', time);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            handleButtonPress('e', time);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            handleButtonPress('a', time);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            handleButtonPress('x', time);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            handleButtonPress('z', time);
        }
    }
    public void handleButtonPress(char key, float time)
    {
        startedPuzzle = true;
        if (!battleInfo.ContainsKey(key)) {
            battleInfo[key] = new List<float>();
        }
        battleInfo[key].Add(time);
        GameManagerScript.instance.samplePadButtonMap[key].playSample(0f);
        
        
    }

    public bool parseBattle() {
        //check to see if all of the keys were used
        //current boss data is a 
        Dictionary<char, float[]> currentBossData = GameManagerScript.instance.currentFightData[GameManagerScript.instance.currentBattleNumber];
        
        
        
        float delta = 0;
        int totalNotes = 0;
        //check to see all the times
        //loop through each boss key and check against the battle
        foreach (char key in currentBossData.Keys) {
            if (currentBossData[key].Length == 0) {
                continue;
            }
            //check to see if you have the right amount of keys
            if (!battleInfo.ContainsKey(key)) {
                Debug.Log("Player did not press key: " + key);
                return false;
            }
            if (currentBossData[key].Length != battleInfo[key].ToArray().Length) {
                Debug.Log("key: " + key +" was not pressed enough times");
                return false;
            }
            totalNotes += currentBossData[key].Length;
            for (int i = 0; i < battleInfo[key].Count; i++) {

                float missAmount = Mathf.Abs(battleInfo[key][i] - GameManagerScript.instance.timePerMeasure * currentBossData[key][i]);
                delta += missAmount;
                Debug.Log("during battle hit at " + battleInfo[key][i]);
                Debug.Log("boss hit was " + GameManagerScript.instance.timePerMeasure * currentBossData[key][i]);
                Debug.Log("miss amount: " + missAmount);
            }
        }

        Debug.Log("delta was " + delta);
        if (delta > totalNotes * 0.15f) {
            return false;
        }


        return true;


    }
    //spawn the pattern based on the current battle number


}
