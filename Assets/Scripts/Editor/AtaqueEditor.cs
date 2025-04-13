using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ataque))]
public class AtaqueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Obtener la referencia al script Ataque
        Ataque ataque = (Ataque)target;

        // Dibujar las propiedades predeterminadas
        DrawDefaultInspector();

        // Deshabilitar fuerzaEmpuje si explosion está activado
        GUI.enabled = !ataque.explosion; // Deshabilitar si explosion es true
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fuerzaEmpuje"), new GUIContent("Fuerza Empuje"));
        GUI.enabled = true; // Restaurar el estado de GUI

        // Aplicar cambios al objeto serializado
        serializedObject.ApplyModifiedProperties();
    }
}
