using System;

[Serializable]
public class TiledTileMap
{
    [Serializable]
    public class Layer
    {
        public int[] data;
        public int height;
        public int id;
        public string name;
        public float opacity;
        public string type;
        public bool visible;
        public int width;
        public int x;
        public int y;
    }

    [Serializable]
    public class TileSet
    {
        public int firstGid;
        public string source;
    }

    public int compressionLevel;
    public int height;
    public bool infinite;
    public Layer[] layers;
    public int nextLayerId;
    public int nextObjectId;
    public string orientation;
    public string renderOrder;
    public string tiledVersion;
    public int tileHeight;
    public TileSet[] tileSets;
    public int tileWidth;
    public string type;
    public string version;
    public int width;
}
