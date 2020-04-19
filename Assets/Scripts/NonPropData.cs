
public class NonPropData : TileData
{
    private string tileActionValue;

    public NonPropData(string _tileActionValue, string _name, int _position, bool _canBeOwned, TileActionEnum _tileAction) : base(_name, _position, _canBeOwned, _tileAction) {
        tileActionValue =_tileActionValue;
    }

    public string getTileActionValue() { return tileActionValue; }
    public void setTileActionValue(string _tileActionValue) { tileActionValue = _tileActionValue; }
}
