using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleMenu : MonoBehaviour
{
    [SerializeField] int toggle;

    [SerializeField] private GameObject[] menus = new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { ToggleView(toggle); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleView(int r)
    {

        for (int i = 0; i < menus.Length; i++)
        {
            if (i == r)
            {
                menus[i].SetActive(true);
            } else
            {
                menus[i].SetActive(false);
            }

        }

    }
}
