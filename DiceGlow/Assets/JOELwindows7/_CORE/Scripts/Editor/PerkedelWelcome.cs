#define PERKEDEL_DICEGLOW
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
// using EditorGUILayout;

namespace com.perkedel.DiceGlow{
    public class PerkedelWelcomeScreen : EditorWindow
    {
        // First, Yoink basic CCK window elements. sorry marm. No mean.. idk, basically not any of it.
        private int _tab;
        private Vector2 _scroll;
        private Vector2 _tabScroll;

        public VisualElement root;
        public Texture2D perkedelLogo;
        public Texture2D perkedelIcon;

        public static string scriptingDefines;

        public readonly List<PerkedelDependencyListCell> dependencyList = new List<PerkedelDependencyListCell>(){
            // new PerkedelDependencyListCell("CCK",DependencyScanMethod.Defines, "CVR_CCK_EXISTS"),
            new PerkedelDependencyListCell(){
                name = "CCK",
                scanMethod = DependencyScanMethod.Defines,
                whatDefine = "CVR_CCK_EXISTS",
            },
            new PerkedelDependencyListCell(){
                name = "Perkedel DiceGlow",
                scanMethod = DependencyScanMethod.Defines,
                whatDefine = "PERKEDEL_DICEGLOW",
            },
        };

        public static readonly string[] TabsInHere = {
            "Home",
            "Dependency",
        };

        [MenuItem("Perkedel/Welcome Screen")]
        public static void Init()
        {
            
            PerkedelWelcomeScreen wnd = GetWindow<PerkedelWelcomeScreen>();
            wnd.titleContent = new GUIContent("Welcome", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/JOELwindows7/_CORE/Sprites/Logo/Perkedel Profile Monochrome White Circle 64x64.png"));
        }

        private void OnEnable(){
            perkedelLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/JOELwindows7/_CORE/Sprites/Logo/Perkedel Profile CutoutType.png");
            perkedelIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/JOELwindows7/_CORE/Sprites/Logo/Perkedel Profile Monochrome White Circle 64x64.png");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            root = rootVisualElement;


            // VisualElements objects can contain other VisualElement following a tree hierarchy
            // Label label = new Label("Hello World!");
            // root.Add(label);

            // Create button
            // Button button = new Button();
            // button.name = "button";
            // button.text = "Button";
            // root.Add(button);

            // Create toggle
            // Toggle toggle = new Toggle();
            // toggle.name = "toggle";
            // toggle.label = "Toggle";
            // root.Add(toggle);

            // NAAAAW
            // Label title = new Label("Welcome to DiceGlow!");
            // root.Add(title);
        }

        public void OnGUI(){
            // Yoink from CCK lmao, sorry marm.
            GUIStyle centeredStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 70, // Fixes size when Android
            };

            // Also NMF. sorry kafeijao
            scriptingDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup
            );

            // Now add this myself!
            GUILayout.Label(perkedelLogo, centeredStyle);

            EditorGUILayout.BeginVertical();

            // Apply CCK yoinks to here, add our twists!
            // _tabScroll = EditorGUILayout.BeginScrollView(_tabScroll);
            // _tab = GUILayout.Toolbar(_tab, new string []{"Home", "Dependency"});
            _tab = GUILayout.Toolbar(_tab, TabsInHere);
            // EditorGUILayout.EndScrollView();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            switch(_tab){
                case 0:
                    Tab_Home();
                    break;
                case 1:
                    Tab_Dependency();
                    break;
                default:
                    Tab_Dummy();
                    break;
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void Tab_Home(){
            EditorGUILayout.LabelField("Welcome to DiceGlow!",EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("Hello!", MessageType.Info);

            EditorGUILayout.LabelField("Haha: ", "hihi");
            EditorGUILayout.LabelField("Hehe: ", "hoho");

            Buttons_Home();
        }

        private void Tab_Dependency(){
            EditorGUILayout.LabelField("Please ensure these are installed to work on this project",EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("AAA", "BBB");
            // if(scriptingDefines.Contains("CVR_CCK_EXISTS")){
            //     EditorGUILayout.LabelField("CCK", "Installed");
            // } else {
            //     EditorGUILayout.LabelField("CCK", "Not Installed");
            // }
            foreach(var dependency in dependencyList){
                EditorGUILayout.LabelField(dependency.name, dependency.executeScan(scriptingDefines)? "Installed": "Not Installed");
                
            }
            // EditorGUILayout.LabelField(dependencyList[0].name, dependencyList[0].executeScan(scriptingDefines)? "Installed": "Not Installed");
            EditorGUILayout.EndVertical();
        }

        private void Tab_Dummy(){

        }

        private void Buttons_Home(){
            EditorGUILayout.BeginHorizontal();
            //Vert Left
            EditorGUILayout.BeginVertical();
            if(GUILayout.Button("View Source Code")){
                Application.OpenURL("https://github.com/Perkedel/CVR_Stuffings");
            }
            if(GUILayout.Button("Check Dependencies")){
                _tab = 1;
            }
            if(GUILayout.Button("Official CVR Documentation")){
                Application.OpenURL("https://documentation.abinteractive.net/");
            }
            if(GUILayout.Button("Unity Official Documentations")){
                Application.OpenURL("https://docs.unity3d.com/");
            }
            if(GUILayout.Button("Report CVR Bugs & Feature Requests")){
                Application.OpenURL("https://feedback.abinteractive.net/");
            }
            EditorGUILayout.EndVertical();
            //Vert Right
            EditorGUILayout.BeginVertical();
            if(GUILayout.Button("Download & Update CCK")){
                Application.OpenURL("https://documentation.abinteractive.net/cck/setup/");
            }
            if(GUILayout.Button("Read Tutorials")){
                
            }
            if(GUILayout.Button("Unofficial CVR Community Docs")){
                Application.OpenURL("https://wiki.chilloutvr.eu/");
            }
            if(GUILayout.Button("Download & Update Unity")){
                Application.OpenURL("https://unity.com/download");
            }
            if(GUILayout.Button("CVR Terms of Service")){
                Application.OpenURL("https://documentation.abinteractive.net/official/legal/tos");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        void Update()
        {

        }
    }
}


#endif