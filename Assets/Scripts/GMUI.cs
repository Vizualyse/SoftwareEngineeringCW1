using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GMUI : MonoBehaviour
{
    //InGameUI
    public GameObject[] bottomUI;

    //PlayerSelectUI
    public GameObject[] addPlayerUI = new GameObject[6];
    void Start()
    {
        setDropdownOptions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setNoOfPlayers(InputField inp) { bottomUI = new GameObject[Int32.Parse(inp.text)]; makeAddPlayerUI(); }

    private void makeAddPlayerUI() {
        for (int i = 0; i < addPlayerUI.Length; i++) {
            if (i < bottomUI.Length) {
                addPlayerUI[i].SetActive(true);
            } else {
                addPlayerUI[i].SetActive(false);
            }
        }
    }

    private void setDropdownOptions() {

        for (int i = 0; i < addPlayerUI.Length; i++) {
            addPlayerUI[i].GetComponentInChildren<Dropdown>().options.Clear();
            foreach (String piece in System.Enum.GetNames(typeof(PiecesEnum))) {
                addPlayerUI[i].GetComponentInChildren<Dropdown>().options.Add(new Dropdown.OptionData() { text = piece });
            }
        }
    }
}
