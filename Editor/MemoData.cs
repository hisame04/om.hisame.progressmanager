using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "MemoData", menuName = "Scriptable Objects/MemoData")]
public class MemoData : ScriptableObject
{
    public MonoScript lastEditedScript;
    [TextArea(5, 10)]
    public string lastSessionNotes = "ここに前回どこまでやったかを記録します。";
    
    [TextArea(5, 10)]
    public string nextTodoNotes = "ここに次回のタスクを記録します。";
}
