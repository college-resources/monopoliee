using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    private Transform[] _childObjects;
    public List<Transform> childNodeList = new List<Transform>();

    private void Start()
    {
        FillNodes();
    }

    private void FillNodes()
    {
        childNodeList.Clear();

        _childObjects = GetComponentsInChildren<Transform>();

        foreach (var child in _childObjects)
        {
            if (child != transform)
            {
                childNodeList.Add(child);
            }
        }
    }
}
