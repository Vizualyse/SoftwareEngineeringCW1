using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardObject
{
    private CardData data;

    public void setupCard(CardData _data) {
        data = _data;
    }

    public CardData getData() { return data; }
}
