using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private State _state;
    [SerializeField]
    private Controller _playerController;
    [SerializeField]
    private GameObject[] _props;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for(int i = 0; i < _props.Length; i++)
        {
            if(_props[i] != null)
            {
                _props[i].SetActive(false);
            }
        }
    }
#endif

    IEnumerator Start()
    {
        for (int i = 0; i < _props.Length; i++)
        {
            Debug.Log("실행중");
            Debug.Log(_props[i] != null);
            bool isTrue = (_props[i] != null);
            Debug.Log("실행중22");
            if (isTrue)
            {
                _props[i].SetActive(true);
            }
          
            //{
            //    Debug.Log("2");
            //    _props[i].SetActive(true);
            //    Debug.Log("3");
            //}
        }
        Debug.Log(_props.Length);
        if (_playerController != null)
        {
            _playerController.Initialize(SetStamina, SetLife);
            _playerController.Revive();
        }
        yield return null;
    }

    public void SetLife(uint current, uint max)
    {
        _state.SetLife(current, max);
    }

    public void SetStamina(float current, float max)
    {
        _state.SetStamina(current, max);
    }
}