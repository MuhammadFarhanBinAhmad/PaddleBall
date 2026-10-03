
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_BrickAbilityStats))]
public class SO_BrickAbilityStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Ability types
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_aggresiveBrick"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_defensiveBrick"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_supportBrick"));

        EditorGUILayout.Space(10);

        // Aggressive Brick Stats
        if (serializedObject.FindProperty("_aggresiveBrick").boolValue)
        {
            EditorGUILayout.LabelField(
                "Aggressive Brick Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_shootInterval"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_damage"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_projectileSpeed"));

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(
                "Projectile Special Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_burstFire"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_shotgunFire"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_homingShot"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_sniperlaserShot"));

            // Burst Fire
            if (serializedObject.FindProperty("_burstFire").boolValue)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField(
                    "Burst Fire Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_burstInterval"));

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_numberOfSuccession"));
            }

            // Shotgun Fire
            if (serializedObject.FindProperty("_shotgunFire").boolValue)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField(
                    "Shotgun Fire Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_numberOfProjectiles"));

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_shotAngle"));
            }

            // Sniper / Laser
            if (serializedObject.FindProperty("_sniperlaserShot").boolValue)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField(
                    "Sniper / Laser Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_buildUpTime"));

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_buildUpVFX"));
            }
        }

        EditorGUILayout.Space(10);

        // Defensive Brick Stats
        if (serializedObject.FindProperty("_defensiveBrick").boolValue)
        {
            EditorGUILayout.LabelField(
                "Defensive Brick Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_activateStaticShield"));

            // Static Shield Stats
            if (serializedObject.FindProperty("_activateStaticShield").boolValue)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField(
                    "Static Shield Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_shieldSide"));
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField(
                "Shield Timing",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_upTime"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_rechargeTime"));

            // Moving Shield Stats
            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_activateMovingShield"));

            if (serializedObject.FindProperty("_activateMovingShield").boolValue)
            {
                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_movementSpeed"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}