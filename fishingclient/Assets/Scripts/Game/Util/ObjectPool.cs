using System;
using System.Collections.Generic;
using UnityEngine;
public class ObjectPool<T>
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly Func<int, T> m_Creator;
    private readonly Action<T> m_Destroyer;
    private readonly Action<T> m_ActionOnGet;
    private readonly Action<T> m_ActionOnRelease;
    private int m_type;
    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }
    public ObjectPool(Func<int, T> creator, int type, Action<T> destroyer = null
                            , Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
    {
        Debug.Assert(creator != null);
        m_Creator = creator;
        m_Destroyer = destroyer;
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
        m_type = type;
    }
    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = m_Creator(m_type);
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }
    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
    /// Prepare some objects
    public void Prepare(int count)
    {
        for (int i = 0; i < count; ++i)
        {
            T element = m_Creator(m_type);
            countAll++;
            m_Stack.Push(element);
        }
    }
    /// Destroy all inactive objects. 
    public void Clear()
    {
        int nInactiveCount = countInactive;
        countAll -= nInactiveCount;
        for (int i = 0; i < nInactiveCount; ++i)
        {
            var obj = Get();
            if (m_Destroyer != null) m_Destroyer(obj);
        }
    }
}
