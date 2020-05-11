using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerPieces : MonoBehaviour
{
    private GameManager gm;
    private PlayerObject[] players;
    private GameObject[] playerPieces;
    private int[] piecesRotations;
    private int[] playerPos;
    private List<int> cornerTiles;

    private bool init = false;

    private GameObject goTile;

    public void Init()
    { 

        init = true;

        cornerTiles = new List<int>(new int[] { 1, 11, 21, 31 });
        goTile = GameObject.Find("Tile1");

        gm = GameObject.Find("GM").GetComponent<GameManager>();
        players = gm.getPlayers();
        playerPos = new int[players.Length];
        playerPieces = new GameObject[players.Length];
        piecesRotations = new int[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            playerPos[i] = players[i].getPosition();
            playerPieces[i] = Prefab(players[i].getPiece(), i);
        }
    }

    public GameObject Prefab(PiecesEnum name, int playerNo)
    {
        Object pf = null;
        pf = Resources.Load("Pieces/" + name);
         
         
        if (pf != null)
        {
            GameObject inst = (GameObject)Instantiate(pf);
            
            if (inst != null)
            {
                if (playerNo < 3)
                {
                    inst.transform.position = goTile.transform.position + new Vector3(0.2f, 0.2f, -0.2f + (playerNo * 0.6f));
                }
                else
                {
                    inst.transform.position = goTile.transform.position + new Vector3(0.5f, 0.2f, -0.2f + ((playerNo-3) * 0.6f));
                }
                switch (name)
                {
                    case PiecesEnum.Boot:
                        inst.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                        inst.transform.Rotate(new Vector3(0, -90, 0));
                        piecesRotations[playerNo] = -90;
                        break;
                    case PiecesEnum.Cat:
                        inst.transform.localScale = new Vector3(3, 3, 3);
                        inst.transform.Rotate(new Vector3(0, -90, 0));
                        piecesRotations[playerNo] = -90;
                        break;
                    case PiecesEnum.Goblet:
                        inst.transform.localScale = new Vector3(5,5,5);
                        inst.transform.Rotate(new Vector3(0, 0, 0));
                        piecesRotations[playerNo] = 90;
                        break;
                    case PiecesEnum.Hatstand:
                        inst.transform.localScale = new Vector3(15,15,15);
                        inst.transform.Rotate(new Vector3(0, 0, 0));
                        piecesRotations[playerNo] = 90;
                        break;
                    default:
                        inst.transform.localScale = new Vector3(5, 5, 5);
                        inst.transform.Rotate(new Vector3(0, 90, 0));
                        piecesRotations[playerNo] = 90;
                        break;
                }
                return inst;
            }
        }
        return null;
    }


    void Update()
    {
        if (init)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].getPosition() != playerPos[i])
                {
                    int turns = countTurns(playerPos[i], players[i].getPosition());
                    UpdatePosition(i, players[i].getPosition(), turns);
                    playerPos[i] = players[i].getPosition();
                }
            }
        }
    }

    void UpdatePosition(int playerNo, int newPos, int turns)
    {
        GameObject newTile = GameObject.Find("Tile" + (newPos));
        GameObject newTileParent = newTile.transform.parent.gameObject;
        int playersOnTile = 0;
        foreach(int p in playerPos)
        {
            if(p == newPos)
            {
                playersOnTile++;
            }
        }
        Vector3 tileOffset = new Vector3(1.3f, 0, -0.2f + playersOnTile * 0.6f);
        switch (newTileParent.name)
        {
            case "Bot":
                tileOffset = Quaternion.Euler(0, 0, 0) * tileOffset;
                //playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo], 0);
                break;
            case "Left":
                tileOffset = Quaternion.Euler(0, 90, 0) * tileOffset;
                //playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+90, 0);
                break;
            case "Top":
                tileOffset = Quaternion.Euler(0, 180, 0) * tileOffset;
                //playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+180, 0);
                break;
            case "Right":
                tileOffset = Quaternion.Euler(0, 270, 0) * tileOffset;
                //playerPieces[playerNo].transform.localRotation = Quaternion.Euler(0, piecesRotations[playerNo]+270, 0);
                break;
        }
        Vector3 end = new Vector3(newTile.transform.position.x , 0.2f, newTile.transform.position.z) + tileOffset;
        moveWithTurns(end, playerNo, turns);
    }

    private int countTurns(int start, int end)
    {
        int count = 0;
        if(start == 0)
        {
            start++;
        }

        if(start < end) { 
            for(int i = start+1; i <= end; i++)
            {
                if (cornerTiles.Contains(i))
                {
                    count++;
                }
            }
        }
        else
        {
            for (int i = 1; i <= end; i++)
            {
                if (cornerTiles.Contains(i))
                {
                    count++;
                }
            }
        }
        return count;
    }

    private void moveWithTurns(Vector3 end, int playerNo, int turns)
    {
        int currentPos;
        if (playerPos[playerNo] == 0)
        {
            currentPos = playerPos[playerNo] + 2;
        }
        else { 
            currentPos = playerPos[playerNo] + 1;
        }

        Vector3 start = playerPieces[playerNo].transform.position;
        List<Vector3> midpoints = new List<Vector3>();
        for (int i = 0; i < turns;)
        {
            if (cornerTiles.Contains(currentPos))
            {
                Vector3 midPoint = GameObject.Find("Tile" + currentPos).transform.position;
                midpoints.Add(midPoint);
                currentPos++;
                i++;
            }
            else
            {
                currentPos++;
            }
            if(currentPos > 40)
            {
                currentPos = 1;
            }
        }
        StartCoroutine(movePiece(start, end, midpoints, playerPieces[playerNo], turns));
    }

    IEnumerator movePiece(Vector3 start, Vector3 end, List<Vector3> midpoints ,GameObject piece, int turns)
    {
        turns = Mathf.Max(turns, 1);
        Vector3 begin = Vector3.zero;
        Vector3 stop;
        for (int i = 0; i < midpoints.Count + 1; i++)
        {
            if (begin.Equals(Vector3.zero)) {
                begin = start;
            }
            if (i == midpoints.Count) {
                stop = end;
            } else {
                stop = new Vector3(midpoints[i].x, 0.2f, midpoints[i].z);
            }

            float t = 0f;
            Debug.Log("At the loop");
            while (t < 1) {
                Debug.Log("Start the loop");
                t += (Time.deltaTime / 1f) * turns;
                piece.transform.position = Vector3.Lerp(begin, stop, t);
                yield return null;
                Debug.Log("Past yield null");
            }
            Debug.Log("Out da loop");
            if (i != midpoints.Count)
            {       //if were at a midpoint rotate the piece
                piece.transform.localRotation = Quaternion.Euler(
                    piece.transform.localRotation.eulerAngles.x,
                    piece.transform.localRotation.eulerAngles.y + 90,
                    piece.transform.localRotation.eulerAngles.z);
            }

            begin = stop;
        }

    }
}
