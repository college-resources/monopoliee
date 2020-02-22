using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

// https://perma.cc/B83G-YYZM

public class CoroutineQueue
{
    private Coroutine _mInternalCoroutine;
    private readonly MonoBehaviour _mOwner;
    private readonly Queue<IEnumerator> _actions = new Queue<IEnumerator>();
    
    public CoroutineQueue(MonoBehaviour aCoroutineOwner)
    {
        _mOwner = aCoroutineOwner;
    }
    
    public void StartLoop()
    {
        _mInternalCoroutine = _mOwner.StartCoroutine(Process());
    }
    
    public void StopLoop()
    {
        _mOwner.StopCoroutine(_mInternalCoroutine);
        _mInternalCoroutine = null;
    }
    
    public void EnqueueAction(IEnumerator aAction)
    {
        _actions.Enqueue(aAction);
    }
 
    private IEnumerator Process()
    {
        while (true)
        {
            if (_actions.Count > 0)
                yield return _mOwner.StartCoroutine(_actions.Dequeue());
            else
                yield return null;
        }
    }
    
    public void EnqueueWait(float aWaitTime)
    {
        _actions.Enqueue(Wait(aWaitTime));
    }
 
    private static IEnumerator Wait(float aWaitTime)
    {
        yield return new WaitForSeconds(aWaitTime);
    }
}