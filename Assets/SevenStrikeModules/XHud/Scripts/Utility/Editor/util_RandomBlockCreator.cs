using UnityEngine;
using UnityEditor;

// 自定义一个 Editor 脚本，用于生成不规则的组团物体
public class util_RandomBlockCreator : EditorWindow
{
    // 生成的 Cube 数量
    public int cubeCount = 20;
    // Cube 的大小范围
    public Vector3 sizeRange = new Vector3(0.5f, 2f, 0.5f);
    // Cube 的位置范围
    public Vector3 positionRange = new Vector3(5f, 5f, 5f);
    // 是否随机旋转
    public bool randomRotation = true;
    // 父对象，用于组织生成的物体
    public GameObject parentObject;

    [MenuItem("Tools/XHud/RandomBlockCreator")]
    public static void ShowWindow()
    {
        GetWindow<util_RandomBlockCreator>("Random Group Generator");
    }

    private void OnGUI()
    {
        // 绘制 GUI 控件
        cubeCount = EditorGUILayout.IntField("Cube Count", cubeCount);
        sizeRange = EditorGUILayout.Vector3Field("Size Range (Min, Max)", sizeRange);
        positionRange = EditorGUILayout.Vector3Field("Position Range", positionRange);
        randomRotation = EditorGUILayout.Toggle("Random Rotation", randomRotation);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        // 添加一个按钮，用于生成不规则组团物体
        if (GUILayout.Button("Generate Random Group"))
        {
            GenerateRandomGroup();
        }
    }

    private void GenerateRandomGroup()
    {
        // 如果没有父对象，则创建一个新的父对象
        if (parentObject == null)
        {
            parentObject = new GameObject("RandomGroupParent");
        }

        // 清空父对象下的所有子对象
        foreach (Transform child in parentObject.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        // 生成指定数量的 Cube
        for (int i = 0; i < cubeCount; i++)
        {
            // 创建一个新的 Cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            // 设置 Cube 的大小
            Vector3 size = new Vector3(
                Random.Range(sizeRange.x, sizeRange.y),
                Random.Range(sizeRange.x, sizeRange.y),
                Random.Range(sizeRange.x, sizeRange.y)
            );
            cube.transform.localScale = size;

            // 设置 Cube 的位置
            Vector3 position = new Vector3(
                Random.Range(-positionRange.x, positionRange.x),
                Random.Range(0, positionRange.y), // 保持在正 正向Y轴 轴范围内
                Random.Range(-positionRange.z, positionRange.z)
            );
            cube.transform.position = position;

            // 如果启用随机旋转，则设置随机旋转
            if (randomRotation)
            {
                cube.transform.rotation = Quaternion.Euler(
                    Random.Range(0, 360),
                    Random.Range(0, 360),
                    Random.Range(0, 360)
                );
            }

            // 将 Cube 添加到父对象下
            cube.transform.SetParent(parentObject.transform);
        }

        // 重新计算父对象的层级关系
        EditorUtility.SetDirty(parentObject);
        Debug.Log("Random group generated with " + cubeCount + " cubes.");
    }
}
