using System.Collections.Generic;

namespace Game;

/// <summary>
/// FLoor class.
/// </summary>
public class FloorData
{
    /// <summary>
    /// Is floor cleaned
    /// </summary>
    public bool IsCleaned;

    /// <summary>
    /// Is floor has anomaly
    /// </summary>
    public bool hasAnomaly;

    public List<AnomalyData> Anomalies;
}
