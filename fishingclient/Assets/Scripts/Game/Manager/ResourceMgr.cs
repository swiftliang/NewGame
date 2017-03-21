using UnityEngine;
using System.Collections.Generic;
namespace Fishing
{
    public class ResourceMgr : Singleton<ResourceMgr>
    {
        private Dictionary<string, AudioClip> _mapSounds = new Dictionary<string, AudioClip>();
        public void LoadResources()
        {
        }
        public void LoadSound()
        {
            var sounds = Resources.LoadAll<AudioClip>("Audio/SFX");
            foreach (AudioClip clip in sounds)
            {
                _mapSounds.Add(clip.name, clip);
            }
            sounds = Resources.LoadAll<AudioClip>("Audio/BgMusic");
            foreach (AudioClip clip in sounds)
            {
                _mapSounds.Add(clip.name, clip);
            }
        }
        public TextAsset LoadInfo(string strName, bool bFromAssetBundle = false)
        {
            if (bFromAssetBundle)
            {
                //todo
                return null;
            }
            else
            {
                return Resources.Load<TextAsset>("GameData/" + strName);
            }
        }
        public GameObject CreateObj(string strName, Transform parent = null, bool bFromBundle = false)
        {
            if (bFromBundle)
            {
                return null;
            }
            else
            {
                GameObject obj = Utils.Instantiate(InfoManager.Instance.GetPath(strName));
                if (obj != null)
                    obj.transform.SetParent(parent, false);
                return obj;
            }
        }
        public Sprite CreatHeadIconSprite(int nId)
        {
            Sprite sprite = Utils.LoadSprite(InfoManager.Instance.GetHeadIconPath(nId));
            return sprite;
        }
        public Texture CreatSceneBgTexture(int nId)
        {
            Texture texture = Utils.LoadTexture(InfoManager.Instance.GetSceneBg(nId+1));
            return texture;
        }
        public AudioClip GetAudioClip(string strName)
        {
            Debug.Assert(_mapSounds.ContainsKey(strName), "SoundClip " + strName + " not exist!");
            return _mapSounds[strName];
        }
    }
}
