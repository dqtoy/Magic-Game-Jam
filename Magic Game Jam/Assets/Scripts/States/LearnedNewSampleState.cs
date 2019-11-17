using UnityEngine;
using System.Collections;

public class LearnedNewSampleState : ByTheTale.StateMachine.State
{
    public GameManagerScript exampleCharacter { get { return (GameManagerScript)machine; } }
    char unlockedKey;
    bool isDrum = false;
    public override void Enter()
    {
        base.Enter();
        //beting the boss unlocked
        //drum pad key
        isDrum = false;

        if (GameManagerScript.instance.currentBossNumber % 2 == 0)
        {
            isDrum = true;

            //spawn magic at key
            unlockedKey = GameManagerScript.instance.beatUnlockOrder[GameManagerScript.instance.currentBossNumber / 2];
            GameManagerScript.instance.samplePadButtonMap[unlockedKey].baseColor = Color.cyan;

        }
        else {
            unlockedKey = GameManagerScript.instance.trackUnlockOrder[(int)Mathf.Floor(GameManagerScript.instance.currentBossNumber / 2)];
            GameManagerScript.instance.samplePadButtonMap[unlockedKey].baseColor = Color.green;

        }
        Debug.Log("learnerd new sample " + unlockedKey);
    }

    public override void Execute()
    {
        base.Execute();

        KeyCode keycode = KeyCode.Q;
        if (unlockedKey == 'q') {
            keycode = KeyCode.Q;
        }
        else if (unlockedKey == 'w')
        {
            keycode = KeyCode.W;
        }
        else if (unlockedKey == 'e')
        {
            keycode = KeyCode.E;
        }
        else if (unlockedKey == 'a')
        {
            keycode = KeyCode.A;
        }
        else if (unlockedKey == 's')
        {
            keycode = KeyCode.S;
        }
        else if (unlockedKey == 'd')
        {
            keycode = KeyCode.D;
        }
        else if (unlockedKey == 'z')
        {
            keycode = KeyCode.Z;
        }
        else if (unlockedKey == 'x')
        {
            keycode = KeyCode.X;
        }
        else if (unlockedKey == 'c')
        {
            keycode = KeyCode.C;
        }

        if (Input.GetKeyDown(keycode)) {
            if (isDrum)
            {
                GameManagerScript.instance.samplePadButtonMap[unlockedKey].playSample(0f);
            }
            else {
                float timeLeftInMeasure = GameManagerScript.instance.timePerMeasure - GameManagerScript.instance.timeInMeasure;
                GameManagerScript.instance.samplePadButtonMap[unlockedKey].audiosrc.loop = true;
                GameManagerScript.instance.samplePadButtonMap[unlockedKey].playSample(timeLeftInMeasure);
                
            }
            machine.ChangeState<IdleState>();
            

        }

    }
}

