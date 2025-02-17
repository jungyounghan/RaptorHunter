using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Vector3 _offset;

    private void LateUpdate()
    {
        //if (transform.parent != null)
        //{
        //    Vector3 euler = transform.parent.eulerAngles;
        //    euler += _offset;
        //    transform.rotation = Quaternion.Euler(euler);
        //}
    }
}
