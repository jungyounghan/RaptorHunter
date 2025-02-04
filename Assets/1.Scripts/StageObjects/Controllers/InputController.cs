using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 특정 플레이어 객체를 조종할 수 있는 입력 컨트롤러 클래스 
/// </summary>
public sealed class InputController : Controller
{
    //오른쪽 또는 위쪽 이동 방향
    private const bool IncreasingDirection = true;
    //왼쪽 또는 아래쪽 이동 방향
    private const bool DecreasingDirection = false;

    [Serializable]
    private struct Input
    {
        public KeyCode[] keyCodes;

        public bool isPressed {
            set;
            get;
        }
    }

    [Header("방향키 ↑"), SerializeField]
    private Input _upInput;
    [Header("방향키 ↓"), SerializeField]
    private Input _downInput;
    [Header("방향키 ←"), SerializeField]
    private Input _leftInput;
    [Header("방향키 →"), SerializeField]
    private Input _rightInput;
    [Header("돌진"), SerializeField]
    private Input _dashInput;
    [Header("공격"), SerializeField]
    private Input _attackInput;
    [Header("도약"), SerializeField]
    private KeyCode[] _jumpKeyCodes;

#if UNITY_EDITOR
    private void OnValidate()
    {
        CheckKey(ref _upInput.keyCodes);
        CheckKey(ref _downInput.keyCodes);
        CheckKey(ref _leftInput.keyCodes);
        CheckKey(ref _rightInput.keyCodes);
        CheckKey(ref _dashInput.keyCodes);
        CheckKey(ref _attackInput.keyCodes);
        CheckKey(ref _jumpKeyCodes);
    }

    private void CheckKey(ref KeyCode[] keyCodes)
    {
        List<KeyCode> list = new List<KeyCode>();
        int length = keyCodes.Length;
        for (int i = 0; i < length; i++)
        {
            KeyCode keyCode = keyCodes[i];
            if (list.Contains(keyCode) == false)
            {
                list.Add(keyCode);
            }
            else if(i == length - 1)
            {
                list.Add(KeyCode.None);
            }
        }
        keyCodes = list.ToArray();
    }

#endif

    private void SetKey(ref Input input)
    {
        foreach (KeyCode keyCode in input.keyCodes)
        {
            if (UnityEngine.Input.GetKey(keyCode) == true)
            {
                input.isPressed = true;
                break;
            }
        }
    }

    private bool GetKey(KeyCode[] keyCodes)
    {
        int length = keyCodes != null ? keyCodes.Length : 0;
        for (int i = 0; i < length; i++)
        {
            if (UnityEngine.Input.GetKeyDown(keyCodes[i]) == true)
            {
                return true;
            }
        }
        return false;
    }

    protected override IEnumerator DoProcess()
    {
        while (true)
        {
            SetKey(ref _upInput);
            SetKey(ref _downInput);
            SetKey(ref _leftInput);
            SetKey(ref _rightInput);
            SetKey(ref _dashInput);
            SetKey(ref _attackInput);
            getCharacter.dash = _dashInput.isPressed;
            if (_upInput.isPressed != _downInput.isPressed)
            {
                switch (_upInput.isPressed)
                {
                    case IncreasingDirection:
                        getCharacter.TryMoveForward();
                        break;
                    case DecreasingDirection:
                        getCharacter.TryMoveBackward();
                        break;
                }
            }
            if (_rightInput.isPressed != _leftInput.isPressed)
            {
                switch (_rightInput.isPressed)
                {
                    case IncreasingDirection:
                        getCharacter.TryTurnRight();
                        break;
                    case DecreasingDirection:
                        getCharacter.TryTurnLeft();
                        break;
                }
            }
            if (GetKey(_jumpKeyCodes) == true)
            {
                getCharacter.TryJump();
            }
            if(_attackInput.isPressed == true)
            {
                getCharacter.TryAttack();
            }
            _upInput.isPressed = false;
            _downInput.isPressed = false;
            _leftInput.isPressed = false;
            _rightInput.isPressed = false;
            _dashInput.isPressed = false;
            _attackInput.isPressed = false;
            yield return null;
        }
    }
}