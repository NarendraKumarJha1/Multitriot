using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class PowerUuidGenerator : MonoBehaviour
{
    public List<PowerUpsPickUpHandler> powers = new List<PowerUpsPickUpHandler>();

    internal void GenerateUuid()
    {
        if (powers.Count == 0)
        {
            Debug.LogError("List is empty");
            return;
        }

        int pwrIndex = 0;

        while (pwrIndex < powers.Count)
        {
        GenAgain: int id = Random.Range(1000, 9999);

            PowerUpsPickUpHandler pwr = powers.Find(x => x.powerUuid == id);
            if (pwr != null)
                goto GenAgain;

            if (powers[pwrIndex] == null)
            {
                Debug.LogError("Invalide Element at " + pwrIndex + " index");
                return;
            }

            powers[pwrIndex].powerUuid = id;
            pwrIndex++;
        }
    }
}

[CustomEditor(typeof(PowerUuidGenerator))]
public class PowerUuidGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var target = base.target as PowerUuidGenerator;

        base.OnInspectorGUI();

        if (GUILayout.Button("Fill List"))
        {
            target.powers.Clear();
            GameObject[] obj = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var a in obj)
            {
                PowerUpsPickUpHandler handle = a.GetComponent<PowerUpsPickUpHandler>();
                if (a.GetComponent<PowerUpsPickUpHandler>() == null) continue;
                if (!a.activeInHierarchy) continue;
                if (a.scene.buildIndex == EditorSceneManager.GetActiveScene().buildIndex) continue;

                target.powers.Add(handle);
            }
            // target.powers = Resources.FindObjectsOfTypeAll<PowerUpsPickUpHandler>().ToList();
        }

        if (GUILayout.Button("Generate UUID"))
        {
            target.GenerateUuid();
        }
    }
}