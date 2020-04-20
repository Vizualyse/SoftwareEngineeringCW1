using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
    private string playerName;
    private PiecesEnum piece;
    private int money;
    private int position;
    private List<TileObject> propertiesOwned;

    private bool isOut;

    public void setupPlayer(string _name, PiecesEnum _piece, int _money, int _position) {
        propertiesOwned = new List<TileObject>();
        playerName = _name;
        piece = _piece;
        money = _money;
        position = _position;
        isOut = false;
    }

    public string getPlayerName() { return playerName; }
    public PiecesEnum getPiece() { return piece; }
    public int getMoney() { return money; }
    public void increaseMoney(int inc) { money += inc; }
    public void decreaseMoney(int dec) { money -= dec; }
    public int getPosition() { return position; }
    public void setPosition(int newPos) { position = newPos; }
    public void increasePosition(int inc) { position += inc; }
    public void addProperty(TileObject prop) { propertiesOwned.Add(prop); }
    public void removeProperty(TileObject prop) { propertiesOwned.Remove(prop); }

    public void setIsOut(bool set) { isOut = set; }
    public bool getIsOut() { return isOut; }

}
