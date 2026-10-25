
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_BrickAbilityStats))]
public class SO_BrickAbilityStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // =====================================
        // ABILITY TYPES
        // =====================================

        EditorGUILayout.LabelField(
            "Ability Types",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_aggresiveBrick"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_defensiveBrick"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("_supportBrick"));

        EditorGUILayout.Space(10);


        // =====================================
        // AGGRESSIVE BRICK STATS
        // =====================================

        if (serializedObject.FindProperty("_aggresiveBrick").boolValue)
        {
            EditorGUILayout.LabelField(
                "Aggressive Brick Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            // Projectile Basic Stats
            EditorGUILayout.LabelField(
                "Projectile Basic Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_shootInterval"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_damage"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_projectileSpeed"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_buildUpTime"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_buildUpVFX"));


            EditorGUILayout.Space(10);

            // Projectile Special Stats
            EditorGUILayout.LabelField(
                "Projectile Special Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_burstFire"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_shotgunFire"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_homingShot"));


            // =====================================
            // BURST FIRE
            // =====================================

            if (serializedObject.FindProperty("_burstFire").boolValue)
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(
                    "Burst Fire Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_burstInterval"));

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_numberOfSuccession"));
            }


            // =====================================
            // SHOTGUN FIRE
            // =====================================

            if (serializedObject.FindProperty("_shotgunFire").boolValue)
            {
                EditorGUILayout.Space(10);

                EditorGUILayout.LabelField(
                    "Shotgun Fire Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_numberOfProjectiles"));

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_shotAngle"));
            }
        }


        EditorGUILayout.Space(10);


        // =====================================
        // DEFENSIVE BRICK STATS
        // =====================================

        if (serializedObject.FindProperty("_defensiveBrick").boolValue)
        {
            EditorGUILayout.LabelField(
                "Defensive Brick Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.Space(5);

            // Static Shield
            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_activateStaticShield"));

            if (serializedObject.FindProperty("_activateStaticShield").boolValue)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "Static Shield Stats",
                    EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_shieldSide"));
            }


            EditorGUILayout.Space(10);

            // Shield Timing
            EditorGUILayout.LabelField(
                "Shield Timing",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_upTime"));

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_rechargeTime"));


            EditorGUILayout.Space(10);

            // Moving Shield
            EditorGUILayout.LabelField(
                "Moving Shield Stats",
                EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(
                serializedObject.FindProperty("_activateMovingShield"));

            if (serializedObject.FindProperty("_activateMovingShield").boolValue)
            {
                EditorGUILayout.Space(5);

                EditorGUILayout.PropertyField(
                    serializedObject.FindProperty("_movementSpeed"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
