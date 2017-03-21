using UnityEngine;
using System.Collections;
namespace Fishing
{
    public class FishingCamera : SingletonBehaviour<FishingCamera>
    {
        [Header("Camera Setting")]
        public int WIDTH = 1280;
        public int HEIGHT = 720;
        [Header("Cameras")]
        public Camera BGCamera;
        public Camera PlayCamera;
        public Camera UICamera;
        protected override void Init()
        {
            base.Init();
            Utils.AutoScaleCamera(PlayCamera, WIDTH, HEIGHT);
            Utils.AutoScaleCamera(UICamera, WIDTH, HEIGHT);
        }
    }
}
