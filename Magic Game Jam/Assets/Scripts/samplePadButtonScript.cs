using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class samplePadButtonScript : MonoBehaviour
{
    // Start is called before the first frame update

    public char assocKey;
    public Sprite buttonUpSprite;
    public Sprite buttonDownSprite;
    public AudioClip sample;
    public AudioSource audiosrc;
    Light2D light;
    public bool playsOnBeat = false;
    public int numMeasures;
    public bool looping = false;

    void Start()
    {
        light = GetComponentInChildren<Light2D>();
        GameManagerScript.instance.samplePadButtonMap[assocKey] = this;
        audiosrc = gameObject.AddComponent<AudioSource>();
        audiosrc.clip = sample;
        light.gameObject.SetActive(false);
        //audiosrc.loop = looping;
    }

    // Update is called once per frame
    float timePlaying =0;
    void Update()
    {
        if (audiosrc.isPlaying && looping) {
            timePlaying += Time.deltaTime;
            if (timePlaying % audiosrc.clip.length < 0.1f)
            {
                animateButton(0f);
            }
            
        }
        else
        {
            timePlaying = 0f;
        }

    }
    public void animateButton(float delay) {
        
        StartCoroutine(startButtonAnimation(delay));
    }

    public IEnumerator startButtonAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(makeButtonFinish());
        light.gameObject.SetActive(true);



    }
    public IEnumerator makeButtonFinish() {
        yield return new WaitForSeconds(0.1f);
        light.gameObject.SetActive(false);


    }
    public void playSample(float delay) {
        audiosrc.Stop();
        float timeUntilNextMeasure = GameManagerScript.instance.timePerMeasure - GameManagerScript.instance.timeInMeasure;



        if (playsOnBeat) {
            audiosrc.PlayScheduled(AudioSettings.dspTime + delay + timeUntilNextMeasure);
        }
        else {
            audiosrc.PlayScheduled(AudioSettings.dspTime + delay);
        }
        audiosrc.PlayScheduled(AudioSettings.dspTime + delay);
        animateButton(0f);
        //StartCoroutine(destroySound(sample.length, newAudioSource.gameObject));

        //StartCoroutine(spawnSound(delay));
    }

    public IEnumerator destroySound(float time, GameObject objectToDestroy) {

        yield return new WaitForSeconds(time);

        Destroy(objectToDestroy);
       
        
    }

    

    
}
