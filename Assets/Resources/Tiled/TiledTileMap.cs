using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class TiledTileMap
{

    [Serializable]
    public class TileSet
    {
        public int firstGid;
        public string source;
    }

    public int compressionLevel;
    public int height;
    public bool infinite;

    [JsonConverter(typeof(TiledLayerConverter))]
    public TiledTileMapLayer[] layers;
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


public class TiledLayerConverter : JsonConverter
{

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Not implemented yet");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {

        JArray arr = JArray.Load(reader);
        var layers = new List<TiledTileMapLayer>();

        foreach (var obj in arr) {
            var type = obj["type"].ToString();
            if (type == "tilelayer")
                layers.Add(obj.ToObject<TiledTileMapTileLayer>());
            else if (type == "objectgroup")
                    layers.Add(obj.ToObject<TiledTileMapObjectLayer>());
            else
                throw new NotImplementedException($"Not able to deserialize layer of type '{type}'");
        }

        return layers.ToArray();
    }

    public override bool CanWrite {
        get { return false; }
    }

    public override bool CanConvert(Type objectType)
    {
        return false;
    }
}