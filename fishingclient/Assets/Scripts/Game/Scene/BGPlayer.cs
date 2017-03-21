using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Fishing
{
    public class BGPlayer
    {
        public float BgMusicVolume = 0.5f;
        public float UIBgMusicVolume = 0.24f;
        private int _bgHandle = -1;
        public void PlayLevelBG()
        {
            if (_bgHandle != -1)
                StopBG();
            _bgHandle = Utils.PlaySound(AudioNames.BG_GAME, BgMusicVolume, true);//SceneMgr.Instance.PlaySound (getAudioNameByLevelIndex (index), BgMusicVolume, true);
        }
        public void PlayHallBG()
        {
            if (_bgHandle != -1)
                StopBG();
            _bgHandle = Utils.PlaySound(AudioNames.BG_UI, UIBgMusicVolume, true);//SoundMgr.Instance.PlaySound (AudioNames.BG_UI, UIBgMusicVolume, true);
        }
        public void PlayBG(string strAudioName)
        {
            if (_bgHandle != -1)
                StopBG();
            _bgHandle = Utils.PlaySound(strAudioName, BgMusicVolume, true);//SoundMgr.Instance.PlaySound (strAudioName, BgMusicVolume, true);
        }
        public void StopBG()
        {
            Utils.StopSound(_bgHandle);
            _bgHandle = -1;
        }
    }
}
