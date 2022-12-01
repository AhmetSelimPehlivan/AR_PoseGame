using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombCircleController : MonoBehaviour
{
    [SerializeField] private GameObject _particles;
    private TMP_Text m_TextComponent;
    private Rigidbody rb;
    private float lifeTime = 5.0f;
    
    void Start()
    {
        Debug.Log("BombCircleController");
        Destroy(this.gameObject,lifeTime);
    }
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject txt = GameObject.Find("Score");
        m_TextComponent = txt.GetComponent<TMP_Text>();
        m_TextComponent.text = (Mathf.Max(float.Parse(m_TextComponent.text)-10.0f,0.0f)).ToString();
        GameObject explosion = Instantiate(_particles, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
        GameObject.FindObjectOfType<AudioManager>().playSound("BombSound");
        Destroy(explosion, 2.0f);
        
    }
}