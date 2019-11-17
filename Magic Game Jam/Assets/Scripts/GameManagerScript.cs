using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.LWRP;

public class GameManagerScript : ByTheTale.StateMachine.MachineBehaviour
{
    // Start is called before the first frame update

    public GameObject player;
    public GameObject treadmill;
    public GameObject userSamplePad;
    public GameObject frogSamplePad;
    public GameObject magicSpell;

    public Animator playerAC;
    public Animator treadmillAC;


    public string[] bossDataJsonFIles;
    public List<BossData> bossDatas = new List<BossData>();
    public TMP_Text currentStateText;
    public TMP_Text measureDebug;
    public TMP_Text timeInBattleDebug;

    public int currentBossNumber = 0;
    public int currentBattleNumber = 0;
    public float bpm = 120f;
    public int measuresInLoop = 1;
    public float timePerMeasure;
    public float timeInMeasure = 0f;
    public float timeAtBeginningOfLastMeasure = 0f;
    public GameObject tempoHelper;
    
    public List<prepBossAudio> currentBossAudioSources = new List<prepBossAudio>();


    //everything is either looping on 4, 8, 16, etc

    public static GameManagerScript instance;
    public int measuresSoFar = 0;
    public Light2D tempoLight;
    //these are important :-)
    public Dictionary<char, samplePadButtonScript> samplePadButtonMap = new Dictionary<char, samplePadButtonScript>();

    //list of 
    public List<Dictionary<char, float[]>> currentFightData = new List<Dictionary<char, float[]>>();

    //public Dictionary<char, float> loopingSamplesDictionary = new Dictionary<char, float>();
    //to keep track of everything looping, I could just make a list of looping


    public HashSet<char> loopingSamples = new HashSet<char>();


    public List<int> currentBattleMeasureLengths = new List<int>();

    AudioSource tempoAudio;

    public GameObject[] enemiesToSpawn;
    public GameObject currentEnemy;

    public char[] beatUnlockOrder = { 'q', 'w', 'e', 'a' };
    public char[] trackUnlockOrder = { 's', 'd', 'z', 'x', 's' };


    private void Awake()
    {
        instance = this;
    }

    public override void AddStates()
    {
        AddState<IdleState>();
        AddState<EnemyBattleState>();
        AddState<PlayerBattleState>();
        AddState<PlayerConfirmState>();
        AddState<GameWonState>();
        AddState<LearnedNewSampleState>();

        SetInitialState<IdleState>();
    }

    public void spawnSpell(Vector3 pos, float delay) {
        StartCoroutine(spawnSpellCoroutine(delay, pos));
    }

    public IEnumerator spawnSpellCoroutine(float delay, Vector3 pos) {

        yield return new WaitForSeconds(delay);
        GameObject spellSpawned = Instantiate(magicSpell);
        spellSpawned.transform.position = pos;
        Destroy(spellSpawned, 0.5f);
    }

    public override void Start()
    {

        base.Start();
        //16 measures
        tempoAudio = GetComponent<AudioSource>();
        tempoAudio.Play();
        //this is say there
        //bpm/60 = beats per second
        //there are 1/(beats per second) = seconds per beat
        //there are 4 beats per measure so 4 * seconds per beat = seconds 4 beats take = beats in a measure
        tempoLight.gameObject.SetActive(false);
        timePerMeasure = (1/(bpm/60)) * 4f;
        timeAtBeginningOfLastMeasure = Time.time;
        foreach (string json in bossDataJsonFIles) {
            TextAsset jsonfile = Resources.Load(json) as TextAsset;
            Debug.Log(jsonfile.text);

            bossDatas.Add(JsonUtility.FromJson<BossData>(jsonfile.text));
        }
        beginningOfMeasure();
        foreach (BossData bd in bossDatas) {
            foreach (Battle battle in bd.fight) {
                foreach (Track t in battle.inputs) {
                    Debug.Log("key to be pressed: " + t.key);
                    Debug.Log("at times : ");
                    foreach (float time in t.data) {
                        Debug.Log("\t" + time);
                    }

                        

                }
            }
        }
        playerAC = player.GetComponent<Animator>();
        treadmillAC = treadmill.GetComponent<Animator>();


        setNewBoss();
        //StartCoroutine(spaceMovement(player));
        StartCoroutine(spaceMovement(treadmill));
        StartCoroutine(spaceMovement(frogSamplePad));
        StartCoroutine(spaceMovement(userSamplePad));
        //queue up next boss battle
        //could create all of them all at once
        clearPrevBattleAudioSources();

        queueUpAudioSourcesForNextBattle();

    }


    public IEnumerator spaceMovement(GameObject go)
    {
        float randomOffset = UnityEngine.Random.value * 2*Mathf.PI;
        float theta = 0;
        while (true)
        {
            yield return new WaitForSeconds(1f/60f);
            theta += 0.04f;
            go.transform.localPosition += new Vector3(0f, Mathf.Sin(theta + randomOffset)*0.03f, 0f);
        }
    }


    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        currentStateText.text = currentState.ToString();
        timeInMeasure += Time.deltaTime;
        
        if (timeInMeasure >= timePerMeasure)
        {
            timeInMeasure = 0;
            beginningOfMeasure();
        }
        measureDebug.text = "boss: " + currentBossNumber + "battle: " + currentBattleNumber;

        /*
        foreach sample in big list of samples
            if(nummeasures so far % num num measures the loop occupies = 0){
                stop sample, play sample
                play the sample
            }


         */
    }

    public override void FixedUpdate()
    {
        

    }

    public void beginningOfMeasure() {
        //set new listen times here based on the current boss
        timeAtBeginningOfLastMeasure = Time.time;
        tempoLight.gameObject.SetActive(true);
        StartCoroutine(turnLightOff());
        measuresSoFar++;


       


    }

    public IEnumerator turnLightOff() {
        yield return new WaitForSeconds(0.1f);
        tempoLight.gameObject.SetActive(false);
    }


    //puts the battles into a data structure that has a list of battles
    //each battle is a dictionary of char to float[], the char being the
    //key included in the battle, and the float[] being all the times that is pressed
    public void setNewBoss() {

        currentFightData = new List<Dictionary<char, float[]>>();
        if (currentBossNumber >= bossDatas.Count) {
            Debug.Log("game over");
        }

        BossData boss = bossDatas[currentBossNumber];
        currentBattleMeasureLengths = new List<int>();
        foreach (Battle battle in boss.fight) {
            //each battle is a dictionary
            //battle is a dictionary mapping the char to the corresponding times it is hit
            Dictionary<char, float[]> newBattle = new Dictionary<char, float[]>();
            foreach (Track track in battle.inputs) {
                try
                {
                    newBattle[track.key[0]] = track.data;

                }
                catch (Exception e) {
                    Debug.Log("cannot get key");
                }
                
            }
            currentBattleMeasureLengths.Add(battle.numMeasures);
            currentFightData.Add(newBattle);
        }

    }

    // this should be called when you press a button, it should check if you hit a
    // button within a threshold

    

    //public void handleSamplePressed(char key) {

    //    //checks current beginning
    //    //time at last measure

    //    float[] timesToCheck = currentFightData[currentBattleNumber][key];
    //    float currentMeasureTime = Time.time - timeAtBeginningOfLastMeasure;
    //    foreach (float t in timesToCheck) {
    //        Debug.Log(key + " was off by " + (currentMeasureTime - t));
    //        Debug.Log("target time: " + t + ", time hit: " +currentMeasureTime);
    //    }
        
    //}

    public void clearPrevBattleAudioSources()
    {
        foreach (prepBossAudio preppedAudioSource in currentBossAudioSources)
        {
            Destroy(preppedAudioSource.audioSource.gameObject);
        }
        currentBossAudioSources.Clear();
    }

    public void queueUpAudioSourcesForNextBattle()
    {
        


        Dictionary<char, float[]> beatDict = currentFightData[currentBattleNumber];

        foreach (char c in beatDict.Keys)
        {
            if (!currentFightData[currentBattleNumber].ContainsKey(c)) {
                continue;
            }
            foreach (float sampleTime in currentFightData[currentBattleNumber][c])
            {
                AudioSource newAudioSource = new GameObject().AddComponent<AudioSource>();
                newAudioSource.clip = samplePadButtonMap[c].sample;
                prepBossAudio newPrep = new prepBossAudio(newAudioSource, instance.timePerMeasure * sampleTime, c);
                currentBossAudioSources.Add(newPrep);

            }
        }


    }

    public GameObject spawnEnemy() {
        return currentEnemy = Instantiate(enemiesToSpawn[currentBossNumber], treadmill.transform);
    }

    public void destroyCurrentEnemy()
    {
        spawnSpell(currentEnemy.transform.position, 0f);
        currentEnemy.transform.DOLocalMove(new Vector3(50, 0f, 0f), 1f);
        Destroy(currentEnemy, 1f);
    }


}

//JSON helpers
[System.Serializable]
public class BossData {
    public string boss;
    public Battle[] fight;
}


//each battle is a list of inputs and their corresponding times
[System.Serializable]
public class Battle
{
    public int numMeasures;
    public Track[] inputs;
}
[System.Serializable]
public class Track
{
    public string key;
    public float[] data; 
}


public struct prepBossAudio
{
    public AudioSource audioSource;
    public float delay;
    public char assocChar;

    public prepBossAudio(AudioSource _audioSource, float _delay, char _assocChar)
    {
        audioSource = _audioSource;
        delay = _delay;
        assocChar = _assocChar;
    }
}

/*



    example JSON File

    {
        boss: 0,
        tracks: [
            {
            "key": "q",
            "data": [0.25, 0.3, 0.6, 0.9]
            }
        ]
    }
 */
