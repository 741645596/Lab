using UnityEngine;
using System.Collections.Generic;

public class LabObjectOperateCenter
{
    public static Dictionary<NDlabObject, LabObjectDragMove> m_lMove = new Dictionary<NDlabObject, LabObjectDragMove>();

    public static void RegeditMove(LabObjectDragMove move,NDlabObject obj)
    {
        if (!m_lMove.ContainsKey(obj))
        {
            m_lMove.Add(obj,move);
        }
    }

    public static void UnRegeditMove(LabObjectDragMove move, NDlabObject obj)
    {
        if (m_lMove.ContainsKey(obj))
        {
            m_lMove.Remove(obj);
        }
    }

    public static void NoticeObjToMove(List<NDlabObject> l, Vector3 moveOffset)
    {
        foreach(NDlabObject obj in l)
        {
            if (obj == null) continue;
            LabObjectDragMove move;
            if (m_lMove.TryGetValue(obj,out move))
            {
                move.ReceivedMoveCommand(moveOffset);
            }
        }
    }
}
