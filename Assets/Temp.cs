using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Temp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(GameObject.Find("Hunter").transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
