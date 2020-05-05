
public class CardData
{
    private string name;
    private string pile;
    private CardActionEnum actionType;
    private string cardActionValue;

    public CardData(string _name, string _pile, CardActionEnum _actionType, string _cardActionValue)
    {
        name = _name;
        pile = _pile;
        actionType = _actionType;
        cardActionValue = _cardActionValue;
    }

    public string getName() { return name; }
    public string getPile() { return pile; }
    public CardActionEnum getActionType() { return actionType; }
    public string getCardActionValue() { return cardActionValue; }
}
