using UnityEngine;
using TMPro;

public class PointCircleController : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private Rigidbody rb;
    private float lifeTime = 5.0f;

    void Start()
    {
        Destroy(this.gameObject,lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject txt = GameObject.Find("Score");
        m_TextComponent = txt.GetComponent<TMP_Text>();
        m_TextComponent.text = (float.Parse(m_TextComponent.text)+5.0f).ToString();
        Destroy(this.gameObject);
        GameObject.FindObjectOfType<AudioManager>().playSound("CoinSound");
    }
}