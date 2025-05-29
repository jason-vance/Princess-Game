using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class GameWorldLoader : MonoBehaviour
{
    [SerializeField]
    public GameObject GameWorld;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadGameWorld();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadGameWorld()
    {
        var mapJson = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Tiled/sample_world.json", typeof(TextAsset));
        if (mapJson != null) {
            var tileMap = JsonConvert.DeserializeObject<TiledTileMap>(mapJson.text);

            var tileSetDict = new Dictionary<string, TiledTileSet>();
            foreach (var tileSet in tileMap.tileSets) {
                var tileSetJson = (TextAsset)AssetDatabase.LoadAssetAtPath($"Assets/Tiled/{tileSet.source}", typeof(TextAsset));
                tileSetDict[tileSet.source] = JsonConvert.DeserializeObject<TiledTileSet>(tileSetJson.text);
            }
        }

        var background = new GameObject("Background");
        background.transform.SetParent(GameWorld.transform);
    }
}
