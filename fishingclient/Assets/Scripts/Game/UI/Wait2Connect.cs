using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Fishing.UI
{
    public class Wait2Connect : Window
    {
        private Text _tips;
        public override void Show(ArgList args)
        {
            base.Show(args);
            this._tips = transform.FindChild("tips").GetComponent<Text>();
            this._tips.text = args.GetAt<string>(0);
        }
    }
}
