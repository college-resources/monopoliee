using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float rotationTime = 1f;
    public Route currentRoute;
    public Vector3 offset;

    public IEnumerator Move(int previousLocation, int newLocation)
    {
        var _routePosition = previousLocation;
        var steps = newLocation - _routePosition;
        if (steps < 0) steps += 40;
        
        while (steps > 0)
        {
            _routePosition++;
            _routePosition %= currentRoute.childNodeList.Count;
            
            var nextPos = currentRoute.childNodeList[_routePosition].position + offset;
            while (Step(nextPos)) yield return null; 
            
            yield return new WaitForSeconds(0.1f);
            
            steps--;
            
            if (_routePosition % 10 == 0)
            {
                yield return StartCoroutine(Rotate(Vector3.up * 90, rotationTime));
            }
        }
    }
    
    private IEnumerator Rotate(Vector3 byAngles, float inTime) 
    {
        var thisTransform = transform;
        
        var fromAngle = thisTransform.rotation;
        var toAngle = Quaternion.Euler(thisTransform.eulerAngles + byAngles);
        for (var t = 0f; t <= 1; t += Time.deltaTime / inTime) {
            thisTransform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        thisTransform.rotation = toAngle;
    }

    private bool Step(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 5f * Time.deltaTime));
    }
}
