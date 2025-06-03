using System;

[Serializable]
public class TiledTileMapLayer
{
    public int id;
    public string name;
    public float opacity;
    public TiledCustomProperty[] properties;
    public string type;
    public bool visible;
    public int x;
    public int y;
}

[Serializable]
public class TiledTileMapTileLayer : TiledTileMapLayer
{
    public int[] data;
    public int height;
    public int width;
}

[Serializable]
public class TiledTileMapObjectLayer : TiledTileMapLayer
{
    public string drawOrder;
    public TiledObject[] objects;
}