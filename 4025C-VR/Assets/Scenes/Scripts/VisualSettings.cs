using UnityEngine;
using UnityEngine.XR;

public class VisualSettings : MonoBehaviour {

	public void SetRenderScale(float pRenderScale) {
        XRSettings.eyeTextureResolutionScale = pRenderScale;
    }

	public void SetAntialiasing(int pAntialiasingLevel) {
        QualitySettings.antiAliasing = pAntialiasingLevel;
    }

}
