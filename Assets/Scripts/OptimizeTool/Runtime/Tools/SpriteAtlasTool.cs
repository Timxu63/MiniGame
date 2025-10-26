using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class SpriteAtlasTool
{
    public static List<Sprite> GetSpriteList(this SpriteAtlas atlas)
    {
        // 1. 根据 spriteCount 分配数组
        int count = atlas.spriteCount;  
        Sprite[] sprites = new Sprite[count];
        
        // 2. 填充数组
        atlas.GetSprites(sprites);  // 数组长度必须 >= spriteCount，否则不会全部填充 :contentReference[oaicite:0]{index=0}
        
        // 3. 转为 List 并返回
        return new List<Sprite>(sprites);
    }
}
