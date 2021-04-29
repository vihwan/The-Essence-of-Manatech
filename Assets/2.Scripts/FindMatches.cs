using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.2f);
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                GameObject currentTile = BoardManager.instance.characterTilesBox[x, y];
                if (currentTile != null)
                {
                    if (x > 0 && x < BoardManager.instance.width - 1)
                    {
                        GameObject leftTile = BoardManager.instance.characterTilesBox[x - 1, y];
                        GameObject rightTile = BoardManager.instance.characterTilesBox[x + 1, y];
                        if (leftTile != null && rightTile != null)
                        {
                            if (currentTile.CompareTag(leftTile.tag) && currentTile.CompareTag(rightTile.tag))
                            {
                                if (!currentMatches.Contains(leftTile))
                                {
                                    currentMatches.Add(leftTile);
                                }
                                leftTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(rightTile))
                                {
                                    currentMatches.Add(rightTile);
                                }
                                rightTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(currentTile))
                                {
                                    currentMatches.Add(currentTile);
                                }
                                currentTile.GetComponent<Tile>().isMatched = true;
                            }
                        }
                    }

                    if (y > 0 && y < BoardManager.instance.height - 1)
                    {
                        GameObject downTile = BoardManager.instance.characterTilesBox[x, y - 1];
                        GameObject upTile = BoardManager.instance.characterTilesBox[x, y + 1];
                        if (downTile != null && upTile != null)
                        {
                            if (currentTile.CompareTag(downTile.tag) && currentTile.CompareTag(upTile.tag))
                            {
                                if (!currentMatches.Contains(downTile))
                                {
                                    currentMatches.Add(downTile);
                                }
                                downTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(upTile))
                                {
                                    currentMatches.Add(upTile);
                                }
                                upTile.GetComponent<Tile>().isMatched = true;
                                if (!currentMatches.Contains(currentTile))
                                {
                                    currentMatches.Add(currentTile);
                                }
                                currentTile.GetComponent<Tile>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}