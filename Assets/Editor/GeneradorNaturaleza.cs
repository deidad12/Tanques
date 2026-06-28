using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class GeneradorNaturaleza : EditorWindow
{
    // ── QUITAR todos los arbolitos que creamos ──────────────────────
    [MenuItem("Tools/QUITAR Tema Naturaleza")]
    public static void QuitarTema()
    {
        string[] nombresABorrar = { "ArbolCute", "Plant", "Trunk", "Leaves" };

        var allObjects = Object.FindObjectsOfType<GameObject>();
        int borrados = 0;
        foreach (var go in allObjects)
        {
            if (go == null) continue;
            foreach (string nombre in nombresABorrar)
            {
                if (go.name == nombre)
                {
                    Object.DestroyImmediate(go);
                    borrados++;
                    break;
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("QUITAR Tema: se eliminaron " + borrados + " objetos de naturaleza. Guarda con Ctrl+S.");
    }
}
