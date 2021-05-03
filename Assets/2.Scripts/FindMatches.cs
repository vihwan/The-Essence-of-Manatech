using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> currentMatches = new List<GameObject>();

    private int lolipopCount = 0;

    public int LolipopCount { get => lolipopCount; set => lolipopCount = value; }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        LolipopCount = 0;
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
                            // 1. 현재 타일이 롤리팝 타일이면
                            if (currentTile.CompareTag("Lolipop"))
                            {
                                //비교하는 타일중에 롤리팝이 또 있다면
                                if (leftTile.CompareTag("Lolipop") || rightTile.CompareTag("Lolipop"))
                                {
                                    AddMatching(currentTile, leftTile, rightTile);
                                    LolipopCount += 2;
                                }
                                else if (leftTile.tag == rightTile.tag)
                                {
                                    AddMatching(currentTile, leftTile, rightTile);
                                    LolipopCount++;
                                }
                            }
                            // 2. 현재 타일이 롤리팝이 아니지만
                            else if (!currentTile.CompareTag("Lolipop"))
                            {
                                // 내 옆의 타일 중 하나가 롤리팝이라면
                                if (leftTile.CompareTag("Lolipop") || rightTile.CompareTag("Lolipop"))
                                {
                                    //자신하고 나머지 타일이 서로 같아야한다.
                                    if (leftTile.CompareTag(currentTile.tag))
                                    {
                                        AddMatching(currentTile, leftTile, rightTile);
                                        LolipopCount++;
                                    }
                                    else if (rightTile.CompareTag(currentTile.tag))
                                    {
                                        AddMatching(currentTile, leftTile, rightTile);
                                        LolipopCount++;
                                    }
                                }
                                // 3. 타일 3개가 전부 캔디가 아닐 때
                                else
                                {
                                    //3개의 타일이 서로 같아야한다.
                                    if (currentTile.CompareTag(leftTile.tag) && currentTile.CompareTag(rightTile.tag))
                                    {
                                        AddMatching(currentTile, leftTile, rightTile);
                                    }
                                }
                            }
                        }
                    }

                    if (y > 0 && y < BoardManager.instance.height - 1)
                    {
                        GameObject downTile = BoardManager.instance.characterTilesBox[x, y - 1];
                        GameObject upTile = BoardManager.instance.characterTilesBox[x, y + 1];
                        if (downTile != null && upTile != null)
                        {
                            // 1. 현재 타일이 롤리팝 타일이면
                            if (currentTile.CompareTag("Lolipop"))
                            {
                                if (downTile.CompareTag("Lolipop") || upTile.CompareTag("Lolipop"))
                                {
                                    AddMatching(currentTile, downTile, upTile);
                                    LolipopCount += 2;
                                }
                                else if (downTile.tag == upTile.tag)
                                {
                                    AddMatching(currentTile, downTile, upTile);
                                    LolipopCount++;
                                }
                            }
                            // 2. 현재 타일이 롤리팝이 아니지만
                            else if (!currentTile.CompareTag("Lolipop"))
                            {
                                // 내 옆의 타일 중 하나가 롤리팝이라면
                                if (downTile.CompareTag("Lolipop") || upTile.CompareTag("Lolipop"))
                                {
                                    //자신하고 나머지 타일이 서로 같아야한다.
                                    if (downTile.CompareTag(currentTile.tag))
                                    {
                                        AddMatching(currentTile, downTile, upTile);
                                        LolipopCount++;
                                    }
                                    else if (upTile.CompareTag(currentTile.tag))
                                    {
                                        AddMatching(currentTile, downTile, upTile);
                                        LolipopCount++;
                                    }
                                }
                                // 3. 타일 3개가 전부 캔디가 아닐 때
                                else
                                {
                                    //3개의 타일이 서로 같아야한다.
                                    if (currentTile.CompareTag(downTile.tag) && currentTile.CompareTag(upTile.tag))
                                    {
                                        AddMatching(currentTile, downTile, upTile);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }//coroutine

    private void AddMatching(GameObject currentTile, GameObject firstTile, GameObject secondTile)
    {
        if (!currentMatches.Contains(firstTile))
        {
            currentMatches.Add(firstTile);
        }
        firstTile.GetComponent<Tile>().isMatched = true;

        if (!currentMatches.Contains(secondTile))
        {
            currentMatches.Add(secondTile);
        }
        secondTile.GetComponent<Tile>().isMatched = true;
        if (!currentMatches.Contains(currentTile))
        {
            currentMatches.Add(currentTile);
        }
        currentTile.GetComponent<Tile>().isMatched = true;
    }
}