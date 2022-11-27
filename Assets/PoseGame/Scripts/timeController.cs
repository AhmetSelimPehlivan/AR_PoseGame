using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class timeController : MonoBehaviour
{
    private float counter;
    private Slider timer;
    public TMP_Text text;
    // Start is called before the first frame update
    private void Awake(){
        timer = GameObject.Find("Timer").GetComponent<Slider>();
    }

    void Start()
    {
        timer.maxValue = 60;
        timer.minValue = 0;
        timer.wholeNumbers = false;
        timer.value = timer.maxValue;
        counter = timer.value;
    }

    // Update is called once per frame
    void Update()
    {
        counter = timer.value - Time.deltaTime;
        if(timer.value > timer.minValue){
            GameObject txt = GameObject.Find("TimeText");
            text = txt.GetComponent<TMP_Text>();
            timer.value = counter;
            text.text = ((int)timer.value).ToString();
        }
    }
}
