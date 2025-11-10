using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FoodSpawner))]
public class FoodSpawnerInspector : Editor {
    public override void OnInspectorGUI() {
        var spawner = (FoodSpawner)target;

        foreach (CustomerOrder.Meal foodType in spawner.FoodTypesMapping) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(foodType.ToString(), GUILayout.Width(100));
            EditorGUI.BeginChangeCheck();
            spawner.FoodTypesMapping[foodType] = (GameObject)EditorGUILayout.ObjectField(spawner.FoodTypesMapping[foodType], typeof(GameObject), false);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(spawner);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.LabelField("Item Tiling Offset", GUILayout.Width(100));
        spawner.Offset = EditorGUILayout.FloatField(spawner.Offset);

        EditorGUILayout.LabelField("Spawn Delay", GUILayout.Width(100));
        spawner.SpawnDelay = EditorGUILayout.FloatField(spawner.SpawnDelay);

        EditorGUILayout.LabelField("Number Spawned at Once", GUILayout.Width(500));
        spawner.NumberSpawnedAtOnce = EditorGUILayout.IntField(spawner.NumberSpawnedAtOnce);
    }
}