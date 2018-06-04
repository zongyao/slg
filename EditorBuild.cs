using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Zeus.Build;
using System;
using LitJson;
using System.IO;
using System.Text;
using UnityEditorInternal;


enum Platform
{
    ANDROID,
    IOS,
    WINDOWS,
    WINDOWS64
}

public  class SetttingElement
{
    public string Key;
    public string Value;
}
     
public class EditorBuild : EditorWindow
{
    private static void Init()
    {
        isDevelopment = true;  
        jsonData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/BuildSetting.json"));
        Debug.Log("Read Json Succeed  !! JsonCount: " + jsonData.Count);
        InitMainKeyList();
        InitAddJsonItemList();
        InitMainBuildSettingItem();
    }

    public static JsonData SetJsonDataItem(string name, string value)
    {
        JsonData setItem = new JsonData();
        setItem[name] = value;
        return setItem;
    }

     static string GetPlatform(Platform platForm)
    {
        if (platForm == Platform.ANDROID )
        {
            return GlobalBuild.Platform.ANDROID; 
        }
        else if (platForm == Platform.IOS)
        {
            return GlobalBuild.Platform.IOS;
        }
        else if (platForm == Platform.WINDOWS)
        {
            return GlobalBuild.Platform.WINDOWS;
        }
        else if (platForm == Platform.WINDOWS64)
        {
            return GlobalBuild.Platform.WINDOWS64;
        }
        return "";
    }

    static Platform GetPlatform(string platForm)
    {
        if (platForm == GlobalBuild.Platform.ANDROID )
        {
            return Platform.ANDROID;
        }
        else if (platForm == GlobalBuild.Platform.IOS )
        {
            return Platform.IOS;
        }
        else if (platForm == GlobalBuild.Platform.WINDOWS )
        {
            return Platform.WINDOWS;
        }
        else if (platForm == GlobalBuild.Platform.WINDOWS64 )
        {
            return Platform.WINDOWS64;
        }
        return Platform.WINDOWS;
    }

    static string ToStringIsDevelopment(bool isDevelopment)
    {
        if (isDevelopment)
        {
            return "true";
        }
        else
        {
            return "false";
        }
    }

    static bool ToBoolIsDevelopment(string isDevelopment)
    {
        if (isDevelopment == "true")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void InitMainBuildSettingItem()
    {
        outPutPath = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.OUTPUT_PATH) == null ?
                     "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.OUTPUT_PATH).ToString();

        packageName = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PACKAGE_NAME) == null ? 
                      "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PACKAGE_NAME).ToString();

        bundleVersion = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.BUNDLE_VERSION) == null ?
                        "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.BUNDLE_VERSION).ToString();

        string strPlatForm = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PLATFORM) == null ?
                             GlobalBuild.Default.Platform : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PLATFORM).ToString();

        platForm =  GetPlatform(strPlatForm);

        isDevelopment = ToBoolIsDevelopment (GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD) == null ?
                        null : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD).ToString());
    
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.PLATFORM, strPlatForm);
   
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD, ToStringIsDevelopment(isDevelopment)); ;
   
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.OUTPUT_PATH, outPutPath);
  
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.PACKAGE_NAME, packageName);

        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.BUNDLE_VERSION, bundleVersion);
    }

    static void InitMainKeyList()
    {
        MainKeyList.Add(GlobalBuild.CmdArgsKey.PLATFORM);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.OUTPUT_PATH);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.PACKAGE_NAME);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.BUNDLE_VERSION);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD);
    }

    static bool IsNotMainKey( string key)
    {
        for(int i=0;i<MainKeyList.Count;i++)
        {
            if (key == MainKeyList[i])
            {
                return false;
            }
        }
        return true;
    }

    static void InitAddJsonItemList()
    {
        bool flag = false;
        string key = "";
        for (int i = jsonData.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in jsonData[i].Keys)
            {
                flag = IsNotMainKey(itemKey);
                key = itemKey;
            }
            if (flag)
            {              
                string value = jsonData[i][key].ToString();
                SetttingElement item = new SetttingElement();
                item.Value = value;
                item.Key = key;
                _addedJsonItemList.Add(item);
            }
        }
    }

    public static void AddJsonBuildSettingItem(string key,string value)
    {
        bool flag = false;
        for (int i = jsonData.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in jsonData[i].Keys)
            {
                if (key == itemKey)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                jsonData[i][key] = value;
                CommandLineArgs.Add(key, value);
                return;
            }
        }
        jsonData.Add(SetJsonDataItem(key, value));
        CommandLineArgs.Add(key, value);
        Debug.Log("Add Json Succeed  "+ jsonData.Count);
    }
    
    public static JsonData GetBuildSettingItemValue(string key)
    {
        for (int i = jsonData.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in jsonData[i].Keys)
            {
                if (key == itemKey)
                {
                    return jsonData[i][key];
                }
            }
        }
            return "";
    }

    public static JsonData GetJsonDataItem(string key)
    {
        for (int i = jsonData.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in jsonData[i].Keys)
            {
                if (key == itemKey)
                {
                    return jsonData[i];
                }
            }
        }
        return "";
    }

    public static int GrtAddBuildItemListIndex(string key)
    {
        for(int i=0;i< _addedJsonItemList.Count;i++)
        {
            if(_addedJsonItemList[i].Key == key)
            {
                return i;
            }
        }
        return -1;
    }
    public void AddGuiItem(ref string name,ref string value)
    {
        GUILayout.BeginHorizontal();
        name = EditorGUILayout.TextField("name:", name);
        value = EditorGUILayout.TextField("value:", value);;
        if (GUILayout.Button("s"))
        {
            if (value == "" || name == "" || value == null || name == null)
            {
                Debug.Log("保存失败");
            }
            else
            {
                AddJsonBuildSettingItem(name, value);
                CommandLineArgs.Add(name, value);
            }
        }
        GUILayout.EndHorizontal();
    }

    public void AddedGuiItem(ref string name, ref string value)
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("name:", name);
        value = EditorGUILayout.TextField("value:", value); ;
        if (GUILayout.Button("s"))
        {
            if (value == "" || value == null)
            {
                Debug.Log("保存失败");
            }
            else
            {
                AddJsonBuildSettingItem(name, value);
                CommandLineArgs.Add(name, value);
            }
        }
        if (GUILayout.Button("del"))
        {
           JsonData itemData = GetJsonDataItem(name);
            Debug.Log(itemData.ToString());
            CommandLineArgs.Remove(name);
            jsonData.Remove(itemData);
            Debug.Log(name);
            GrtAddBuildItemListIndex(name);
            _addedJsonItemList.RemoveAt(GrtAddBuildItemListIndex(name));
        }
        GUILayout.EndHorizontal();
    }

    [MenuItem("Tools/BuildEditor")]
    public static void Open()
    {
        Init();
        EditorBuild window = GetWindow<EditorBuild>("EditorBuild", true);
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    //私有成员变量
    static int selectIndex;
    static bool isDevelopment;
    static string strIsDevelopment;
    static string outPutPath;
    static string packageName;
    static string bundleVersion;
    static Platform platForm;
    static string RESVERSION = "RESVERSION";
    static JsonData jsonData;
    private static List<SetttingElement> _addBuildItemList = new List<SetttingElement>();
    private static List<string> MainKeyList = new List<string>();
    private static List<SetttingElement> _addedJsonItemList = new List<SetttingElement>();

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        platForm = (Platform)EditorGUILayout.EnumPopup("Platform: ", platForm);
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.PLATFORM, GetPlatform(platForm));
       
        isDevelopment = EditorGUILayout.Toggle("isDevelopment", isDevelopment);
        AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD, ToStringIsDevelopment(isDevelopment));

        GUILayout.BeginHorizontal();
        outPutPath = EditorGUILayout.TextField("OutPutPath", outPutPath);
        if (GUILayout.Button("…", EditorStyles.toolbar, GUILayout.Width(30)))
        {
            string temp = EditorUtility.OpenFolderPanel("Output Folder", outPutPath, string.Empty);
            if (!string.IsNullOrEmpty(temp))
                outPutPath = temp;
        }
        if (outPutPath != "" && outPutPath !=null)
        {
            AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.OUTPUT_PATH, outPutPath);
        }
        GUILayout.EndHorizontal();

        packageName = EditorGUILayout.TextField("PackageName", packageName);
        if (packageName != "" && packageName != null)
        {
            AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.PACKAGE_NAME, packageName);
        }

        bundleVersion = EditorGUILayout.TextField("BundleVersion", bundleVersion);
        if (bundleVersion != "" && bundleVersion != null)
        {
            AddJsonBuildSettingItem(GlobalBuild.CmdArgsKey.BUNDLE_VERSION, bundleVersion);
        }
        

         for (int i = 0; i < _addedJsonItemList.Count; i++)
        {
            AddedGuiItem(ref _addedJsonItemList[i].Key, ref _addedJsonItemList[i].Value);
        }

        for (int i = 0; i < _addBuildItemList.Count; i++)
        {
            AddGuiItem(ref _addBuildItemList[i].Key, ref _addBuildItemList[i].Value);
        }

        if (GUILayout.Button("Add"))
        {
            SetttingElement item = new SetttingElement();
            _addBuildItemList.Add(item);
        }

        if (GUILayout.Button("Remove"))
        {
            if (_addBuildItemList.Count <= 0)
            {
                return;
            }
            if(_addBuildItemList[_addBuildItemList.Count - 1].Key!=null && _addBuildItemList[_addBuildItemList.Count - 1].Key!="")
            {
               CommandLineArgs.Remove(_addBuildItemList[_addBuildItemList.Count - 1].Key);
                jsonData.Remove(jsonData[jsonData.Count - 1]);
            }
            _addBuildItemList.RemoveAt(_addBuildItemList.Count - 1);
        }

        if (GUILayout.Button("SaveJson"))
        {
            string json = jsonData.ToJson();
            string path = Application.dataPath + "/BuildSetting.json";
            StreamWriter sw = new StreamWriter(path);
            sw.Write(json);
            sw.Close();
            jsonData = JsonMapper.ToObject(File.ReadAllText(path));
            Debug.Log("Save Json Succeed");
        }

        if (GUILayout.Button("BuildMain"))
        {
            Zeus.Build.BuildScript.BuildPlayer();
            Debug.Log("Build Succeed");   
        }

        EditorGUILayout.EndVertical();
    }

    void OnDisable()
    {
        _addBuildItemList.Clear();
        MainKeyList.Clear();
        _addedJsonItemList.Clear();
    }

}
