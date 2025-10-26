using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

public static class FileTool
{
    /// <summary>
    /// 递归查找指定目录下所有扩展名为 .cs 的文件，将其重命名为 .cs.txt
    /// </summary>
    /// <param name="directoryPath">根目录路径</param>
    public static void RenameCsToCsTxt(string directoryPath)
    {
        // 获取所有 .cs 文件（递归查找）
        string[] csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
        foreach (string file in csFiles)
        {
            // 如果文件扩展名确实为 .cs（而非已经转换为 .cs.txt 的情况）
            if (Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase))
            {
                // 生成新文件名：在原路径后面追加 .txt
                string newPath = file + ".txt";
                Debug.Log($"Renaming {file} to {newPath}");
                try
                {
                    File.Move(file, newPath);
                }
                catch (Exception e)
                {
                    Debug.LogError($"重命名失败: {file} -> {newPath}, 错误信息: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 递归查找指定目录下所有扩展名为 .cs.txt 的文件，将其重命名为 .cs
    /// </summary>
    /// <param name="directoryPath">根目录路径</param>
    public static void RenameCsTxtToCs(string directoryPath)
    {
        // 获取所有 .cs.txt 文件（递归查找）
        string[] csTxtFiles = Directory.GetFiles(directoryPath, "*.cs.txt", SearchOption.AllDirectories);
        foreach (string file in csTxtFiles)
        {
            // 去除最后的 .txt 后缀
            string newPath = file.Substring(0, file.Length - ".txt".Length);
            Debug.Log($"Renaming {file} to {newPath}");
            try
            {
                File.Move(file, newPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"重命名失败: {file} -> {newPath}, 错误信息: {e.Message}");
            }
        }
    }


    public static void CopyDirectory(string sourceDir, string destinationDir)
    {
        // 如果源目录不存在，则抛出异常
        if (!Directory.Exists(sourceDir))
        {
            throw new DirectoryNotFoundException($"源目录不存在: {sourceDir}");
        }

        // 如果目标目录存在，则先删除
        if (Directory.Exists(destinationDir))
        {
            Directory.Delete(destinationDir, true);
        }

        // 创建目标目录
        Directory.CreateDirectory(destinationDir);

        // 复制所有文件（直接覆盖）
        foreach (string filePath in Directory.GetFiles(sourceDir))
        {
            string destFile = Path.Combine(destinationDir, Path.GetFileName(filePath));
            File.Copy(filePath, destFile, true);
        }

        // 递归复制所有子目录
        foreach (string subDir in Directory.GetDirectories(sourceDir))
        {
            string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
            CopyDirectory(subDir, destSubDir);
        }
    }

    public static string GetLocalIPAddress()
    {
        // 【优化】macOS/Linux 兼容的 IP 获取方式，排除回环地址
        try
        {
            using (var socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP))
            {
                socket.Connect("8.8.8.8", 65530); // 通过连接外部地址获取真实 IP
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint?.Address.ToString() ?? "127.0.0.1";
            }
        }
        catch
        {
            // Fallback 方案：筛选第一个 IPv4 非回环地址
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return hostEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))?.ToString() ?? "127.0.0.1";
        }
    }

    public static async Task<List<(string rootPath, string filePath, string fileType, string fileText, byte[] fileBytes)>> LoadAndAnalysisAllBytes(string path)
    {
        var textAsset = await AssetsPoolManager.Instance.LoadAssetAsync<TextAsset>(path);
        var files = AnalysisAllBytes(path, (TextAsset)textAsset.Result);
        AssetsPoolManager.Instance.Release(path, true);
        return files;
    }

    public static List<(string rootPath, string filePath, string fileType, string fileText, byte[] fileBytes)> AnalysisAllBytes(string rootPath, TextAsset textAsset)
    {
        byte[] textBytes = textAsset.bytes;

        MemoryStream stream = UnzipBytesToMemory(textBytes);
        BinaryReader reader = new BinaryReader(stream);

        List<(string rootPath, string fileName, string fileType, string fileText, byte[] fileBytes)> files = new List<(string rootPath, string fileName, string fileType, string fileText, byte[] fileBytes)>();
        while (reader.BaseStream.Position != reader.BaseStream.Length)
        {
            int fileTypeLen = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            byte[] fileTypeData = reader.ReadBytes(fileTypeLen);
            string fileType = System.Text.Encoding.UTF8.GetString(fileTypeData);
            // 读取固定长度的文件名和文件大小
            int fileNameLen = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            byte[] fileNameData = reader.ReadBytes(fileNameLen);
            string fileName = System.Text.Encoding.UTF8.GetString(fileNameData);
            int fileSize = BitConverter.ToInt32(reader.ReadBytes(4), 0);
            byte[] fileContent = reader.ReadBytes(fileSize);
            string fileText = null;
            if (Path.GetExtension(fileName).ToLower() == ".json")
            {
                fileText = System.Text.Encoding.UTF8.GetString(fileContent);
            }

            files.Add((rootPath, fileName, fileType, fileText, fileContent));
        }

        reader.Close();
        stream.Close();
        return files;
    }

    public static MemoryStream UnzipBytesToMemory(byte[] compressedBytes)
    {
        using (MemoryStream inputStream = new MemoryStream(compressedBytes))
        {
            MemoryStream outputStream = new MemoryStream();
            using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }
    }
}