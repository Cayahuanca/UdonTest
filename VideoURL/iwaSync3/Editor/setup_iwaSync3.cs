using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using HoshinoLabs.IwaSync3.Udon;

namespace Praecipua.Udon.VideoURL
{
    public class setup_iwaSync : EditorWindow
    {
        private VideoCore videoCore;
        private VideoController videoController;

        private bool useMessage;

        // ウィンドウを表示
        [MenuItem("Window/Praecipua/VideoURL Setup (iwaSync3)")]
        static void ShowWindow()
        {
            GetWindow<setup_iwaSync>("VideoURL Setup (iwaSync3)");
        }

        // ウィンドウの表示内容
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Setup VideoURL for iwaSync3");

            EditorGUILayout.Space();

            videoCore = EditorGUILayout.ObjectField("Video Core", videoCore, typeof(VideoCore), true) as VideoCore;
            videoController = EditorGUILayout.ObjectField("Video Controller", videoController, typeof(VideoController), true) as VideoController;

            EditorGUILayout.Space();

            useMessage = EditorGUILayout.Toggle("Use Message", useMessage);

            // YTTL の機能とは競合するため、YTTL がプロジェクトに存在する場合に警告を表示
            string yttlPath = AssetDatabase.GUIDToAssetPath("1433f797c5d9d4b44a2bcce919b4f1f9");
            MonoScript yttlScript = AssetDatabase.LoadAssetAtPath<MonoScript>(yttlPath);
            if (yttlScript != null)
            {
                EditorGUILayout.HelpBox("You can not use this VideoURL Message with YTTL.", MessageType.Warning);
            }

            if (GUILayout.Button("Setup"))
            {
                CreateVideoURLGameObject();
            }
        }

        // セットアップの処理
        private void CreateVideoURLGameObject()
        {
            // Video Core と Video Controller がアタッチされているか確認
            if (videoCore == null || videoController == null)
            {
                Debug.LogError("Video Core and Video Controller must be assigned.");
                return;
            }

            // Video Controller の子にすでに VideoURL があるか確認、ある場合は削除
            GameObject existingVideoURLObject = videoController.transform.parent?.Find("VideoURL")?.gameObject;
            if (existingVideoURLObject != null)
            {
                Undo.DestroyObjectImmediate(existingVideoURLObject);
            }

            // VideoURL を作成
            GameObject videoURLObject = new GameObject("VideoURL");
            videoURLObject.transform.SetParent(videoController.transform.parent);
            videoURLObject.transform.localPosition = Vector3.zero;
            videoURLObject.transform.localScale = Vector3.one;

            // VideoURL に VideoURL Udon コンポーネントをアタッチ
            var player_iwaSync3 = videoURLObject.AddComponent<Praecipua.Udon.VideoURL.player_iwaSync3>();
            player_iwaSync3.videoCore = videoCore;

            // Use Message が true の場合は VideoURL Udon コンポーネントに Use Message を設定
            if (useMessage)
            {
                player_iwaSync3.useMessage = true;
            }

            // Video Controller の子に Canvas と Canvas/Panel があるか確認
            // UdonButton Prefab があるか確認
            // それらがある場合は、Canvas/Panel の子に UdonButton の Prefab を設置し、VideoURL Udon コンポーネントに InputField を設定
            Transform panelTransform = videoController.transform.Find("Canvas/Panel");

            string prefabPath = AssetDatabase.GUIDToAssetPath("f95329b2f1c7e894b81bc2e3ec6cae09");
            GameObject urlButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            // GameObject urlButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Praecipua/VideoURL/iwaSync3/URLButton.prefab");

            if (panelTransform != null && urlButtonPrefab != null)
            {
                    GameObject existingURLButtonObject = panelTransform.transform.Find("URLButton")?.gameObject;
                    if (existingURLButtonObject != null)
                    {
                        Undo.DestroyObjectImmediate(existingURLButtonObject);
                    }

                    GameObject urlButton = Instantiate(urlButtonPrefab, panelTransform);

                    urlButton.name = "URLButton";

                    player_iwaSync3.inputField = urlButton.GetComponentInChildren<InputField>();
            }
            else
            {
                Debug.LogError("Canvas or Canvas/Panel, or URLButton prefab are not found in the child of Video Controller.");
            }
        }
    }
}