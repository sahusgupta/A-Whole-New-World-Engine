using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Brightness : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider s;
    void Start()
    {
        s = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnValueChange()
    {
        Screen.brightness = s.value;
    }
}
