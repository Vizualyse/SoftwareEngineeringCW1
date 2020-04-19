using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    private TileData data;

    public void setupTileObject(TileData _data) {
        data = _data;
    }

    public TileData getData() { return data; }
}
