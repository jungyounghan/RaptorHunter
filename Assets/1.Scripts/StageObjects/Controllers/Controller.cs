using System.Collections;
using UnityEngine;

/// <summary>
/// Ư�� �÷��̾� ��ü�� ������ �� �ִ� �߻� ��Ʈ�ѷ� Ŭ���� 
/// </summary>
[RequireComponent(typeof(Character))]
[DisallowMultipleComponent]
public abstract class Controller : MonoBehaviour
{
    private bool _hasCharacter = false;

    private Character _character = null;

    protected Character getCharacter {
        get
        {
            if (_hasCharacter == false)
            {
                _hasCharacter = true;
                _character = GetComponent<Character>();
            }
            return _character;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(DoProcess());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected abstract IEnumerator DoProcess();
}