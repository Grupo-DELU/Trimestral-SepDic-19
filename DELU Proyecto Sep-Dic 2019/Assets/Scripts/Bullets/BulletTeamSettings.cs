using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
#endif

/// <summary>
/// Bullet Team for Collision Detection
/// </summary>
[System.Serializable]
public class BulletTeam {
    /// <summary>
    /// Layers affected by bullet in a Collision
    /// </summary>
    public int value = 0;
}

#if UNITY_EDITOR

// BulletTeamDrawerUIE
[CustomPropertyDrawer(typeof(BulletTeam))]
public class BulletTeamDrawerUIE : PropertyDrawer {
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var prop = property.FindPropertyRelative("value");

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        prop.intValue = EditorGUI.MaskField(
            position,
            prop.intValue,
            BulletTeamSettings.GetOrCreateSettings().LayerNames);

        EditorGUI.EndProperty();
    }
}

/// <summary>
/// Class for layer settings
/// </summary>
public class BulletTeamSettings : ScriptableObject {

    /// <summary>
    /// Bullet Team Layers Names
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private string[] layerNames = new string[32];

    /// <summary>
    /// Bullet Team Layers Names
    /// </summary>
    public string[] LayerNames { get { return layerNames; } private set { layerNames = value; } }

    public const string k_MyCustomSettingsPath = "Assets/Editor/BulletTeamSettings.asset";

    internal static BulletTeamSettings GetOrCreateSettings() {
        var settings = AssetDatabase.LoadAssetAtPath<BulletTeamSettings>(k_MyCustomSettingsPath);
        if (settings == null) {
            settings = ScriptableObject.CreateInstance<BulletTeamSettings>();
            settings.LayerNames = new string[32];
            AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings() {
        return new SerializedObject(GetOrCreateSettings());
    }

    [SettingsProvider]
    public static SettingsProvider CreateBulletTeamSettingsProvider() {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
        var provider = new SettingsProvider("Project/BulletTeamSettings", SettingsScope.Project) {
            label = "Bullet Team Layers",
                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) => {
                    var settings = BulletTeamSettings.GetSerializedSettings();
                    settings.Update();

                    // rootElement is a VisualElement. If you add any children to it, the OnGUI function
                    // isn't called because the SettingsProvider uses the UIElements drawing framework.
                    //rootElement.styleSheets.Add (AssetDatabase.LoadAssetAtPath<StyleSheet> ("Assets/Editor/settings_ui.uss"));
                    var title = new Label() {
                        text = "Bullet Team Layers"
                    };
                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new ScrollView() {
                        style = {
                        flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    SerializedProperty layerNamesProp = settings.FindProperty("layerNames");

                    for (int i = 0; i < layerNamesProp.arraySize; i++) {
                        SerializedProperty property = layerNamesProp.GetArrayElementAtIndex(i);
                        TextField prop = new TextField() {
                            label = string.Format("Layer {0}", i),
                            value = property.stringValue
                        };
                        prop.BindProperty(property);
                        prop.AddToClassList("property-value");
                        properties.Add(prop);
                    }
                    settings.ApplyModifiedProperties();
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new [] { "Bullet Team Layers" })
        };

        return provider;
    }

}
#endif // UNITY_EDITOR