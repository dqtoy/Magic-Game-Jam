using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public string[] bossDataJsonFIles;
    List<BossData> bossDatas = new List<BossData>();

    void Start()
    {
        foreach (string json in bossDataJsonFIles) {
            TextAsset jsonfile = Resources.Load(json) as TextAsset;
            Debug.Log(jsonfile.text);

            bossDatas.Add(JsonUtility.FromJson<BossData>(jsonfile.text));
        }

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


[System.Serializable]
public class BossData {
    public int boss;
    public Battle[] fight;
}

[System.Serializable]
public class Battle
{
    public Track[] inputs;
}
[System.Serializable]
public class Track
{
    public string key;
    public float[] data; 
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
