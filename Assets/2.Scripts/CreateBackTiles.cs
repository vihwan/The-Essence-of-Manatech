using UnityEngine;

public class CreateBackTiles : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tileBackgroundPrefab;
    public GameObject[,] backTilesBox;
    private const float ReferScreenWidth = 1920f;
    private const float ReferScreenHeight = 1080f;


    // Start is called before the first frame update
    public void Init()
    {
        backTilesBox = new GameObject[width, height];

        tileBackgroundPrefab.GetComponent<RectTransform>().localScale =
           new Vector3(GameManager.instance.ResolutionScale, GameManager.instance.ResolutionScale, 1f);

        Vector2 offset = tileBackgroundPrefab.GetComponent<RectTransform>().sizeDelta;
        float xsize = offset.x * GameManager.instance.ResolutionScale;
        float ysize = offset.y * GameManager.instance.ResolutionScale;

        SetUp(xsize, ysize);
    }

    // Update is called once per frame
    void SetUp(float xOffset, float yOffset)
    {
        //BoardManager 위치에 따라 시작점이 달라짐
        //왼쪽 하단을 기준으로.
        float startX = transform.position.x;
        float startY = transform.position.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newBackTile = Instantiate(tileBackgroundPrefab,
                                                new Vector3(startX + (xOffset * x),
                                                startY + (yOffset * y), transform.position.z),
                                                Quaternion.identity);
                newBackTile.transform.SetParent(transform);
                newBackTile.gameObject.name = "Tile Background [" + x + ", " + y + "]";
                newBackTile.GetComponent<BackgroundTile>().Init(
                    newBackTile.transform.position.x, newBackTile.transform.position.y);
                backTilesBox[x, y] = newBackTile;
            }
        }
    }
}


