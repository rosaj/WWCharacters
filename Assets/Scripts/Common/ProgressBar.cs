using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    private Text text;
    private Image image;

    public Color[] blendColors;
    
	void Start () {
        text = GetComponentInChildren<Text>();
        image = GetComponentsInChildren<Image>()[1];
        
	}
	
	public void SetProgress(float progress, string pbText = null, int colorIndex = -1)
    {
        image.fillAmount = progress / 100;
        if (pbText == null) pbText = progress + "%";
        text.text = pbText;
        if(blendColors != null && blendColors.Length > 0)
        {
            if(colorIndex == -1)
            {
                colorIndex = (int)(blendColors.Length * progress / 100);
                if (colorIndex > blendColors.Length - 1) colorIndex = blendColors.Length - 1;
            }
           
            image.color = blendColors[colorIndex];
        }
    }
}
