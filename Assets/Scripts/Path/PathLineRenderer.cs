using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]   // 在编辑器也能运行
[RequireComponent(typeof(LineRenderer))]
public class PathLineRenderer : MonoBehaviour
{
    [Header("引用 Path（自动获取同对象的 Path）")]
    public Path path;

    private LineRenderer lr;

    [Header("线条基本设置")]
    public float lineWidth = 0.15f;
    public Color lineColor = new Color(1f, 0.8f, 0.2f, 0.85f);

    private void Reset()
    {
        lr = GetComponent<LineRenderer>();
        if (path == null)
            path = GetComponent<Path>();

        SetupDefaults();
        UpdateLine();
    }

    private void OnValidate()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        SetupDefaults();
        UpdateLine();
    }

    void SetupDefaults()
    {
        if (lr == null) return;

        lr.useWorldSpace = true;
        lr.loop = false;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;
    }

    public void UpdateLine()
    {
        if (path == null || path.waypoints == null || path.waypoints.Length == 0)
        {
            if (lr != null)
                lr.positionCount = 0;
            return;
        }

        if (lr == null) lr = GetComponent<LineRenderer>();

        lr.positionCount = path.waypoints.Length;

        for (int i = 0; i < path.waypoints.Length; i++)
        {
            if (path.waypoints[i] != null)
                lr.SetPosition(i, path.waypoints[i].position);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PathLineRenderer))]
public class PathLineRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PathLineRenderer plr = (PathLineRenderer)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Update Path Line", GUILayout.Height(30)))
        {
            plr.UpdateLine();
        }
    }
}
#endif
