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
     
public class BuildEditor : EditorWindow
{
    private static void Init()
    {
        //isDevelopment = true;  
        buildItemList = JsonMapper.ToObject(File.ReadAllText(Application.streamingAssetsPath + "/BuildSetting.json"));
        Debug.Log("Add Json Succeed  " + buildItemList.Count);
        outPutPath = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.OUTPUT_PATH) == null ? "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.OUTPUT_PATH).ToString();
        packageName = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PACKAGE_NAME) == null ? "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.PACKAGE_NAME).ToString();
        bundleVersion = GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.BUNDLE_VERSION) == null ? "" : GetBuildSettingItemValue(GlobalBuild.CmdArgsKey.BUNDLE_VERSION).ToString();
        InitMainKeyList();
        InitAddJsonItemList();
        SetMainBuildSettingItem();
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

    public static void SetMainBuildSettingItem()
    {    
        AddBuildSettingItem(GlobalBuild.CmdArgsKey.PLATFORM, GetPlatform(platForm));

        //buildItemList.Add(SetJsonDataItem(GlobalBuild.CmdArgsKey.IS_DEVELOPMENT_BUILD, (JsonData)isDevelopment));

        AddBuildSettingItem(GlobalBuild.CmdArgsKey.OUTPUT_PATH, outPutPath);

        AddBuildSettingItem(GlobalBuild.CmdArgsKey.PACKAGE_NAME, packageName);

        AddBuildSettingItem(GlobalBuild.CmdArgsKey.BUNDLE_VERSION, bundleVersion);
    }

    static void InitMainKeyList()
    {
        MainKeyList.Add(GlobalBuild.CmdArgsKey.OUTPUT_PATH);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.PACKAGE_NAME);
        MainKeyList.Add(GlobalBuild.CmdArgsKey.BUNDLE_VERSION);
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
        for (int i = buildItemList.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in buildItemList[i].Keys)
            {
                flag = IsNotMainKey(itemKey);
                key = itemKey;
            }
            if (flag)
            {
                
                string value = buildItemList[i][key].ToString();
                SetttingElement item = new SetttingElement();
                item.Value = value;
                item.Key = key;
                _addJsonItemList.Add(item);
            }
        }
    }

    public static void AddBuildSettingItem(string key,string value)
    {
        bool flag = false;
        for (int i = buildItemList.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in buildItemList[i].Keys)
            {
                if (key == itemKey)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                buildItemList[i][key] = value;
                return;
            }
        }
        buildItemList.Add(SetJsonDataItem(key, value));
        Debug.Log("Add Json Succeed  "+ buildItemList.Count);
    }
    
    public static JsonData GetBuildSettingItemValue(string key)
    {
        for (int i = buildItemList.Count - 1; i >= 0; i--)
        {
            foreach (string itemKey in buildItemList[i].Keys)
            {
                if (key == itemKey)
                {
                    return buildItemList[i][key];
                }
            }
        }
            return "";
    }

    public void AddGuiItem(ref string name,ref string value)
    {
        GUILayout.BeginHorizontal();
        name = EditorGUILayout.TextField("name", name);
        value = EditorGUILayout.TextField("value", value);;
        if (GUILayout.Button("s"))
        {
            if (value == "" || name == "" || value == null || name == null)
            {
                Debug.Log("保存失败");
            }
            else
            {
                AddBuildSettingItem(name, value);
                CommandLineArgs.Add(name, value);
            }
        }
        GUILayout.EndHorizontal();
    }

    [MenuItem("Tools/BuildEditor")]
    public static void Open()
    {
        Init();
        BuildEditor window = GetWindow<BuildEditor>("BuildEditor", true);
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    //私有成员变量
   
    static int selectIndex;
    static bool isDevelopment;
    static string outPutPath;
    static string packageName;
    static string bundleVersion;
    static Platform platForm;
    static string RESVERSION = "RESVERSION";
    static JsonData buildItemList;
    private static List<SetttingElement> _buildItemList = new List<SetttingElement>();
    private static List<string> MainKeyList = new List<string>();
    private static List<SetttingElement> _addJsonItemList = new List<SetttingElement>();


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        platForm = (Platform)EditorGUILayout.EnumPopup("Platform: ", platForm);
        AddBuildSettingItem(GlobalBuild.CmdArgsKey.PLATFORM, GetPlatform(platForm));
        CommandLineArgs.Add(GlobalBuild.CmdArgsKey.PLATFORM, GetPlatform(platForm));

        isDevelopment = EditorGUILayout.Toggle("isDevelopment", isDevelopment);

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
            AddBuildSettingItem(GlobalBuild.CmdArgsKey.OUTPUT_PATH, outPutPath);
            CommandLineArgs.Add(GlobalBuild.CmdArgsKey.OUTPUT_PATH, outPutPath);
        }
        GUILayout.EndHorizontal();

        packageName = EditorGUILayout.TextField("PackageName", packageName);
        if (packageName != "" && packageName != null)
        {
            AddBuildSettingItem(GlobalBuild.CmdArgsKey.PACKAGE_NAME, packageName);
            CommandLineArgs.Add(GlobalBuild.CmdArgsKey.PACKAGE_NAME, packageName);
        }

        bundleVersion = EditorGUILayout.TextField("BundleVersion", bundleVersion);
        if (bundleVersion != "" && bundleVersion != null)
        {
            AddBuildSettingItem(GlobalBuild.CmdArgsKey.BUNDLE_VERSION, bundleVersion);
            CommandLineArgs.Add(GlobalBuild.CmdArgsKey.BUNDLE_VERSION, bundleVersion);
        }
        

         for (int i = 0; i < _addJsonItemList.Count; i++)
        {
            AddGuiItem(ref _addJsonItemList[i].Key, ref _addJsonItemList[i].Value);
        }

        for (int i = 0; i < _buildItemList.Count; i++)
        {
            AddGuiItem(ref _buildItemList[i].Key, ref _buildItemList[i].Value);
        }

        if (GUILayout.Button("Add"))
        {
            SetttingElement item = new SetttingElement();
            _buildItemList.Add(item);
        }

        if (GUILayout.Button("Remove"))
        {
            if(_buildItemList.Count <= 0)
            {
                return;           
            } 
            _buildItemList.RemoveAt(_buildItemList.Count-1);
           // CommandLineArgs.Remove(_buildItemList[_buildItemList.Count - 1].Key);
        }
      
        if (GUILayout.Button("SaveJson"))
        {
            string json ="";
            //for (int i = buildItemList.Count - 1; i >= 0; i--)
            //{
            //    for (int j = buildItemList[i].Count - 1; j >= 0; j--)
            //    {
            //        Debug.Log(buildItemList[i].ToString());
            //    }
            //}
            json = buildItemList.ToJson();
            Debug.Log(json);
            string path = Application.streamingAssetsPath + "/BuildSetting.json";
            StreamWriter sw = new StreamWriter(path);
            sw.Write(json);
            sw.Close();
            Debug.Log("Save Json Succeed");
        }

        if (GUILayout.Button("BuildMain"))
        {
            Zeus.Build.BuildScript.BuildPlayer();
            Debug.Log("Build Succeed");
        }

        EditorGUILayout.EndVertical();
    }

}
