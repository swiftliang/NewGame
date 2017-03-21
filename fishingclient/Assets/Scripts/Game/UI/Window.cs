using UnityEngine;
using System.Collections;
namespace Fishing.UI
{
    public class Window : MonoBehaviour
    {
        public virtual void OnNotification(Fishing.Event evt)
        {
        }
        public virtual void Show(Fishing.ArgList args)
        {
        }
        public virtual void Close()
        {
            UIManager.Instance.DestroyWindow(this);
        }
    }
}
