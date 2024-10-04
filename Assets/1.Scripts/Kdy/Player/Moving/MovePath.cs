using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovePath : CharacterMovement
{
    NavMeshPath m_path;
    // 네비게이션 길 이동
    public void MoveToPathByNav(Vector3 pos, GameObject obj)
    {
        StopAllCoroutines();
        if (m_path == null) { m_path = new NavMeshPath(); }
        if (NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, m_path))
        {
            switch (m_path.status) 
            {
                case NavMeshPathStatus.PathComplete:
                case NavMeshPathStatus.PathPartial:
                    StartCoroutine(MovingByPath(m_path.corners, obj));
                    break;
                case NavMeshPathStatus.PathInvalid:
                    break;
            }
        }
    }

    IEnumerator MovingByPath(Vector3[] pathList, GameObject obj)
    {
        int curPath = 1;
        while (curPath < pathList.Length)
        {
            yield return StartCoroutine(Moving(pathList[curPath++], obj));
        }
    }

}
