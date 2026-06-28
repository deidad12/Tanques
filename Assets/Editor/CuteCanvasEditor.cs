using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class CuteCanvasEditor : EditorWindow
{
    [MenuItem("Tools/Restaurar Escenario (Fix Cámara y Bordes)")]
    public static void RestaurarEscenario()
    {
        // 1. Buscar LevelArt en la escena
        GameObject currentLevelArt = GameObject.Find("LevelArt");
        if (currentLevelArt != null)
        {
            Undo.DestroyObjectImmediate(currentLevelArt);
        }

        // 2. Cargar el Prefab original
        string prefabPath = "Assets/Prefabs/LevelArt.prefab";
        GameObject levelArtPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (levelArtPrefab != null)
        {
            GameObject newLevelArt = PrefabUtility.InstantiatePrefab(levelArtPrefab) as GameObject;
            newLevelArt.name = "LevelArt";
            Undo.RegisterCreatedObjectUndo(newLevelArt, "Restaurar LevelArt");
            
            // Forzar actualización de la escena
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("¡Escenario original LevelArt restaurado correctamente! La cámara y los bordes deberían estar bien.");
        }
        else
        {
            Debug.LogError("No se encontró el prefab original en: " + prefabPath);
        }
    }

    [MenuItem("Tools/Aplicar Diseño Rosa y Pequeño")]
    public static void ApplyPinkDesign()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        bool changed = false;

        RectTransform nameRect = null;
        Color colorTexto = Color.white; // Blanco para mayor legibilidad

        // 1. Cambiar el nombre a blanco, negrita y un poco más grande
        // Chequear TextMeshProUGUI
        TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var t in allTexts)
        {
            if (t.gameObject.scene != activeScene) continue;

            if (t.text.Contains("Clever") || t.text.Contains("Deidania"))
            {
                t.text = "Deidania Ureña Taveras 1-22-2809";
                t.color = colorTexto; 
                t.fontSize = 28f; // Un poco más grande
                t.fontStyle = FontStyles.Bold; // En negrita
                nameRect = t.rectTransform;
                EditorUtility.SetDirty(t);
                changed = true;
            }
        }

        // Chequear UnityEngine.UI.Text (el original suele ser este)
        UnityEngine.UI.Text[] standardTexts = Resources.FindObjectsOfTypeAll<UnityEngine.UI.Text>();
        foreach (var t in standardTexts)
        {
            if (t.gameObject.scene != activeScene) continue;

            if (t.text.Contains("Clever") || t.text.Contains("Deidania"))
            {
                t.text = "Deidania Ureña Taveras 1-22-2809";
                t.color = colorTexto;
                t.fontSize = 26; // Un poco más grande
                t.fontStyle = FontStyle.Bold; // En negrita
                nameRect = t.rectTransform;
                EditorUtility.SetDirty(t);
                changed = true;
            }
        }

        // 2. Modificar el texto "TANQUES" (subir, quitar exclamaciones y poner rojo)
        foreach (var t in allTexts)
        {
            if (t.gameObject.scene != activeScene) continue;
            if (t.text.Contains("TANQUES") || t.text.Contains("Tanques"))
            {
                t.text = "TANQUES";
                t.color = Color.red; 
                RectTransform rt = t.rectTransform;
                Vector3 pos = rt.anchoredPosition;
                pos.y = 150f; 
                rt.anchoredPosition = pos;
                EditorUtility.SetDirty(t);
                EditorUtility.SetDirty(rt);
                changed = true;
            }
        }
        foreach (var t in standardTexts)
        {
            if (t.gameObject.scene != activeScene) continue;
            if (t.text.Contains("TANQUES") || t.text.Contains("Tanques"))
            {
                t.text = "TANQUES";
                t.color = Color.red;
                RectTransform rt = t.rectTransform;
                Vector3 pos = rt.anchoredPosition;
                pos.y = 150f;
                rt.anchoredPosition = pos;
                EditorUtility.SetDirty(t);
                EditorUtility.SetDirty(rt);
                changed = true;
            }
        }

        // 3. Buscar y reposicionar el Timer debajo del nombre
        TimerUI timerUI = Object.FindObjectOfType<TimerUI>();
        UnityEngine.UI.Text timerText = null;
        if (timerUI != null)
        {
            timerText = timerUI.timerText;
        }
        
        if (timerText == null)
        {
            foreach (var t in standardTexts)
            {
                if (t.gameObject.scene != activeScene) continue;
                if (t.gameObject.name.Contains("Timer") || t.text.Contains("⏱"))
                {
                    timerText = t;
                    break;
                }
            }
        }

        if (timerText != null)
        {
            // Poner el timer en blanco, negrita y un poco más grande
            timerText.color = colorTexto;
            timerText.fontStyle = FontStyle.Bold;
            timerText.fontSize = 24; // Más grande
            EditorUtility.SetDirty(timerText);

            if (nameRect != null)
            {
                RectTransform timerRect = timerText.rectTransform;
                
                // Registrar el estado para poder deshacer con Ctrl+Z
                Undo.RecordObject(timerRect, "Mover Timer debajo del Nombre");

                // Copiar anchors y pivot para mantener la consistencia
                timerRect.anchorMin = nameRect.anchorMin;
                timerRect.anchorMax = nameRect.anchorMax;
                timerRect.pivot = nameRect.pivot;
                
                // Moverlo un poco más abajo del nombre (Y del nombre - 70 unidades por el wrap)
                Vector2 newPos = nameRect.anchoredPosition;
                newPos.y -= 70f;
                timerRect.anchoredPosition = newPos;
                
                EditorUtility.SetDirty(timerRect);
                changed = true;
            }
        }

        // 4. Cambiar los fondos a un color rosa pastel
        Image[] allImages = Resources.FindObjectsOfTypeAll<Image>();
        foreach (var img in allImages)
        {
            if (img.gameObject.scene != activeScene) continue;

            if (img.gameObject.name.Contains("Panel") || img.gameObject.name.Contains("Background") || img.gameObject.name.Contains("Fill"))
            {
                img.color = new Color(1.0f, 0.85f, 0.9f, 0.85f);
                EditorUtility.SetDirty(img);
                changed = true;
            }
        }

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("¡Diseño actualizado! Nombre y Timer en blanco, en negrita y más grandes, Timer debajo del nombre, TANQUES en rojo.");
        }
    }

    [MenuItem("Tools/Cambiar Música a Super Mario World")]
    public static void CambiarMusicaMario()
    {
        AudioSource[] sources = Object.FindObjectsOfType<AudioSource>();
        
        // Cargar el nuevo recurso (ahora con extensión .mp3)
        AudioClip nuevaMusica = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/AudioClips/BackgroundMusic.mp3");

        if (nuevaMusica == null)
        {
            nuevaMusica = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/AudioClips/SuperMarioWorld.mp3");
        }

        if (nuevaMusica == null)
        {
            Debug.LogError("No se encontró el archivo de música en Assets/AudioClips/ (verifica que exista BackgroundMusic.mp3 o SuperMarioWorld.mp3)");
            return;
        }

        bool cambiada = false;
        foreach (var source in sources)
        {
            if (source.clip != null && (source.clip.name.Contains("BackgroundMusic") || source.gameObject.name.Contains("Camera") || source.gameObject.name.Contains("GameManager")))
            {
                source.clip = nuevaMusica;
                source.volume = 0.8f;
                source.mute = false;
                source.playOnAwake = true;
                
                if (Application.isPlaying)
                {
                    source.Stop();
                    source.Play();
                }
                
                EditorUtility.SetDirty(source);
                cambiada = true;
            }
        }

        if (!cambiada)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null)
            {
                AudioSource source = gameManager.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.clip = nuevaMusica;
                    source.volume = 0.8f;
                    source.mute = false;
                    source.playOnAwake = true;
                    EditorUtility.SetDirty(source);
                    cambiada = true;
                }
            }
        }

        if (cambiada)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("¡Música de fondo cambiada con éxito a Super Mario World!");
        }
    }

    [MenuItem("Tools/QUITAR Tema Naturaleza")]
    public static void QuitarTema()
    {
        // Limpieza de objetos de la jerarquía
        GameObject a = GameObject.Find("ArbolCute");
        while (a != null) { Undo.DestroyObjectImmediate(a); a = GameObject.Find("ArbolCute"); }

        GameObject p = GameObject.Find("Plant");
        while (p != null) { Undo.DestroyObjectImmediate(p); p = GameObject.Find("Plant"); }

        GameObject t = GameObject.Find("Trunk");
        while (t != null) { Undo.DestroyObjectImmediate(t); t = GameObject.Find("Trunk"); }

        GameObject l = GameObject.Find("Leaves");
        while (l != null) { Undo.DestroyObjectImmediate(l); l = GameObject.Find("Leaves"); }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("¡Tema de naturaleza retirado!");
    }

    [MenuItem("Tools/Usar Música Predeterminada de Unity")]
    public static void UsarMusicaPredeterminada()
    {
        // Generamos un arpegio retro y cute de 3 segundos en loop
        AudioClip defaultClip = CreateCuteArpeggio(3f, 44100);

        AudioSource[] sources = Object.FindObjectsOfType<AudioSource>();
        bool cambiada = false;

        foreach (var source in sources)
        {
            if (source.gameObject.name.Contains("GameManager") || source.gameObject.name.Contains("Camera") || (source.clip != null && source.clip.name.Contains("BackgroundMusic")))
            {
                source.clip = defaultClip;
                source.volume = 0.8f;
                source.mute = false;
                source.playOnAwake = true;
                source.loop = true;

                if (Application.isPlaying)
                {
                    source.Stop();
                    source.Play();
                }

                EditorUtility.SetDirty(source);
                cambiada = true;
            }
        }

        if (!cambiada)
        {
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null)
            {
                AudioSource source = gameManager.GetComponent<AudioSource>() ?? gameManager.AddComponent<AudioSource>();
                source.clip = defaultClip;
                source.volume = 0.8f;
                source.mute = false;
                source.playOnAwake = true;
                source.loop = true;
                EditorUtility.SetDirty(source);
                cambiada = true;
            }
        }

        if (cambiada)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("¡Música predeterminada (Retro Arpeggio) generada y cargada con éxito!");
        }
    }

    private static AudioClip CreateCuteArpeggio(float lengthSec, int sampleRate)
    {
        int sampleCount = (int)(sampleRate * lengthSec);
        float[] samples = new float[sampleCount];
        
        float[] notes = { 261.63f, 329.63f, 392.00f, 523.25f, 392.00f, 329.63f };
        float noteDuration = 0.25f; 
        
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            int noteIndex = (int)(time / noteDuration) % notes.Length;
            float freq = notes[noteIndex];
            
            float noteTime = time % noteDuration;
            float envelope = Mathf.Exp(-4f * noteTime); 
            
            samples[i] = Mathf.Sin(2 * Mathf.PI * freq * time) * envelope * 0.4f;
        }

        AudioClip clip = AudioClip.Create("DefaultCuteArpeggio", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
