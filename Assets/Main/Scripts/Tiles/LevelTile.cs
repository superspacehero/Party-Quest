using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Tile", menuName = "2D/Tiles/Level Tile")]
public class LevelTile : RuleTile
{
    public static void InstantiateThing(SavedTile tile, GameObject thingToInstantiate, Tilemap tilemap)
    {
        // Instantiate the GameObject
        GameObject instantiatedObject = Instantiate(thingToInstantiate, tilemap.CellToLocalInterpolated(tile.position + tilemap.tileAnchor) + Vector3.up * tile.position.z, Quaternion.identity, tilemap.transform);
        instantiatedObject.name = thingToInstantiate.name;

        tile.tileThing = instantiatedObject.GetComponent<GameThing>();
    }

    /// <summary>
    /// StartUp is called on the first frame of the running Scene.
    /// </summary>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    /// <param name="instantiatedGameObject">The GameObject instantiated for the Tile.</param>
    /// <returns>Whether StartUp was successful</returns>
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiatedGameObject)
    {
        if (instantiatedGameObject != null)
        {
            Tilemap tmpMap = tilemap.GetComponent<Tilemap>();
            Matrix4x4 orientMatrix = tmpMap.orientationMatrix;

            Vector3 gameObjectTranslation = new Vector3();
            Quaternion gameObjectRotation = new Quaternion();
            Vector3 gameObjectScale = new Vector3();

            bool ruleMatched = false;
            Matrix4x4 transform = Matrix4x4.identity;
            foreach (TilingRule rule in m_TilingRules)
            {
                if (RuleMatches(rule, position, tilemap, ref transform))
                {
                    transform = orientMatrix * transform;

                    // Converts the tile's translation, rotation, & scale matrix to values to be used by the instantiated GameObject
                    gameObjectTranslation = new Vector3(transform.m03, transform.m13 + position.z, transform.m23);
                    gameObjectRotation = Quaternion.LookRotation(new Vector3(transform.m02, transform.m12, transform.m22), new Vector3(transform.m01, transform.m11, transform.m21));
                    gameObjectScale = transform.lossyScale;

                    ruleMatched = true;
                    break;
                }
            }
            if (!ruleMatched)
            {
                // Fallback to just using the orientMatrix for the translation, rotation, & scale values.
                gameObjectTranslation = new Vector3(orientMatrix.m03, orientMatrix.m13 + position.z, orientMatrix.m23);
                gameObjectRotation = Quaternion.LookRotation(new Vector3(orientMatrix.m02, orientMatrix.m12, orientMatrix.m22), new Vector3(orientMatrix.m01, orientMatrix.m11, orientMatrix.m21));
                gameObjectScale = transform.lossyScale;
            }

            instantiatedGameObject.transform.localPosition = gameObjectTranslation + tmpMap.CellToLocalInterpolated(position + tmpMap.tileAnchor);
            instantiatedGameObject.transform.localRotation = gameObjectRotation;
            instantiatedGameObject.transform.localScale = gameObjectScale;

            #if !UNITY_EDITOR
                instantiatedGameObject.isStatic = true;
                foreach (Transform child in instantiatedGameObject.transform)
                {
                    child.gameObject.isStatic = true;
                }
            #endif
        }

        return true;
    }
}