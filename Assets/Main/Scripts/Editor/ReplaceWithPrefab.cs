using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
    // -- this is the box in the window where you drag your "new" prefab
    [SerializeField] private GameObject prefab;

    // -- this will put objects at integer positions
    [SerializeField] private bool integerPositions = true, integerRotations, resetRotations, resetScale = true, uniformRandomScale = true, positiveRandomScale, keepName;
    [SerializeField] private Vector3 rotationRandomize, scaleRandomize;

    private Vector3 scale;

    // -- this creates the menu to open the "Replace With Prefab" window
    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();
    }

    private void OnGUI()
    {
        // -- get a handle to the prefab you want to replace everything with
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        integerPositions = EditorGUILayout.Toggle("Use integer positions", integerPositions);
        integerRotations = EditorGUILayout.Toggle("Use integer rotations", integerRotations);
        resetRotations = EditorGUILayout.Toggle("Reset rotations", resetRotations);
        rotationRandomize = EditorGUILayout.Vector3Field("Reset rotation randomization", rotationRandomize);
        resetScale = EditorGUILayout.Toggle("Reset scale", resetScale);
        scaleRandomize = EditorGUILayout.Vector3Field("Scale randomization", scaleRandomize);
        uniformRandomScale = EditorGUILayout.Toggle("Uniform random scale", uniformRandomScale);
        positiveRandomScale = EditorGUILayout.Toggle("Positive random scale", positiveRandomScale);
        keepName = EditorGUILayout.Toggle("Keep name", keepName);

        // -- if you've pressed the "Replace" button...
        if (GUILayout.Button("Replace"))
        {
            // -- get the list of objects you have selected
            var selection = Selection.gameObjects;

            // -- the objects that replace the selected ones
            List<GameObject> newObjects = new List<GameObject>();

            // -- get the prefab type (I moved this out of the loop because it makes
            // -- no sense to check it every time)
            var prefabType = PrefabUtility.GetPrefabAssetType(prefab);

            // -- loop over all of the selected objects
            for (var i = selection.Length - 1; i >= 0; --i)
            {
                // -- get the next selected object
                var selected = selection[i];
                GameObject newObject;

                // -- if your "prefab" really is a prefab . . .
                if (prefabType != PrefabAssetType.NotAPrefab)
                {
                    // -- . . . then this part should always run.
                    // -- If you update the original prefab, the replaced items will
                    // -- update as well.
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    // -- if this code is running, you didn't drag a prefab to your window.
                    // -- it will just Instantiate whatever you did drag over.
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                // -- if for some reason Unity couldn't perform your request, print an error
                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                // -- set up "undo" features for the new prefab, like setting up the old transform
                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = (integerPositions) ? Vector3Int.RoundToInt(selected.transform.localPosition) : selected.transform.localPosition;

                if (!resetRotations)
                    newObject.transform.localEulerAngles = (integerRotations) ? Vector3Int.RoundToInt(selected.transform.localEulerAngles) : selected.transform.localEulerAngles;
                else
                    newObject.transform.localRotation = Quaternion.identity;
                
                if (rotationRandomize.magnitude > 0)
                {
                    newObject.transform.localEulerAngles += (Vector3.right * Random.Range(-rotationRandomize.x, rotationRandomize.x)) + (Vector3.up * Random.Range(-rotationRandomize.y, rotationRandomize.y)) + (Vector3.forward * Random.Range(-rotationRandomize.z, rotationRandomize.z));
                    if (integerRotations)
                        newObject.transform.localEulerAngles = Vector3Int.RoundToInt(newObject.transform.localEulerAngles);
                }

                if (!resetScale)
                    newObject.transform.localScale = selected.transform.localScale;
                
                if (scaleRandomize.magnitude > 0)
                {
                    scale = (positiveRandomScale) ? (Vector3.right * Random.Range(0, scaleRandomize.x)) + (Vector3.up * Random.Range(0, scaleRandomize.y)) + (Vector3.forward * Random.Range(0, scaleRandomize.z)) : (Vector3.right * Random.Range(-scaleRandomize.x, scaleRandomize.x)) + (Vector3.up * Random.Range(-scaleRandomize.y, scaleRandomize.y)) + (Vector3.forward * Random.Range(-scaleRandomize.z, scaleRandomize.z));
                    if (uniformRandomScale)
                        scale = Vector3.one * ((scale.x + scale.y + scale.z) / 3);

                    newObject.transform.localScale += scale;
                }

                if (keepName)
                    newObject.name = selected.name;

                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                // -- now delete the old prefab
                Undo.DestroyObjectImmediate(selected);

                // -- lastly, add the new prefab to the list of new objects
                newObjects.Add(newObject);
            }

            // -- select the new objects
            Selection.objects = newObjects.ToArray();
        }

        // -- prevent the user from editing the window
        GUI.enabled = false;

        // -- update how many items you have selected (Note, it only updates when the mouse cursor is above the Replace window)
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}