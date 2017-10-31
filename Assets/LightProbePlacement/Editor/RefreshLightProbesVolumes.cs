using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class RefreshLightProbesVolumes : EditorWindow
{
    float HorizontalSpacing = 2.0f;
    float VerticalSpacing = 2.0f;
    float OffsetFomFloor = 0.5f;
    int numberOfLayers = 2;
    bool FillVolume = false;
    bool FollowFloor = true;
    bool discardInsideGeometry;

    [MenuItem("Lighting/Refresh lightprobes volumes")]
    static void Refresh()
    {
        var volumes = GameObject.FindObjectsOfType<LightProbesVolumeSettings>();
        foreach (var volume in volumes)
        {
            volume.Populate();
        }
    }
}