using UnityEngine;
using System.Collections;
namespace Fishing
{
    public enum FishType
    {
        NORMAL_FISH,
        END_FISHTYPE,
        CHEST_FISH
    }
    public interface IFish
    {
        //Add by Xhj. OnCreated function
        void OnCreated(int iSize, int iScore);
        void Init();
        void Recycle();
        void Destroy();
        void OnUpDate();
        void OnEscapeUpDate();
        void SetPos(Vector3 pos);
        void SetRotaion(Quaternion rotation);
        void SetPath(Vector3 startPos, Vector3 endPos);
        bool TimeOver();
        bool EscapeTimeOver();
        //Replaced by "OnGotFish" and "OnEscape"
        //void SetEscapePath();
        //void RemoveLottery();
        void OnGotFish();
        void OnEscape();
        int LifeTime { get; set; }
        int CreateTime { get; set; }
        //float Speed { get; set; }
        float Score { get; set; }
        FishType Type { get; set; }
        int ObjType { get; set; }
        //Add by Xhj. Add property, "Mouth" and "FishID"
        Transform Mouth { get; set; }
        int FishID { get; set; }
        //Collider FCollider { get; }
        float EscapeTime { get; set; }
        SingleHook SHook { get; set; }
        Animator FishAnimator { get; set; }
        int Lottery { get; set; }
    }
}
