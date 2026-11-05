using UnityEngine;
namespace CampusCats {
// Two rigid leg segments rotate about explicit hips/knees. The student root is never flattened.
public class StudentPose : MonoBehaviour {
 Transform[] upper=new Transform[2],lower=new Transform[2],arms;Transform torso,head,hair,pack;float blend;
 void Start(){
  foreach(Transform t in transform)if(t.name=="裤腿")t.gameObject.SetActive(false);
  torso=transform.Find("衣服");head=transform.Find("头");hair=transform.Find("头发");pack=transform.Find("背包");
  Material cloth=torso.GetComponent<Renderer>().sharedMaterial;var armList=new System.Collections.Generic.List<Transform>();foreach(Transform t in transform)if(t.name=="手臂")armList.Add(t);arms=armList.ToArray();
  Material skin=head.GetComponent<Renderer>().sharedMaterial;Material dark=hair.GetComponent<Renderer>().sharedMaterial;
  for(int side=-1;side<=1;side+=2){CampusWorld.Shape(PrimitiveType.Sphere,"眼睛",new Vector3(side*.2f,.02f,.47f),Vector3.one*.075f,dark,head,false);CampusWorld.Shape(PrimitiveType.Sphere,"手",new Vector3(side*.38f,.66f,0),new Vector3(.14f,.19f,.14f),skin,transform,false);CampusWorld.Shape(PrimitiveType.Cube,"背包肩带",new Vector3(side*.2f,1.12f,.2f),new Vector3(.06f,.45f,.05f),dark,transform,false);}
  for(int i=0;i<2;i++){
   upper[i]=new GameObject("髋关节"+i).transform;upper[i].SetParent(transform,false);
   CampusWorld.Shape(PrimitiveType.Capsule,"大腿",Vector3.down*.19f,new Vector3(.2f,.21f,.2f),cloth,upper[i],false);
   lower[i]=new GameObject("膝关节"+i).transform;lower[i].SetParent(upper[i],false);lower[i].localPosition=Vector3.down*.38f;
   CampusWorld.Shape(PrimitiveType.Capsule,"小腿",Vector3.down*.19f,new Vector3(.18f,.21f,.18f),cloth,lower[i],false);
   CampusWorld.Shape(PrimitiveType.Cube,"鞋",new Vector3(0,-.35f,.07f),new Vector3(.21f,.13f,.32f),cloth,lower[i],false);
  }
 }
 public void Pose(float value){blend=value;if(!torso)return;
  float hipY=Mathf.Lerp(.8f,.48f,blend);float hipZ=Mathf.Lerp(0,-.16f,blend);
  for(int i=0;i<2;i++){upper[i].localPosition=new Vector3(i==0?-.16f:.16f,hipY,hipZ);upper[i].localRotation=Quaternion.Euler(-55*blend,0,0);lower[i].localRotation=Quaternion.Euler(100*blend,0,0);}
  torso.localPosition=new Vector3(0,1.04f-.36f*blend,.1f*blend);torso.localRotation=Quaternion.Euler(24*blend,0,0);
  head.localPosition=new Vector3(0,1.55f-.6f*blend,.19f*blend);hair.localPosition=head.localPosition+new Vector3(0,.12f,-.02f);pack.localPosition=new Vector3(0,1.1f-.34f*blend,-.28f+.1f*blend);
  if(arms!=null)for(int i=0;i<arms.Length;i++){arms[i].localPosition=new Vector3(i==0?-.38f:.38f,.98f-.3f*blend,.12f*blend);arms[i].localRotation=Quaternion.Euler(25*blend,0,0);}
  foreach(Transform t in transform){if(t.name=="手")t.localPosition=new Vector3(Mathf.Sign(t.localPosition.x)*.38f,.66f-.27f*blend,.26f*blend);if(t.name=="背包肩带")t.localPosition=new Vector3(Mathf.Sign(t.localPosition.x)*.2f,1.12f-.35f*blend,.2f+.1f*blend);}
 }
 void LateUpdate(){if(!GameSession.I||!GameSession.I.Playing)return;Pose(GameSession.I.player.CrouchBlend);}
}
}
