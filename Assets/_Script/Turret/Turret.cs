using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Turret : MonoBehaviour
{
    public Gun gun;
    public MountPoint[] mountPoints;
    Transform target;
    public float range = 3;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!target) return;

        var dashLineSize = 2f;

        foreach (var mountPoint in mountPoints)
        {
            var hardpoint = mountPoint.transform;
            var from = Quaternion.AngleAxis(-mountPoint.angleLimit / 2, hardpoint.up) * hardpoint.forward;
            var projection = Vector3.ProjectOnPlane(target.position - hardpoint.position, hardpoint.up);

            // projection line
            Handles.color = Color.white;
            Handles.DrawDottedLine(target.position, hardpoint.position + projection, dashLineSize);

            // do not draw target indicator when out of angle
            if (Vector3.Angle(hardpoint.forward, projection) > mountPoint.angleLimit / 2) return;

            // target line
            Handles.color = Color.red;
            Handles.DrawLine(hardpoint.position, hardpoint.position + projection);

            // range line
            Handles.color = Color.green;
            Handles.DrawWireArc(hardpoint.position, hardpoint.up, from, mountPoint.angleLimit, projection.magnitude);
            Handles.DrawSolidDisc(hardpoint.position + projection, hardpoint.up, .5f);
#endif
        }
    }

    void Update()
    {
        //Only shoot what is in range.
        Transform detected =  GameObject.FindGameObjectWithTag("Enemy")?.transform;
        if(!detected) return;
        if (Vector3.Distance(this.transform.position, detected.position) < range){
            target = detected;
        }else{
            target = null; 
        }
        // do nothing when no target
        if (!target) return;
            
        // aim target
        var aimed = true;
        foreach (var mountPoint in mountPoints)
        {
            if (!mountPoint.Aim(target.position))
            {
                aimed = false;
            }
        }

        // shoot when aimed
        if (aimed)
        {
            gun.Fire();
        }
    }
}