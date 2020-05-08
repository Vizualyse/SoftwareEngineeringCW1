using System.Collections.Generic;
public class PropertyData : TileData
{
    private string colour;
    private int purchasePrice;
    private Dictionary<RentTypeEnum, int> rentCharges;
    private RentTypeEnum currentRent;

    public PropertyData(string _colour, int _purchasePrice, Dictionary<RentTypeEnum, int> _rentCharges, RentTypeEnum _currentRent, string _name, int _position, bool _canBeOwned, TileActionEnum _tileAction) : base(_name, _position, _canBeOwned, _tileAction)
    {
        colour = _colour;
        purchasePrice = _purchasePrice;
        rentCharges = _rentCharges;
        currentRent = _currentRent;
    }

    /// <summary>
    /// Get the colour group of this property
    /// </summary>
    /// <returns>Returns a string of this colour</returns>
    public string getColour() { return colour; }

    /// <summary>
    /// Get the price of purchasing this property
    /// </summary>
    /// <returns>Returns the purchase price of this property</returns>
    public int getPurchasePrice() { return purchasePrice; }

    /// <summary>
    /// Get the collection of all the rent charges for this property
    /// </summary>
    /// <returns>Returns a dictionary of all the rent charges</returns>
    public Dictionary<RentTypeEnum, int> getRentCharges() { return rentCharges; }

    /// <summary>
    /// Get the current rent proce being charged
    /// </summary>
    /// <returns>REturns the current price</returns>
    public int getCurrentRentPrice() { return rentCharges[currentRent]; }
}
