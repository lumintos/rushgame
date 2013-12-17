using UnityEngine;
using System.Collections;

public class GUISkinManager : MonoBehaviour {

	public GUISkin customSkin;
	public float scalingFactor = 2.0f;
	public static GUISkin Skin {
		get {if (Instance != null) return Instance.actualSkin; return null;}
	}
	
	public static GUISkinManager Instance{ //Static obj is required to make this class static
		get; private set;
	}	
	
	private GUISkin actualSkin;
	
	void Awake(){
		Instance = this;	
	}

	public void UpdateGuiColors(Color color, Color secondaryColor) {
		GUI.skin = customSkin;
		actualSkin = (GUISkin) ScriptableObject.CreateInstance(typeof(GUISkin));

		// Button
		actualSkin.button.normal.background = UpdateGuiSkin(customSkin.button.normal.background, color);
		actualSkin.button.active.background = UpdateGuiSkin(customSkin.button.active.background, color);
		actualSkin.button.normal.textColor = secondaryColor;
		actualSkin.button.active.textColor = secondaryColor;	
		
		// TexField
		actualSkin.textField.normal.background = UpdateGuiSkin(customSkin.textField.normal.background, color);
		actualSkin.textField.focused.background = UpdateGuiSkin(customSkin.textField.focused.background, color);
		actualSkin.textField.active.background = UpdateGuiSkin(customSkin.textField.active.background, color);
		actualSkin.textField.onNormal.background = UpdateGuiSkin(customSkin.textField.onNormal.background, color);

		// Label
		actualSkin.label.normal.textColor = color;	
	}

	private Texture2D UpdateGuiSkin(Texture2D texture, Color primaryColor) {
		Texture2D newTexture = new Texture2D((int)(texture.width / scalingFactor), (int) (texture.height / scalingFactor), texture.format, false);
		for (int i = 0; i < newTexture.width; i++) {
			for (int j = 0; j < newTexture.height; j++) {
				Color color = texture.GetPixelBilinear(((float)i * scalingFactor) / texture.width, ((float)j * scalingFactor) / texture.height) * primaryColor;
				newTexture.SetPixel(i, j, color);
			}
		}
		//newTexture.Apply();
		return newTexture;
	}
}
