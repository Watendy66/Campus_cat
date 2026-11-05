using UnityEngine;
namespace CampusCats {
public class CatModelAdapter : MonoBehaviour {
 public Animator animator;
 public string idle="Idle", walk="Walk", run="Run", eat="Eat", pounce="Pounce", affection="Affection";
 public bool placeholder=true;
 string previous;CatState current;Transform head;Vector3 rest;
 void Awake(){rest=transform.localPosition;if(animator){animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;foreach(var bone in animator.GetComponentsInChildren<Transform>())if(bone.name.ToLower().Contains("head")){head=bone;break;}}}
 public void Apply(CatState state,float speed){
  current=state;if(!animator)return;
  string clip=state==CatState.Pounce?pounce:state==CatState.Eat?eat:state==CatState.Affection?affection:speed>.2f?(speed>2.5f?run:walk):idle;
  if(string.IsNullOrEmpty(clip))clip=speed>.1f?walk:idle;
  animator.speed=clip==walk?Mathf.Clamp(speed/1.5f,.55f,2.1f):1;
  if(clip==previous||string.IsNullOrEmpty(clip))return;
  int hash=Animator.StringToHash(clip);
  if(animator.HasState(0,hash)){animator.CrossFade(hash,.15f);previous=clip;}
 }
 // Downloaded clips cover Idle/Walking. These are explicitly additional procedural gestures.
 void LateUpdate(){if(!GameSession.I||!GameSession.I.Playing||placeholder)return;transform.localPosition=rest+(current==CatState.Pounce?Vector3.up*(.25f+Mathf.Sin(Time.time*7)*.2f):Vector3.zero);if(head&&current==CatState.Eat)head.Rotate(animator.transform.right,20+Mathf.Sin(Time.time*5)*4,Space.World);if(head&&current==CatState.Affection)head.Rotate(animator.transform.right,-10+Mathf.Sin(Time.time*5)*5,Space.World);}
}
}
