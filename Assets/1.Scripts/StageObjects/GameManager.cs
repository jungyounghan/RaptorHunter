using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Test test;


    void Start()
    {
        if(test != null)
        {
            Caster[] casters = (Caster[])FindObjectsOfType(typeof(Caster));
            foreach(Caster caster in casters)
            {
                caster.Initialize(test.Add, test.Remove);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
