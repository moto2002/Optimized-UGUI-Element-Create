//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年03月22日-03:20
//IcMusicPlayer.Editor

using System;
using System.IO;
using CabinIcarus.EditorFrame.Attributes;
using CabinIcarus.EditorFrame.Base.Editor;
using CabinIcarus.EditorFrame.Config;
using CabinIcarus.EditorFrame.Localization;
using CabinIcarus.EditorFrame.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace IcMusicPlayer.Editors
{
    [InitializeOnRun(-1)]
    static class Init
    {
        static Init()
        {
            EditorApplication.update += _loadLanguage;
        }

        private static void _loadLanguage()
        {
            LocalizationManager.Instance.LoadCsvLanguageConfig(PathUtil.GetDataPathCombinePath("Cabin Icarus/Optimized_UGUI/Optimized-UGUI-Element-Create/Localzation/Window")
                ,1);
            
            EditorApplication.update -= _loadLanguage;
        }
    }
    
    /// <summary>
    /// ugui 创建工具扩展
    /// </summary>
    public class CustomizeUGUICreate : LocalizationEditorWindow
    {
        
        /// <summary>
        /// 默认材质路径 Key
        /// </summary>
        public const string Uguiexdefaultmaterialpath_String = "UGUIExDefaultMaterialPath";

        /// <summary>
        /// 默认精灵图路径 Key
        /// </summary>
        public const string Uguiexdefaultspritepath_String = "UGUIExDefaultSpritePath";

        /// <summary>
        /// 是否是射线目标 Key
        /// </summary>
        public const string Uguiexisraycasttarget_Bool = "UGUIExIsRayCastTarget";
        
        /// <summary>
        /// 滚动视图Mask转RectMask Key
        /// </summary>
        public const string DoNotMaskToRectMask_Bool = "ScrollViewViewportMaskToRectMask";

        /// <summary>
        /// 是否富文本支持 key
        /// </summary>
        public const string Uguiexisrich_Bool = "UGUIExIsRich";

        #region Language Var
        private static string _notFindMaterial  => LocalizationManager.Instance.GetValue("NotFindMaterial",out  _);
        private static string _notSprite => LocalizationManager.Instance.GetValue("NotSprite",out  _);
        private static string _titile => LocalizationManager.Instance.GetValue("OptimizedElementSettingWindowTitle",out  _);
        private static string _selectDefaultMaterialLabel  => LocalizationManager.Instance.GetValue("SetDefaultMaterial",out  _);
        private static string _selectDefaultSpriteLabel => LocalizationManager.Instance.GetValue("SetDefaultSprite",out  _);
        private static string _isRayCastTargetLabel => LocalizationManager.Instance.GetValue("OpenRayCastTarget",out  _);
        private static string _isOpenRich => LocalizationManager.Instance.GetValue("OpenRich",out  _);
        private static string _optimizeWarning => LocalizationManager.Instance.GetValue("OptimizeWarning",out  _);
        private static string _notFindAssetError => LocalizationManager.Instance.GetValue("NotFindAsset",out  _);
        private static string _doNotMaskToRectMask => LocalizationManager.Instance.GetValue("DoNotMaskToRectMask",out  _);
        #endregion

        
        public static Material GetDefalutMaterial(GameObject go)
        {
            return _loadAsset<Material>(Uguiexdefaultmaterialpath_String, _notFindMaterial,go);
        }

        public static Sprite GetDefalutSprite(GameObject go)
        {
            return _loadAsset<Sprite>(Uguiexdefaultspritepath_String, _notSprite,go);
        }

        private static T _loadAsset<T>(string key, string errorMessage = null,GameObject go = null) where T : Object
        {
            var path = Cfg.CSVEncrypting.GetValue<string>(key);

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (!asset)
            {
                if (string.IsNullOrEmpty(path))
                {
                    if(typeof(T) == typeof(Material))
                        Debug.LogWarningFormat(_optimizeWarning,typeof(T).Name,go);   
                }
                
                if (!string.IsNullOrEmpty(errorMessage) && !string.IsNullOrEmpty(path))
                {
                    Debug.LogErrorFormat(_notFindAssetError,errorMessage,path);
                }
            }

            return asset;
        }

        public static bool IsRayCastTarget => Cfg.CSVEncrypting.GetValue<bool>(Uguiexisraycasttarget_Bool);
        
        public static bool IsRich => Cfg.CSVEncrypting.GetValue<bool>(Uguiexisrich_Bool);
        
        public static bool NoToRectMask => Cfg.CSVEncrypting.GetValue<bool>(DoNotMaskToRectMask_Bool);

        [MenuItem("Icarus/UGUI/Optimized Element Setting",false,33)]
        static void _uGUISetting()
        {
            var win = GetWindow<CustomizeUGUICreate>(true, _titile);

            win.Show();
        }

        static void _setDefaultMaterial(Material material)
        {
            _saveObjectPathToCfg(material, Uguiexdefaultmaterialpath_String);
        }

        static void _setDefaultSprite(Sprite sprite)
        {
            _saveObjectPathToCfg(sprite, Uguiexdefaultspritepath_String);
        }

        static void _saveObjectPathToCfg<T>(T asset, string key) where T : Object
        {
            string path = string.Empty;

            if (asset)
            {
                path = AssetDatabase.GetAssetPath(asset);
            }
            
            Cfg.CSVEncrypting.SetValue(key, path);
        }

        static void _setRayCastEnableState(bool enable)
        {
            Cfg.CSVEncrypting.SetValue(Uguiexisraycasttarget_Bool, enable);
        }
        
        static void _setRichEnableState(bool enable)
        {
            Cfg.CSVEncrypting.SetValue(Uguiexisrich_Bool, enable);
        }

        private Material _material;
        private Sprite _sprite;
        private bool _isRayCastTarget,_isRich,_donotMaskToRMask;


        private void Awake()
        {
            _material = _loadAsset<Material>(Uguiexdefaultmaterialpath_String);
            _sprite = _loadAsset<Sprite>(Uguiexdefaultspritepath_String);
            _isRayCastTarget = IsRayCastTarget;
            _isRich = IsRich;
        }

        private void OnGUI()
        {
            DrawLocalizationSelect();
            
            EditorGUILayout.Space();
            EditorGUILayoutUtil.DrawUILine(Color.cyan,width: position.width);
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_selectDefaultMaterialLabel);

                _material = (Material) EditorGUILayout.ObjectField(_material, typeof(Material), false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_selectDefaultSpriteLabel);

                _sprite = (Sprite) EditorGUILayout.ObjectField(_sprite, typeof(Sprite), false);
            }
            EditorGUILayout.EndHorizontal();

            _isRayCastTarget = EditorGUILayout.ToggleLeft(_isRayCastTargetLabel, _isRayCastTarget);
            
            _isRich = EditorGUILayout.ToggleLeft(_isOpenRich, _isRich);
            
            _donotMaskToRMask = EditorGUILayout.ToggleLeft(_doNotMaskToRectMask, _donotMaskToRMask);
            
            _setDefaultMaterial(_material);
            _setDefaultSprite(_sprite);
            _setRayCastEnableState(_isRayCastTarget);
            _setRichEnableState(_isRich);
            Cfg.CSVEncrypting.SetValue(DoNotMaskToRectMask_Bool, _donotMaskToRMask);
            
//            if (GUILayout.Button("复制 Unity 默认 Sprite 到项目"))
//            {
//                string kKnobPath = "UI/Skin/Knob.psd";
//
//                //var path = EditorUtility.SaveFilePanel("Select Save Folder", Application.dataPath,"uiSprite", ".png");
//
//                var path = EditorUtility.OpenFolderPanel("s", Application.dataPath, "");
//                
//                if (string.IsNullOrEmpty(path))
//                {
//                    Debug.LogError("取消了复制");
//                    return;
//                }

//                Object[] UnityAssets = AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
//                foreach (var asset in UnityAssets)
//                {
//                    path = path.Replace(Application.dataPath,"Assets/");
//                    if (asset is Texture2D)
//                    {
//                        AssetDatabase.CreateAsset(asset, Path.Combine(path, "uiSprite.asset"));
//                    }
//                }

            // var texture2D = AssetDatabase.GetBuiltinExtraResource<Texture2D>(kKnobPath);
//                var sp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
//                    new Vector2(0.5f, 0.5f));
//
//                var bys = sp.texture.EncodeToPNG();
//
//                path = path.Replace(Application.dataPath,"Assets/");

            //var bys = CombineTextures(texture2D).EncodeToPNG();

            //File.WriteAllBytes(path,bys);

//                Graphics.CopyTexture(texture2D.);

//                AssetDatabase.CreateAsset(texture2D, Path.Combine(path, "uiSprite.png"));

//                AssetDatabase.Refresh();
//            }
        }
    }
}