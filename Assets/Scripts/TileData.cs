
public class TileData 
{
    private string name;
    private int position;
    private bool canBeOwned;
    private TileActionEnum tileAction;
    private PlayerObject owner;

    public TileData(string _name, int _position, bool _canBeOwned, TileActionEnum _tileAction) {
        name = _name;
        position = _position;
        canBeOwned = _canBeOwned;
        tileAction = _tileAction;
        owner = null;
    }

    public string getName() { return name; }
    public int getPosition() { return position; }
    public bool getCanBeOwned() { return canBeOwned; }
    public TileActionEnum getTileAction() { return tileAction; }
    public PlayerObject getOwner() { return owner; }
    public void setOwner (PlayerObject newOwner) { owner = newOwner; }
}
