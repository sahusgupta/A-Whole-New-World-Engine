using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class IOManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void WriteToFile(string path, List<string> content)
    {
        File.WriteAllLines(path, content);
    }

    public Dictionary<string, dynamic> GetAttrs(string path) {
        Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();
        string[] attrs = File.ReadAllLines(path);
        foreach (string attr in attrs) {
            string[] kv = attr.Split(':');
            dictionary.Add(kv[0], kv[1]);
        }
        return dictionary;
    }


}
