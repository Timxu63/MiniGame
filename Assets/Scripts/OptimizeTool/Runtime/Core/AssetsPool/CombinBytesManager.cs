using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

public class CombinBytesManager : MonoSingleton<CombinBytesManager>
{
    [Header("释放调试")] public bool isOpenReleaseDebug;
    private Dictionary<string, (string rootPath, string filePath, string fileType, string fileText, byte[] fileBytes)> _combinedBytes = new Dictionary<string, (string rootPath, string filePath, string fileType, string fileText, byte[] fileBytes)>();
    private Dictionary<string, ScriptableObject> _scriptableObjects = new Dictionary<string, ScriptableObject>();

    public async Task InitBytes(string path)
    {
        var l = await FileTool.LoadAndAnalysisAllBytes(path);
        foreach (var item in l)
        {
            _combinedBytes.Add(item.filePath, item);
        }
    }

    public void ReleaseBytes(string path)
    {
        List<string> removeKeys = ListPool<string>.Get();
        foreach (var VARIABLE in _combinedBytes)
        {
            if (VARIABLE.Value.rootPath == path)
            {
                removeKeys.Add(VARIABLE.Key);
            }
        }

        foreach (var VARIABLE in removeKeys)
        {
            if (isOpenReleaseDebug)
            {
                Debug.Log(("Release:" + VARIABLE));
            }

            _combinedBytes.Remove(VARIABLE);
            _scriptableObjects.Remove(VARIABLE);
        }

        ListPool<string>.Release(removeKeys);
    }

    public void InitBytes(string rootPath, TextAsset textAsset)
    {
        var l = FileTool.AnalysisAllBytes(rootPath, textAsset);
        foreach (var item in l)
        {
            _combinedBytes.Add(item.filePath, item);
        }
    }

    public void AddValue(string path, (string fileType, string fileText, byte[] bytes) value)
    {
        _combinedBytes.Add(path, (path, path, value.fileType, value.fileText, value.bytes));
    }

    public (string rootPath, string filePath, string fileType, string fileText, byte[] fileBytes) GetBytes(string path)
    {
        return _combinedBytes[path];
    }

    public ScriptableObject GetScriptableObject(string path)
    {
        if (!_scriptableObjects.TryGetValue(path, out var scriptableObject))
        {
            if (_combinedBytes.TryGetValue(path, out var l))
            {
                string json = Encoding.UTF8.GetString(l.fileBytes);
                System.Type type = System.Type.GetType(l.fileType);
                scriptableObject = ScriptableObject.CreateInstance(type);
                JsonUtility.FromJsonOverwrite(json, scriptableObject);
                _scriptableObjects.Add(path, scriptableObject);
            }
        }

        return scriptableObject;
    }
}