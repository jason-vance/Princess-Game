using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GameWorldLoader : MonoBehaviour
{
    [SerializeField]
    public GameObject DefaultTileMapGridPrefab;
    [SerializeField]
    public GameObject CoinPrefab;

    [SerializeField]
    public PlayerController PlayerController;

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

            var playerGameWorld = new int[tiledTileMap.height][];
            for (var i = 0; i < tiledTileMap.height; i++) {
                playerGameWorld[i] = new int[tiledTileMap.width];
                for (var j = 0; j < tiledTileMap.width; j++) {
                    playerGameWorld[i][j] = 0;
                }
            }

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
                if (layer is TiledTileMapTileLayer) {
                    HandleTileLayer(tiledTileMap, playerGameWorld, tileDict, gameWorld, layer);
                }
                if (layer is TiledTileMapObjectLayer) {
                    HandleObjectLayer(tiledTileMap, layer);
                }
            }

            PlayerController.GameWorld = playerGameWorld;
        } else {
            Debug.LogError("mapJson was null");
        }
    }

    private void HandleTileLayer(TiledTileMap tiledTileMap, int[][] playerGameWorld, Dictionary<int, Tile> tileDict, Grid gameWorld, TiledTileMapLayer layer)
    {
        var tileLayer = (TiledTileMapTileLayer)layer;
        var grid = Instantiate(DefaultTileMapGridPrefab);
        grid.transform.SetParent(gameWorld.transform);
        var tilemap = grid.GetComponentInChildren<Tilemap>();
        var useAsObstacles = false;

        foreach (var prop in tileLayer.properties) {
            if (prop.name == "zOrder" && prop.type == "int") {
                var renderer = tilemap.GetComponent<TilemapRenderer>();
                renderer.sortingOrder = (int)(long)prop.value;
            }
            if (prop.name == "useAsObstacles" && prop.type == "bool") {
                useAsObstacles = (bool)prop.value;
            }
        }

        for (var i = 0; i < tileLayer.data.Length; i++) {
            var tileGid = tileLayer.data[i];

            if (tileGid == 0) { continue; }

            var x = i % tiledTileMap.width;
            var y = (tiledTileMap.height - 1) - (i / tiledTileMap.width);

            Tile tile = tileDict[tileGid]; // Assign a tile asset to this.
            tilemap.SetTile(new Vector3Int(x, y, 0), tile); // Or use SetTiles() for multiple tiles.

            if (useAsObstacles) {
                playerGameWorld[y][x] = int.MaxValue;
            }
        }
    }

    private void HandleObjectLayer(TiledTileMap tiledTileMap, TiledTileMapLayer layer)
    {
        var objectLayer = (TiledTileMapObjectLayer)layer;

        foreach (var obj in objectLayer.objects) {
            var coin = Instantiate(CoinPrefab);

            var x = obj.X / tiledTileMap.tileWidth;
            var y = tiledTileMap.height - (obj.Y / tiledTileMap.tileHeight);

            coin.transform.position = new Vector3(x, y, 0);
        }
    }
}
