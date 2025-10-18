using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Aliyun.OSS; 
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;

public class WeChatOSSBuild: IPostprocessBuildWithReport
{
    private static string endpoint = "http://oss-cn-beijing.aliyuncs.com";
    private static string accessKeyId = "";
    private static string accessKeySecret = "";
    private static string bucketName = "xuminigame";
    // private static string remoteFolder = "";

    public int callbackOrder => 1;
    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.result != BuildResult.Failed && report.summary.result != BuildResult.Cancelled)
        {
            Debug.Log("上传服务器！");
            BuildWechatGame();
        }
    }
    public static void BuildWechatGame()
    {
        // 1. 获取所有可用的 WeChatBuildProfile
        var profiles = AssetDatabase.FindAssets("t:WechatBuildProfile");
        if (profiles.Length == 0)
        {
            Debug.LogError("没有找到 WechatBuildProfile，请先在 Unity Editor 中创建一个！");
            return;
        }
        
        // 2. 取第一个 profile（也可以取你指定的）
        string profilePath = AssetDatabase.GUIDToAssetPath(profiles[0]);
        var profile = AssetDatabase.LoadAssetAtPath<WeChatBuildProfile>(profilePath);
        if (profile == null)
        {
            Debug.LogError("加载 WechatBuildProfile 失败！");
            return;
        }
        
        DeletAllFile();
        // 4. 上传到 OSS
        Uploader(profile);
    }

    private static void Uploader(WeChatBuildProfile profile)
    {
        var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        UploaderRemote(client);
        UploaderSteamingAssets(client, profile);
        UploaderBin(client, profile);
    }
    
    private static void UploaderRemote(OssClient client)
    {
        // 1. 读取 Addressables 配置
        string remoteBuildPath = GetRemotePath();
        if (string.IsNullOrEmpty(remoteBuildPath) || !Directory.Exists(remoteBuildPath))
        {
            Debug.LogError($"未找到有效的 Remote BuildPath: {remoteBuildPath}");
            return;
        }

        UploaderFiles(client, remoteBuildPath, EditorUserBuildSettings.activeBuildTarget.ToString());
    }

    private static void UploaderFiles(OssClient client, string uploaderPath, string prefix = null)
    {
        var files = Directory.GetFiles(uploaderPath, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string relativePath = file.Substring(uploaderPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string key = relativePath.Replace("\\", "/");
        
            try
            {
                using (var fs = File.OpenRead(file))
                {
                    var putResult = client.PutObject(bucketName, prefix +"/" + key, fs);
                    Debug.Log($"上传成功: {key}, ETag: {putResult.ETag}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"上传失败 {key}: {ex}");
            }
        }
    }
    private static void UploaderSteamingAssets(OssClient client, WeChatBuildProfile profile)
    {
        string streamingassetsPath = profile.buildPath + "/webgl/StreamingAssets";
        UploaderFiles(client, streamingassetsPath,"StreamingAssets");
    }
    
    private static void UploaderBin(OssClient client, WeChatBuildProfile profile)
    {
        string streamingassetsPath = profile.buildPath + "/webgl";
        if (Directory.Exists(streamingassetsPath))
        {
            // 获取目录下所有后缀为 .webgl.data.unityweb.bin.txt 的文件
            string[] files = Directory.GetFiles(streamingassetsPath, "*.webgl.data.unityweb.bin.txt",
                SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    try
                    {
                        using (var fs = File.OpenRead(file))
                        {
                            var putResult = client.PutObject(bucketName, Path.GetFileName(file), fs);
                            Debug.Log($"上传成功: {file}, ETag: {putResult.ETag}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"上传失败 {file}: {ex}");
                    }
                }
            }
        }
    }
    private static string GetRemotePath()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        // 查找 Remote BuildPath
        string remoteBuildPath = null;
        foreach (var group in settings.groups)
        {
            if (group != null && !group.Default)
            {
                var curBuildPath = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>()?.BuildPath.GetValue(settings);
                var loadPath = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>()?.LoadPath.GetValue(settings);

                // 判断是否是远程路径
                if (!string.IsNullOrEmpty(loadPath) && loadPath.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    remoteBuildPath = curBuildPath;
                    Debug.Log($"找到 Remote Group: {group.Name} => BuildPath: {curBuildPath}, LoadPath: {loadPath}");
                }
            }
        }

        return remoteBuildPath;
    }
    public static void DeletAllFile()
    {
        var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
        ObjectListing result = client.ListObjects(bucketName);
        List<string> keys = new List<string>();

        foreach (var obj in result.ObjectSummaries)
        {
            keys.Add(obj.Key);
        }

        if (keys.Count > 0)
        {
            // 批量删除
            var deleteResult = client.DeleteObjects(
                new DeleteObjectsRequest(bucketName, keys, true) // true 表示 quiet 模式
            );
        }
    }

}