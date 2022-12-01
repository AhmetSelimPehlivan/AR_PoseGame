using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeCircleController : MonoBehaviour
{
    private Slider timer;
    private TMP_Text text;
    private Rigidbody rb;
    public float lifeTime = 3.0f;

    private void Awake(){
        timer = GameObject.Find("Timer").GetComponent<Slider>();
    }
    void Start()
    {
        Destroy(this.gameObject,lifeTime);
    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject txt = GameObject.Find("TimeText");
        text = txt.GetComponent<TMP_Text>();
        timer.value += 10;
        text.text = ((int)timer.value).ToString();
        Destroy(this.gameObject);
        GameObject.FindObjectOfType<AudioManager>().playSound("TimeSound");
    }
}
