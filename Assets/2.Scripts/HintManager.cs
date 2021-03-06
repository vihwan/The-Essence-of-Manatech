using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    //public GameObject hintEffectPrefab;
    private GameObject currentHintEffect;

    public GameObject CurrentHintEffect { get => currentHintEffect; set => currentHintEffect = value; }

    //Component

    private List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();

        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (BoardManager.instance.characterTilesBox[x, y] != null)
                {
                    if (BoardManager.instance.characterTilesBox[x, y].CompareTag("Bomb") ||
                        BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed)
                        continue;

                    if (x < BoardManager.instance.width - 1)
                    {
                        if (BoardManager.instance.SwitchingAndCheck(x, y, Vector2.right))
                        {
                            possibleMoves.Add(BoardManager.instance.characterTilesBox[x, y]);
                        }
                    }
                    if (y < BoardManager.instance.height - 1)
                    {
                        if (BoardManager.instance.SwitchingAndCheck(x, y, Vector2.up))
                        {
                            possibleMoves.Add(BoardManager.instance.characterTilesBox[x, y]);
                        }
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
/*            currentHintEffect = Instantiate(hintEffectPrefab, move.transform.position, Quaternion.identity);
            currentHintEffect.transform.SetParent(transform);*/

            CurrentHintEffect = Instantiate(Resources.Load<ParticleSystem>("CFXR Electrified Custom").gameObject,
                                             move.transform.position,
                                             Quaternion.identity,
                                             this.transform);
        }

        BoardManager.instance.currentState = PlayerState.MOVE;
    }

    public void DestroyHint()
    {
        if (CurrentHintEffect != null)
        {
            Destroy(CurrentHintEffect);
            CurrentHintEffect = null;
        }
    }
}