using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������̺� ��� �޴� Ư�� Ŭ���� ������Ʈ���� ��ȯ���ִ� Ŭ����
/// </summary>
/// <typeparam name="T"></typeparam>
[Serializable]
public class Spawner<T> where T: MonoBehaviour
{
    [Serializable]
    public struct Handle
    {
        public T unit;
        public float duration;
        public Vector3 position;
        public Vector3 rotation;
        public Transform parent;
    }

    [Header("���� ���"), SerializeField]
    private List<Handle> handles = new List<Handle>();

    private int index = 0;
    private float timer = 0;

    private Dictionary<T, T> dictionary = new Dictionary<T, T>();
    private Action<T> action = null;

    [Header("�Ͻ� ���� ����"), SerializeField]
    private bool pause = false;
    [Header("�ݺ� ����"), SerializeField]
    private bool loop = false;

    public void Set(Action<T> action)
    {
        this.action = action;
    }

    public void Update()
    {
        if (pause == false)
        {
            int count = handles.Count;
            if (index >= count)
            {
                if(loop == true)
                {
                    return;
                }
                index = 0;
            }
            if (count > 0)
            {
                if (timer < handles[index].duration)
                {
                    timer += Time.deltaTime;
                }
                if (timer >= handles[index].duration)
                {
                    T t = Get(handles[index].unit, handles[index].position, Quaternion.Euler(handles[index].rotation), handles[index].parent);
                    timer = 0;
                    index++;
                    action?.Invoke(t);
                }
            }
        }
    }

    public void Pause()
    {
        pause = true;
    }

    public void Stop()
    {
        pause = true;
        timer = 0;
        index = 0;
    }

    public void Play()
    {
        pause = false;
    }

    public void Add(Handle handle)
    {
        handles.Add(handle);
    }

    public T Get()
    {
        int count = handles.Count;
        if (index >= count)
        {
            index = 0;
        }
        if (count > 0)
        {
            T t = Get(handles[index].unit, handles[index].position, Quaternion.Euler(handles[index].rotation), handles[index].parent);
            timer = 0;
            index++;
            return t;
        }
        return null;
    }

    public T Get(T prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (prefab != null)
        {
            //��Ȱ��
            foreach(KeyValuePair<T, T>kvp in dictionary)
            {
                GameObject gameObject = kvp.Key.gameObject;
                if (prefab == kvp.Value && gameObject.activeInHierarchy == false)
                {
                    Transform transform = gameObject.transform;
                    transform.position = position;
                    transform.rotation = rotation;
                    transform.parent = parent;
                    gameObject.SetActive(true);
                    return kvp.Key;
                }
            }
            //�߰� ����
            T t = GameObject.Instantiate(prefab, position, rotation, parent);
            dictionary.Add(t, prefab);
            return t;
        }
        return null;
    }
}