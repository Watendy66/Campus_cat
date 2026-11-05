using UnityEngine;
namespace CampusCats {
public enum StudentGender { Male, Female }
public class StudentAppearance:MonoBehaviour {
 public StudentGender gender;public float measuredHeight;Material uniform,hairMaterial;
 public void Setup(StudentGender value){gender=value;var driver=GetComponent<HumanoidStudent>();if(driver){driver.animator.Play(driver.idle,0,0);driver.animator.Update(0);driver.AttentionPose();}
  Bounds bounds=BodyBounds();if(bounds.size.y>.1f){transform.localScale*=1.75f/bounds.size.y;bounds=BodyBounds();transform.position+=Vector3.up*(transform.parent.position.y-bounds.min.y);}measuredHeight=BodyBounds().size.y;
  // Preserve the native hairstyle and material slots of the female model.
  if(value==StudentGender.Female)return;
  Shader shader=Shader.Find("Campus/UniformPalette");if(!shader)shader=Shader.Find("Standard");uniform=new Material(shader);var skin=GetComponentInChildren<SkinnedMeshRenderer>();if(skin&&skin.sharedMaterial)uniform.mainTexture=skin.sharedMaterial.mainTexture;uniform.SetColor("_UniformColor",new Color(.2f,.42f,.62f));foreach(var renderer in GetComponentsInChildren<SkinnedMeshRenderer>()){var mats=renderer.sharedMaterials;for(int i=0;i<mats.Length;i++)mats[i]=uniform;renderer.sharedMaterials=mats;}
 }
 public Bounds BodyBounds(){var skins=GetComponentsInChildren<SkinnedMeshRenderer>();Bounds bounds=new Bounds(transform.position,Vector3.zero);bool first=true;foreach(var renderer in skins){renderer.updateWhenOffscreen=true;if(first){bounds=renderer.bounds;first=false;}else bounds.Encapsulate(renderer.bounds);}return bounds;}
 void OnDestroy(){if(uniform)Object.Destroy(uniform);if(hairMaterial)Object.Destroy(hairMaterial);}
}
}
