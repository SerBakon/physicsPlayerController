using NUnit.Framework;
using System.Collections.Generic;
using PurrNet;
using UnityEngine;

public class GameViewManager : MonoBehaviour
{
    [SerializeField] private List<View> allViews = new();
    [SerializeField] private View defaultView;

    private void Awake() {
        InstanceHandler.RegisterInstance(this);

        foreach (var view in allViews) {
            hideViewInternal(view);
        }
        showViewInternal(defaultView);
    }
    private void OnDestroy() {
        InstanceHandler.UnregisterInstance<GameViewManager>();
    }

    public void ShowView<T>(bool hideOthers = true) where T : View {
        foreach (var view in allViews) {
            if (view.GetType() == typeof(T)) {
                showViewInternal(view);
            } else {
                if (hideOthers) { hideViewInternal(view); }
            }
        }
    }
    public void HideView<T>() where T : View {
        foreach (var view in allViews) {
            if (view.GetType() == typeof(T)) { hideViewInternal(view); }
        }
    }

    private void hideViewInternal(View view) {
        view.canvasGroup.alpha = 0;
        view.OnHide();
    }
    private void showViewInternal(View view) {
        view.canvasGroup.alpha = 0;
        view.OnHide();
    }
    
}

public abstract class View : MonoBehaviour {

    public CanvasGroup canvasGroup;

    public abstract void OnShow();
    public abstract void OnHide();
}