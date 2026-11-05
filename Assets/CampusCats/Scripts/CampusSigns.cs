using UnityEngine;
namespace CampusCats {
[ExecuteInEditMode] public class CampusSigns:MonoBehaviour {
 public string fontName="Microsoft YaHei";
 void Awake(){Font font=Font.CreateDynamicFontFromOSFont(new[]{fontName,"SimHei","Arial"},64);foreach(var text in GetComponentsInChildren<TextMesh>()){text.font=font;text.GetComponent<MeshRenderer>().sharedMaterial=font.material;}}
}
}
