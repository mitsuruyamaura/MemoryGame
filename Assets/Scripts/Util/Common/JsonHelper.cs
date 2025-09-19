using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class JsonHelper {

    /// <summary>
    /// Jsonファイルをstringで読み込む
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetJsonFile(string filePath, string fileName) {
        string fileText = "";

        // JsonFileを読み込む
        FileInfo info = new FileInfo(Application.streamingAssetsPath + filePath + fileName);
        try {
            // 一行毎読む込み
            using (StreamReader reader = new StreamReader(info.OpenRead(), Encoding.UTF8)) {
                fileText = reader.ReadToEnd();
            }
        } catch (Exception e) {
            // 改行コード
            fileText += e + "\n";
        }
        //Debug.Log(fileText);
        return fileText;
    }

    public static List<T> ListFromJson<T>(string json) {
        var newJson = "{ \"list\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.list;
    }

    [Serializable]
    class Wrapper<T> {
        public List<T> list;
    }
}