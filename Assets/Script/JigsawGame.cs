using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum LevelMap {
    Easy = 3,
    Moderate = 4,
    Hard = 5
}


public class JigsawGame : MonoBehaviour
{
    // 难度等级
    public LevelMap level2;

    ArrayList tilesList = new ArrayList();

    Hashtable boardMap = new Hashtable();

    ArrayList posList = new ArrayList();

    ArrayList currRandomPosList = new ArrayList();

    private GameObject tempTile;
    private BoxCollider tempBox;

    private GameObject TilePrefab;

    // 每个难度判断能否移动的距离
    private float LevelDist = 3;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localPosition = new Vector3(-4.0f, -4.0f, 0.0f);
        tempTile = GameObject.Find("SpriteNode/TempTile");
        TilePrefab = GameObject.Find("SpriteNode/TilePrefab");

        tilesList.Clear();
        boardMap.Clear();
        int level = (int)level2;
        byte[] imageBytes = File.ReadAllBytes(Application.dataPath + "/Res/timg.jpg");
        Texture2D sourceTex = new Texture2D(2, 2);
        sourceTex.LoadImage(imageBytes);

        int blockW = (int)sourceTex.width / level;
        int blockH = (int)sourceTex.height / level;

        tempBox = tempTile.GetComponent<BoxCollider>();
        tempBox.size = new Vector3((float)blockW/100, (float)blockH/100, 1f);
        tempBox.center = new Vector3(tempBox.size.x/2, tempBox.size.y/2, 1f);

        if (level2==LevelMap.Easy)
        {
            // 简单
            LevelDist = 3.0f;
        }
        else if (level2==LevelMap.Moderate)
        {
            LevelDist = 2.5f;
        }
        else
        {
            LevelDist = 2.2f;
        }

        int idIndex = 1;
        for (int i = level - 1; i >= 0; i--)
        {
            for (int j = 0; j < level; j++)
            {
                TileVo vo = new TileVo();
                vo.id = idIndex;
                vo.sprite = Sprite.Create(sourceTex, new Rect(j * blockW, i * blockH, blockW, blockH), Vector2.zero);
                float _x = vo.sprite.bounds.size.x;
                float _y = vo.sprite.bounds.size.y;
                posList.Add(new Vector3(j * _x, i * _y, 0));
                tilesList.Add(vo);
                idIndex += 1;
            }
        }

        InitRandomPosition();
        RandomPosition();
    }

    void RandomPosition()
    {
        for (int i=0; i < currRandomPosList.Count; i++)
        {
            int pos = (int)currRandomPosList[i];
            Vector3 position = (Vector3)posList[pos];
            TileVo vo = (TileVo)tilesList[i];

            GameObject go = CreateTile(vo);
            boardMap.Add(pos, vo);//随机后加入盘面记录
            go.transform.localPosition = position;
        }
    }

    GameObject CreateTile(TileVo vo)
    {
        GameObject go = GameObject.Instantiate(TilePrefab);
        go.name = "Tile";
        Tile tile = go.GetComponent<Tile>();
        tile.SetTileVo(vo);
        go.transform.SetParent(this.transform);
        return go;
    }

    void InitRandomPosition()
    {
        //打乱数组
        currRandomPosList.Clear();
        System.Random ra = new System.Random();
        int raValue = ra.Next(tilesList.Count);
        Hashtable hash = new Hashtable();
        while(currRandomPosList.Count<tilesList.Count)
        {
            if (!hash.Contains(raValue))
            {
                currRandomPosList.Add(raValue);
                hash.Add(raValue, raValue);
            }
            raValue = ra.Next(tilesList.Count);
        }
    }

    void MoveTile(GameObject tile)
    {
        Vector3 tempBoxCenterPosition = tempTile.transform.localPosition + tempBox.center;
        Tile tileItem = tile.GetComponent<Tile>();
        Vector3 tileCenterPosition = tile.transform.localPosition + tileItem.Center();
        float dist = Vector3.Distance(tempBoxCenterPosition, tileCenterPosition);
        if (dist < LevelDist)
        {
            // 可移动
            Vector3 clickPosition = new Vector3(tile.transform.localPosition.x, tile.transform.localPosition.y, 0);
            tile.transform.localPosition = tempTile.transform.localPosition;
            tempTile.transform.localPosition = clickPosition;

            // 交换盘面数据
            int pos = GetVoFromBoard(tileItem.vo);
            TileVo tempVo = (TileVo)boardMap[999];
            boardMap[999] = boardMap[pos];
            boardMap[pos] = tempVo;
        }
    }

    int GetVoFromBoard(TileVo vo)
    {
        foreach(DictionaryEntry entry in boardMap)
        {
            int key = (int)entry.Key;
            TileVo tempVo = (TileVo)entry.Value;
            if (tempVo.id == vo.id)
            {
                return key;
            }
        }
        return -1;
    }

    void CheckFinish()
    {
        bool isFinish = true;
        ICollection keys = boardMap.Keys;
        foreach(int key in keys)
        {
            if (key==999)
            {
                continue;
            }
            TileVo tempVo = (TileVo)boardMap[key];
            if (tempVo.id != key)
            {
                isFinish = false;
                break;
            }
        }
        if (isFinish)
        {
            // 好了
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (hitObj.tag=="Tile")
                {
                    MoveTile(hitObj);
                    CheckFinish();
                }
            }
        }
    }
}
