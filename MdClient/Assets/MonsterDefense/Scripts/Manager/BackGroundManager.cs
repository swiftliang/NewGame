using UnityEngine;
using System.Collections;

public class BackGroundManager : MonoBehaviour {

    private Transform tower;

    // Use this for initialization
    void Start () {
        if (Data.instance.backGround != null)
            GetComponent<SpriteRenderer>().sprite = Data.instance.backGround;

        transform.position = GameSetting.positionCam;
        Vector2 sprite_size = GetComponent<SpriteRenderer>().sprite.rect.size;
        Vector2 local_sprite_size = sprite_size / GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        transform.localScale = new Vector3(GameSetting.sizeCam.x/local_sprite_size.x,GameSetting.sizeCam.y/local_sprite_size.y);

        tower = transform.GetChild(0);
        tower.position = new Vector2(GameSetting.sizeCam.x/2,tower.transform.position.y);
    }

}
