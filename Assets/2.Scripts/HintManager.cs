using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public float hintDelay;
    public float hintDelaySeconds;
    public GameObject hintEffectPrefab;
    public GameObject currentHintEffect;

    //Component

    //초기화 함수
    public void Init()
    {
        hintDelaySeconds = hintDelay;
    }

    private void Update()
    {
        /*        hintDelaySeconds = Time.deltaTime;
                if (hintDelaySeconds <= 0 && currentHintEffect != null)
                {
                    MarkHint();
                    hintDelaySeconds = hintDelay;
                }*/
    }

    private List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (BoardManager.instance.characterTilesBox[x, y] != null)
                {
                    if (BoardManager.instance.SwitchAndCheck(x, y))
                    {
                        possibleMoves.Add(BoardManager.instance.characterTilesBox[x, y]);
                    }

                    if (BoardManager.instance.SwitchAndCheck(x, y))
                    {
                        possibleMoves.Add(BoardManager.instance.characterTilesBox[x, y]);
                    }
                }
            }
        }

        return possibleMoves;
    }

    private GameObject PickUpRandom()
    {
        List<GameObject> possibleMovesList = new List<GameObject>();

        possibleMovesList = FindAllMatches();
        if (possibleMovesList.Count > 0)
        {
            int tileToUse = Random.Range(0, possibleMovesList.Count);
            return possibleMovesList[tileToUse];
        }
        else
        {
            Debug.Log("옮길 수 있는 타일이 없습니다!!");
        }
        return null;
    }

    public void MarkHint()
    {
        GameObject move = PickUpRandom();
        if (move != null)
        {
            currentHintEffect = Instantiate(hintEffectPrefab, move.transform.position, Quaternion.identity);
            currentHintEffect.transform.SetParent(transform);
        }
    }

    public void DestroyHint()
    {
        if (currentHintEffect != null)
        {
            Destroy(currentHintEffect);
            currentHintEffect = null;
            hintDelaySeconds = hintDelay;
        }
    }
}