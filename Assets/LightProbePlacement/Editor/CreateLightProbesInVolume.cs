using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class CreateLightProbesInVolume : EditorWindow
{
    float HorizontalSpacing = 2.0f;
    float VerticalSpacing = 2.0f;
    float OffsetFomFloor = 0.5f;
    int numberOfLayers = 2;
    bool FillVolume = false;
    bool FollowFloor = true;
    bool discardInsideGeometry;

    [MenuItem("Lighting/Create Light Probes in Volume")]

    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CreateLightProbesInVolume window = (CreateLightProbesInVolume)EditorWindow.GetWindow(typeof(CreateLightProbesInVolume), true, "Create Light Probes in Volume");
        window.position = new Rect((Screen.width / 2) - 125, Screen.height / 2 + 100, 300, 200 );
        window.Show();
        Debug.Log("Started Window", window);
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox("Adds a lightprobegroup as a child of the selected volume. Fills the volume with lightprobes placed on a grid based on Horizontal and Vertical Resolution.", MessageType.Info);
        HorizontalSpacing = EditorGUILayout.FloatField("Horizontal Resolution", HorizontalSpacing);
        VerticalSpacing = EditorGUILayout.FloatField("Vertical Resolution", VerticalSpacing);
        OffsetFomFloor = EditorGUILayout.FloatField("Offset From Floor", OffsetFomFloor);
        FillVolume = EditorGUILayout.Toggle("Fill Volume", FillVolume);
        if(!FillVolume)
        {
            numberOfLayers = EditorGUILayout.IntField("Number of layers", numberOfLayers);
        }
        FollowFloor = EditorGUILayout.Toggle("Follow Floor", FollowFloor);
        discardInsideGeometry = EditorGUILayout.Toggle("Discard probes inside Geometry", discardInsideGeometry);

        // Clamp values
        if (HorizontalSpacing < 0.1f) HorizontalSpacing = 0.1f;
        if (VerticalSpacing < 0.1f) VerticalSpacing = 0.1f;

        if (GUILayout.Button("Create Light Probes in Selected Volume"))
        {
            Populate();
        }
    }

    void Populate()
    {
        Debug.Log("Start light probe");

        GameObject Select = Selection.activeGameObject;
        if ( Select == null ) Debug.Log("nothing selected");

        // Get total bounds for selected objects

        // First check that mesh has a collider attached to it
        // if it doesn't then we can't raycast so we should ignore this object
        Collider col = Select.GetComponent<Collider>();
        if (col == null) Debug.Log("Col not found", col);

        //Check if there is already a lightprobegroup component
        // if there is destroy it
        LightProbeGroup oldLightprobes = Select.GetComponent<LightProbeGroup>();
        if (oldLightprobes != null) DestroyImmediate(oldLightprobes);

        // Get the col bounds
        Bounds bbox = col.bounds;
        Select.GetComponent<BoxCollider>().enabled = false;

        // Update total bounds
        float minX = bbox.min.x;
        float minY = bbox.min.y;
        float minZ = bbox.min.z;
        float maxX = bbox.max.x;
        float maxY = bbox.max.y;
        float maxZ = bbox.max.z;

        // Now go through in a grid and attempt to place a light probe using raycasting
        float xCount = (maxX - minX)/HorizontalSpacing;
        float zCount = (maxZ - minZ) / HorizontalSpacing;
        float ycount = (maxY - minY) / VerticalSpacing;
        float startxoffset = ((maxX - minX) - (int)xCount * HorizontalSpacing) / 2;
        float startzoffset = ((maxZ - minZ) - (int)zCount * HorizontalSpacing) / 2;
        List<Vector3> VertPositions = new List<Vector3>();
        for (int z = 0; z < zCount; z++)
        {
            for (int x = 0; x < xCount; x++)
            {
                RaycastHit hit;
                Ray ray = new Ray();
                ray.origin = new Vector3(startxoffset + minX + x * HorizontalSpacing, maxY + 1, startzoffset + minZ + z * HorizontalSpacing);
                ray.direction = -Vector3.up;
                if (Physics.Raycast(ray, out hit, (maxY - minY) * 2))
                {
                    if (hit.point.y + OffsetFomFloor < maxY && hit.point.y + OffsetFomFloor > minY)
                        VertPositions.Add(hit.point + new Vector3(0, OffsetFomFloor, 0));
                    //Debug.DrawRay(hit.point, -ray.direction * hit.distance, Color.red, (maxY - minY));

                    int maxLayer = FillVolume ? Mathf.FloorToInt(ycount) : numberOfLayers ;

                    for (int i = 1; i < maxLayer; i++)
                    {
                        if (hit.point.y + OffsetFomFloor + i * VerticalSpacing < maxY && hit.point.y + OffsetFomFloor + VerticalSpacing > minY)
                            VertPositions.Add(hit.point + new Vector3(0, OffsetFomFloor + i * VerticalSpacing, 0));
                    }
                    EditorUtility.DisplayProgressBar("Tracing floor collisions", (z * x).ToString() + "/" + (zCount * xCount).ToString(), (float)(z * x) / (float)(zCount * xCount));

                }
                else Debug.Log("Miss");
            }
        }
        EditorUtility.ClearProgressBar();
        List<Vector3> validVertPositions = new List<Vector3>();

        int j = 0;
        if (discardInsideGeometry)
        {
            Vector3 insideTestPosition = Select.transform.position + Select.GetComponent<BoxCollider>().center + new Vector3(0,maxY/2,0);
            Debug.DrawLine(insideTestPosition + Vector3.up, insideTestPosition - Vector3.up, Color.green, 5);
            Debug.DrawLine(insideTestPosition + Vector3.right, insideTestPosition - Vector3.right, Color.green, 5);
            Debug.DrawLine(insideTestPosition + Vector3.forward, insideTestPosition - Vector3.forward, Color.green, 5);
            foreach (var positionCandidate in VertPositions)
            {
                EditorUtility.DisplayProgressBar("Checking probes inside geometry", j.ToString() + "/" + VertPositions.Count, (float)j / (float)VertPositions.Count);

                Ray forwardRay = new Ray(insideTestPosition, Vector3.Normalize(positionCandidate - insideTestPosition));
                Ray backwardRay = new Ray(positionCandidate, Vector3.Normalize(insideTestPosition - positionCandidate));
                RaycastHit[] hitsForward;
                RaycastHit[] hitsBackward;
                hitsForward = Physics.RaycastAll(forwardRay,Vector3.Distance(positionCandidate,insideTestPosition));
                hitsBackward = Physics.RaycastAll(backwardRay, Vector3.Distance(positionCandidate, insideTestPosition));
                if (hitsForward.Length == hitsBackward.Length) validVertPositions.Add(positionCandidate);
                else
                    Debug.DrawRay(backwardRay.origin, backwardRay.direction * Vector3.Distance(positionCandidate, insideTestPosition), Color.red, 5);
                j++;
            }
            EditorUtility.ClearProgressBar();
        }
        else
            validVertPositions = VertPositions;
            

        // Check if we have any hits
        if (validVertPositions.Count < 1) Debug.Log("no valid hit");

        // Get _LightProbes game object
        //GameObject LightProbeGameObj = GameObject.Find("_LightProbes");
        //var LightProbeGameObj = new GameObject("_Lightprobes");
        Select.AddComponent<LightProbeGroup>();
        //LightProbeGameObj.transform.parent = Select.transform;
        if (Select == null) Debug.Log("Lightprobegroup not found");

        // Get light probe group component
        LightProbeGroup LPGroup = Select.GetComponent("LightProbeGroup") as LightProbeGroup;
        if (LPGroup == null) Debug.Log("Lightprobe component not found");

        // Create lightprobe positions
        Vector3[] ProbePos = new Vector3[validVertPositions.Count];
        for (int i = 0; i < validVertPositions.Count; i++)
        {
            ProbePos[i] = validVertPositions[i]-Select.gameObject.transform.position;
        }

        // Set new light probes
        LPGroup.probePositions = ProbePos;
        Select.GetComponent<BoxCollider>().enabled = true;
        //Selection.activeGameObject = Select;
        Debug.Log("Finished placing " + ProbePos.Length + " probes.");
    }
}