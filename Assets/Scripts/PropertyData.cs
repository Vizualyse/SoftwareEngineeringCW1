using System.Collections.Generic;
public class PropertyData : TileData
{
    private string colour;
    private int purchasePrice;
    private Dictionary<RentTypeEnum, int> rentCharges;

    public PropertyData(string _colour, int _purchasePrice, Dictionary<RentTypeEnum,int> _rentCharges,string _name, int _position, bool _canBeOwned, TileActionEnum _tileAction) : base(_name, _position, _canBeOwned, _tileAction) {
        colour = _colour;
        purchasePrice = _purchasePrice;
        rentCharges = _rentCharges;
    }

    public string getColour() { return colour; }
    public int getPurchasePrice() { return purchasePrice; }
    public Dictionary<RentTypeEnum, int> getRentCharges() { return rentCharges; }
}
