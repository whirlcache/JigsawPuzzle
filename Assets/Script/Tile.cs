using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileVo vo;

    private SpriteRenderer sr;

    private BoxCollider box;

    void Awake()
    {
        sr = this.GetComponent<SpriteRenderer>();
        box = this.GetComponent<BoxCollider>();
    }
    public void SetTileVo(TileVo vo)
    {
        this.vo = vo;
        sr.sprite = vo.sprite;
        box.size = vo.sprite.bounds.size;
        box.center = new Vector3(box.size.x / 2, box.size.y / 2, box.size.z);
    }

    public Vector3 Center()
    {
        return box.center;
    }
}
