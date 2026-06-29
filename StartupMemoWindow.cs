using UnityEngine;
using UnityEditor;

// Unity起動時にこのクラスのスタティックコンストラクタを呼ぶ
[InitializeOnLoad]
public class StartupMemoWindow : EditorWindow
{
    private static MemoData memoData;
    private const string MemoDataPath = "Assets/Editor/ProgressManager/MemoData.asset";

    // Unity起動時に自動で呼ばれる処理
    static StartupMemoWindow()
    {
        EditorApplication.delayCall += InitOnStartup; // エディタ起動後に呼ばれる処理にメソッドを登録する
    }

    /* ウィンドウ起動用メソッド */
    private static void InitOnStartup()
    {
        // すでに開いている場合は二重に開かないようにする
        if (!HasOpenInstances<StartupMemoWindow>())
        {
            ShowWindow();
        }
    }

    /* 新しいウィンドウを表示するメソッド */
    [MenuItem("Tools/引き継ぎメモ")]
    public static void ShowWindow()
    {
        StartupMemoWindow window = GetWindow<StartupMemoWindow>("引き継ぎメモ");
        window.minSize = new Vector2(350, 450);
        window.Show();
    }

    /* ウィンドウに実際に要素を表示するメソッド */
    private void OnGUI()
    {
        // データの読み込み、なければ自動生成
        if (memoData == null)
        {
            memoData = AssetDatabase.LoadAssetAtPath<MemoData>(MemoDataPath);
            if (memoData == null)
            {
                memoData = CreateInstance<MemoData>();
                AssetDatabase.CreateAsset(memoData, MemoDataPath);
                AssetDatabase.SaveAssets();
            }
        }

        // 見出しの表示
        GUILayout.Label("📝 開発引き継ぎメモ", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // 前回編集スクリプトの表示
        GUILayout.Label("🔗 前回編集していたスクリプト:", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        MonoScript selectedScript = (MonoScript)EditorGUILayout.ObjectField(// オブジェクト設定欄の作成 
            memoData.lastEditedScript, // 初期値(保存されている値)
            typeof(MonoScript), //　設定するオブジェクトのタイプ
            false // シーン上の実態を受け付けるか
        );

        if (EditorGUI.EndChangeCheck())
        {
            memoData.lastEditedScript = selectedScript;
            EditorUtility.SetDirty(memoData);
        }

        // スクリプトが登録されている場合だけ開くボタンを表示
        if (memoData.lastEditedScript != null)
        {
            if (GUILayout.Button("💻 このスクリプトを開く", GUILayout.Height(25)))
            {
                // Visual Studioなどの外部エディタで対象のスクリプトを開く
                AssetDatabase.OpenAsset(memoData.lastEditedScript);
            }
        }

        EditorGUILayout.Space(15);

        // 前回どこまでやったかの表示
        GUILayout.Label("🏷️ 前回どこまで編集していたか:", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        string lastNotes = EditorGUILayout.TextArea(memoData.lastSessionNotes, GUILayout.Height(100));// テキスト入力欄
        if (EditorGUI.EndChangeCheck())
        {
            memoData.lastSessionNotes = lastNotes;
            EditorUtility.SetDirty(memoData); // 変更を保存対象にする
        }

        EditorGUILayout.Space(15);

        // 次に何をするかの表示
        GUILayout.Label("🏷️ 次に何をするか (ToDo):", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        string todoNotes = EditorGUILayout.TextArea(memoData.nextTodoNotes, GUILayout.Height(100));// テキスト入力欄
        if (EditorGUI.EndChangeCheck())
        {
            memoData.nextTodoNotes = todoNotes;
            EditorUtility.SetDirty(memoData);
        }

        EditorGUILayout.Space(20);

        // 手動保存ボタン
        if (GUILayout.Button("メモを保存"))
        {
            AssetDatabase.SaveAssets();
            Debug.Log("引き継ぎメモを保存しました！");
        }
    }
}