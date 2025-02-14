using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �þ߿� ���̰ų� ������ �ʴ� ���������� ���� ī�޶� �˷��ִ� Ŭ����
/// </summary>
[RequireComponent(typeof(Renderer))]
[DisallowMultipleComponent]
public class MaterialSender : MonoBehaviour
{
    private bool _hasRenderer = false;

    private Renderer _renderer = null;

    private Renderer getRenderer {
        get
        {
            if(_hasRenderer == false)
            {
                _hasRenderer = TryGetComponent(out _renderer);
            }
            return _renderer;
        }
    }

    private Action<IEnumerable<Material>> _showAction = null;

    private Action<IEnumerable<Material>> _hideAction = null;

    private void OnBecameVisible()
    {
        _showAction?.Invoke(getRenderer.materials);
    }

    private void OnBecameInvisible()
    {
        _hideAction?.Invoke(getRenderer.materials);
    }

    public void Initialize(Action<IEnumerable<Material>> showAction, Action<IEnumerable<Material>> hideAction)
    {
        _showAction = showAction;
        _hideAction = hideAction;
    }
}