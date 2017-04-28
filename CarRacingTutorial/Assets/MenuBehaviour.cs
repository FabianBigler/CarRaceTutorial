using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour {
    public Slider HueSlider;
    public Slider SaturationSlider;
    public Slider LuminanceSlider;
    public Image PreviewColour;
    public GameObject Buggy;


    public void onStartClick()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void Start()
    {
        HueSlider.onValueChanged.AddListener(delegate { SliderValueChanged(); });
        SaturationSlider.onValueChanged.AddListener(delegate { SliderValueChanged(); });
        LuminanceSlider.onValueChanged.AddListener(delegate { SliderValueChanged(); });                
        SliderValueChanged();                
    }


    private void SliderValueChanged()
    {      
        var color = Color.HSVToRGB(HueSlider.value, SaturationSlider.value, LuminanceSlider.value);        
        //Buggy.GetComponentInChildren<Renderer>().material.color = color;
        BuggyConfiguration.BodyColor = color;
        PreviewColour.color = color;
    }      
}
