/*
 * ============================================================================
 * ⚠ 版权声明（禁止删除、禁止修改、衍生作品必须保留此注释）⚠
 * ============================================================================
 * 版权声明 Copyright (C) 2025-Present Nanjing SevenStrike Media Co., Ltd.
 * 中文名称：南京塞维斯传媒有限公司
 * 英文名称：SevenStrikeMedia
 * 项目作者：徐寅智
 * 项目名称：XHud - Unity UGUI 高级管理架构插件
 * 项目启动：2025年8月
 * 官方网站：http://sevenstrike.com/
 * 授权协议：GNU Affero General Public License Version 3 (AGPL 3.0)
 * 协议说明：
 * 1. 你可以自由使用、修改、分发本插件的源代码，但必须保留此版权注释
 * 2. 基于本插件修改后的衍生作品，必须同样遵循 AGPL 3.0 授权协议
 * 3. 若将本插件用于网络服务（如云端Unity编辑器、在线动效生成工具），必须公开修改后的完整源代码
 * 4. 完整协议文本可查阅：https://www.gnu.org/licenses/agpl-3.0.html
 * ============================================================================
 * 违反本注释保留要求，将违反 AGPL 3.0 授权协议，需承担相应法律责任
 */
namespace SevenStrikeModules.XHud.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class Editor_XHud_Tool_RandomBlockCreator : EditorWindow
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
            GetWindow<Editor_XHud_Tool_RandomBlockCreator>("Random Group Generator");
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
}