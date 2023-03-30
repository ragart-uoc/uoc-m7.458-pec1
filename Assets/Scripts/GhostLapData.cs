using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GhostLapData : ScriptableObject
{
    private List<Vector3> m_CarPositions;
    private List<Quaternion> m_CarRotations;

    public void AddNewData(Transform transform)
    {
        m_CarPositions.Add(transform.position);
        m_CarRotations.Add(transform.rotation);
    }

    public void GetDataAt(int sample, out Vector3 position, out Quaternion rotation)
    {
        position = m_CarPositions[sample];
        rotation = m_CarRotations[sample];
    }

    public void Reset()
    {
        m_CarPositions.Clear();
        m_CarRotations.Clear();
    }
}