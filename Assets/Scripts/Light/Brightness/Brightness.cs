using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Brightness : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider s;
    private GameObject light;
    void Start()
    {
        s = GetComponent<Slider>();
        s.onValueChanged.AddListener(OnValueChanged);
        light = GameObject.Find("Directional Light");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnValueChanged(float brightness)
    {
        light.GetComponent<Light>().intensity = brightness * 10;
    }
}
