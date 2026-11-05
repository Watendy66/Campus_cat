using UnityEngine;
namespace CampusCats {
// Coordinates approach, crouching and hand placement during petting.
public class PetInteraction:MonoBehaviour {
 public CatBrain Target {get;private set;}public bool Active{get;private set;}public bool IsStroking{get{return Active&&phase==1;}}
 public Vector3 HandTarget{get{return Target.transform.position+Vector3.up*.72f+Target.transform.forward*(.23f+Mathf.Sin(elapsed*7)*.045f);}}
 float elapsed,cooldown;int phase;
 public bool Begin(CatBrain cat){var s=GameSession.I;if(Active||Time.time<cooldown)return false;if(Vector3.Distance(s.player.transform.position,cat.transform.position)>2.8f)return false;Target=cat;Active=true;phase=0;elapsed=0;cat.SetState(CatState.Affection);s.Toast("靠近，蹲下轻轻摸摸它……");return true;}
 void Update(){var s=GameSession.I;if(!Active||!s||!s.Playing)return;if(!Target){Finish();return;}elapsed+=Time.deltaTime;
  Vector3 delta=Target.transform.position-s.player.transform.position;delta.y=0;float distance=delta.magnitude;
  if(delta.sqrMagnitude>.01f){s.player.transform.rotation=Quaternion.Slerp(s.player.transform.rotation,Quaternion.LookRotation(delta),Time.deltaTime*10);Target.transform.rotation=Quaternion.Slerp(Target.transform.rotation,Quaternion.LookRotation(-delta),Time.deltaTime*7);}
  var controller=s.player.GetComponent<CharacterController>();
  if(phase==0){if(distance>.7f){controller.Move((delta.normalized*1.2f+Vector3.down*2)*Time.deltaTime);if(elapsed>2.5f){s.Toast("中间有障碍，请靠近一点再摸。");Finish();}return;}phase=1;elapsed=0;}
  s.player.ApplyCrouch(true);controller.Move(Vector3.down*2*Time.deltaTime);if(elapsed>2.8f){s.Toast(Target.name+"放松地蹭了蹭你的手。");Finish();}
 }
 public void ApplyHandPose(Animator animator){if(!IsStroking||!animator)return;var rig=animator.GetComponentInParent<HumanoidStudent>();if(!rig)return;var upper=rig.Bone(HumanBodyBones.RightUpperArm);var lower=rig.Bone(HumanBodyBones.RightLowerArm);var hand=rig.Bone(HumanBodyBones.RightHand);if(!upper||!lower||!hand)return;
  Vector3 target=HandTarget,origin=upper.position;float a=Vector3.Distance(origin,lower.position),b=Vector3.Distance(lower.position,hand.position);Vector3 delta=target-origin;float d=Mathf.Clamp(delta.magnitude,.05f,a+b-.005f);Vector3 direction=delta.normalized;Vector3 outward=GameSession.I.player.transform.right;Vector3 bend=(outward-direction*Vector3.Dot(outward,direction)).normalized;float cosine=Mathf.Clamp((a*a+d*d-b*b)/(2*a*d),-1,1);Vector3 elbow=origin+direction*(a*cosine)+bend*(a*Mathf.Sqrt(1-cosine*cosine));upper.rotation=Quaternion.FromToRotation(lower.position-origin,elbow-origin)*upper.rotation;lower.rotation=Quaternion.FromToRotation(hand.position-lower.position,target-lower.position)*lower.rotation;
 }
 public void Finish(){if(Target&&Target.state==CatState.Affection)Target.SetState(CatState.Idle);Active=false;cooldown=Time.time+3;Target=null;}
}
}
