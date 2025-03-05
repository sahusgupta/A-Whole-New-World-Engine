using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Config : MonoBehaviour
{
    private GameObject manager;
    private AudioSource audioS;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager");
        audioS = manager.GetComponent<AudioSource>();
        gameObject.GetComponent<Slider>().onValueChanged.AddListener(delegate { SetVolume(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SetVolume()
    {
        audioS.volume = gameObject.GetComponent<Slider>().value;
    }
}
