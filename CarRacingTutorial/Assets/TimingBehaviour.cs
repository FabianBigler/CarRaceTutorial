using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimingBehaviour : MonoBehaviour {
    public Text CountDownText;
    public Text TimerText;
    public Text InfoText;

    public int CountDown;
    public GameObject Buggy;
    public AudioClip Beep;
    

    private AudioSource _beepAudioSource;
    private float _pastTime = 0;

    private bool _isFinished = false;
    private bool _isStarted = false;
    private bool _checkPoint2Reached = false;
    private bool _checkPoint3Reached = false;
    private bool _checkPoint4Reached = false;

    void Start()
    {
        StartCoroutine(GameStart());
        CountDownText.text = CountDown.ToString();
        _beepAudioSource = gameObject.AddComponent<AudioSource>();
        _beepAudioSource.clip = Beep;
        //_beepAudioSource.loop = true;
        _beepAudioSource.volume = 0.7f;

    }

    void Update()
    {
        try
        {
            var buggyBehaviour = Buggy.GetComponent<CarBehaviour>();
            if (buggyBehaviour.thrustEnabled)
            {
                if (_isStarted && !_isFinished)
                    _pastTime += Time.deltaTime;
                TimerText.text = _pastTime.ToString("0.0 sec");
            }        
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.ToString());
            
        }
    }


    public void ChildTriggerEntered(string tag)
    {
        switch(tag)
        {
            case "Goal1":
                if(_checkPoint2Reached && _checkPoint3Reached && _checkPoint4Reached && _isStarted)
                {
                    _isFinished = true;
                    InfoText.text = "Round finished!";
                }

                _isStarted = true;
                break;
            case "Goal2":
                if (_isStarted)
                {
                    _checkPoint2Reached = true;
                    InfoText.text = "Brilliant! Checkpoint 2 reached!";
                } else
                {
                    InfoText.text = "Oh no, you skipped checkpoint 1!";
                }                
                break;
            case "Goal3":
                if (_isStarted && _checkPoint2Reached)
                {
                    _checkPoint3Reached = true;
                    InfoText.text = "WOW, Checkpoint 3 reached!";
                }
                else
                {
                    InfoText.text = "Shortcuts are not allowed, mate!";
                }
                break;
            case "Goal4":
                if (_isStarted && _checkPoint2Reached && _checkPoint3Reached)
                {
                    _checkPoint4Reached = true;
                    InfoText.text = "Almost finished! GO GO GO!";
                }
                else
                {
                    InfoText.text = "Too bad, you skipped a check point.";
                }
                break;
        }

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
