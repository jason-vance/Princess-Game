using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GameWorldLoader : MonoBehaviour
{
    [SerializeField]
    public GameObject DefaultTileMapGridPrefab;

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
        var mapJson = Resources.Load<TextAsset>("Tiled/sample_world");
        if (mapJson != null) {
            var tiledTileMap = JsonConvert.DeserializeObject<TiledTileMap>(mapJson.text);

            var tileDict = new Dictionary<int, Tile>();
            foreach (var tiledTileSet in tiledTileMap.tileSets) {
                var tileSetName = tiledTileSet.source.Substring(0, tiledTileSet.source.LastIndexOf("."));
                var tileSetJson = Resources.Load<TextAsset>($"Tiled/{tileSetName}");
                var tileSet = JsonConvert.DeserializeObject<TiledTileSet>(tileSetJson.text);

                var imageName = tileSet.image.Substring(0, tileSet.image.LastIndexOf("."));
                Object[] assets = Resources.LoadAll($"Tiled/{imageName}");

                foreach (var asset in assets) {
                    if (asset is Sprite) {
                        var index = int.Parse(asset.name.Substring(asset.name.LastIndexOf("_") + 1));
                        var sprite = (Sprite)asset;

                        var tile = new Tile();
                        tile.sprite = sprite;
                        tileDict[index + tiledTileSet.firstGid] = tile;
                    }
                }
            }

            var gameWorld = new GameObject("GameWorld").AddComponent<Grid>();
            foreach (var layer in tiledTileMap.layers) {
                var grid = Instantiate(DefaultTileMapGridPrefab);
                grid.transform.SetParent(gameWorld.transform);
                var tilemap = grid.GetComponentInChildren<Tilemap>();

                foreach (var prop in layer.properties) {
                    if (prop.name == "zOrder" && prop.type == "int") {
                        var renderer = tilemap.GetComponent<TilemapRenderer>();
                        renderer.sortingOrder = (int)(long)prop.value;
                    }
                }

                for (var i = 0; i < layer.data.Length; i++) {
                    var tileGid = layer.data[i];

                    if (tileGid == 0) { continue; }

                    var x = i % tiledTileMap.width;
                    var y = (tiledTileMap.height - 1) - (i / tiledTileMap.width);

                    Tile tile = tileDict[tileGid]; // Assign a tile asset to this.
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile); // Or use SetTiles() for multiple tiles.
                }
            }
        } else {
            Debug.LogError("mapJson was null");
        }
    }
}
