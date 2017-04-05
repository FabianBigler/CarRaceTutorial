using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimingBehaviour : MonoBehaviour {
    public Text CountDownText;
    public int CountDown;
    public GameObject Buggy;
    public AudioClip Beep;

    private AudioSource _beepAudioSource;

    void Start()
    {        
        StartCoroutine(GameStart());        
        CountDownText.text = CountDown.ToString();

        _beepAudioSource = gameObject.AddComponent<AudioSource>();
        _beepAudioSource.clip = Beep;
        //_beepAudioSource.loop = true;
        _beepAudioSource.volume = 0.7f;                
    }
        
    IEnumerator GameStart()
    {        
        var counter = 0;
        for (counter = CountDown; counter > 0; counter--)
        {
            yield return new WaitForSeconds(1);
            CountDownText.text = counter.ToString();
            _beepAudioSource.Play();
        }        
        

        yield return new WaitForSeconds(1);
        CountDownText.text = "0";
        _beepAudioSource.pitch = 2.0f;
        _beepAudioSource.Play();
        var buggyBehaviour = Buggy.GetComponent<CarBehaviour>();
        buggyBehaviour.thrustEnabled = true;

        yield return new WaitForSeconds(1);
        CountDownText.text = "";
    }
}
