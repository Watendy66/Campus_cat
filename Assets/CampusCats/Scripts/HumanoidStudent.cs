using System.Collections.Generic;
using UnityEngine;
namespace CampusCats {
public class HumanoidStudent:MonoBehaviour {
 public bool attentionActive,nativeRig; public Animator animator; public string idle,walk,run;string previous;
 readonly Dictionary<HumanBodyBones,Transform> bones=new Dictionary<HumanBodyBones,Transform>();
 public Transform Bone(HumanBodyBones bone){
  if(!animator)return null;if(animator.isHuman)return animator.GetBoneTransform(bone);
  Transform found;if(bones.TryGetValue(bone,out found))return found;
  string name="";switch(bone){
   case HumanBodyBones.Hips:name="Body";break;case HumanBodyBones.Spine:name="Hips";break;
   case HumanBodyBones.LeftUpperLeg:name="UpperLeg.L";break;case HumanBodyBones.LeftLowerLeg:name="LowerLeg.L";break;case HumanBodyBones.LeftFoot:name="LowerLeg.L_end";break;
   case HumanBodyBones.RightUpperLeg:name="UpperLeg.R";break;case HumanBodyBones.RightLowerLeg:name="LowerLeg.R";break;case HumanBodyBones.RightFoot:name="LowerLeg.R_end";break;
   case HumanBodyBones.LeftUpperArm:name="UpperArm.L";break;case HumanBodyBones.LeftLowerArm:name="LowerArm.L";break;case HumanBodyBones.LeftHand:name="Palm.L";break;
   case HumanBodyBones.RightUpperArm:name="UpperArm.R";break;case HumanBodyBones.RightLowerArm:name="LowerArm.R";break;case HumanBodyBones.RightHand:name="Palm.R";break;
  }
  foreach(var t in animator.GetComponentsInChildren<Transform>())if(t.name==name){found=t;break;}bones[bone]=found;return found;
 }
 public void AttentionPose(){if(!animator)return;foreach(var side in new[]{new[]{HumanBodyBones.LeftUpperArm,HumanBodyBones.LeftLowerArm,HumanBodyBones.LeftHand},new[]{HumanBodyBones.RightUpperArm,HumanBodyBones.RightLowerArm,HumanBodyBones.RightHand}}){var upper=Bone(side[0]);var lower=Bone(side[1]);var hand=Bone(side[2]);if(upper&&lower)upper.rotation=Quaternion.FromToRotation(lower.position-upper.position,Vector3.down)*upper.rotation;if(lower&&hand)lower.rotation=Quaternion.FromToRotation(hand.position-lower.position,Vector3.down)*lower.rotation;}}
 public static void SolveLimb(Transform upper,Transform lower,Transform tip,Vector3 target,Vector3 pole){
  if(!upper||!lower||!tip)return;Vector3 origin=upper.position;float a=Vector3.Distance(origin,lower.position),b=Vector3.Distance(lower.position,tip.position);Vector3 delta=target-origin;float d=Mathf.Clamp(delta.magnitude,.01f,a+b-.0001f);Vector3 direction=delta.normalized;Vector3 bend=Vector3.ProjectOnPlane(pole,direction).normalized;if(bend.sqrMagnitude<.01f)bend=Vector3.ProjectOnPlane(Vector3.forward,direction).normalized;
  float cosine=Mathf.Clamp((a*a+d*d-b*b)/(2*a*d),-1,1);Vector3 knee=origin+direction*a*cosine+bend*a*Mathf.Sqrt(1-cosine*cosine);upper.rotation=Quaternion.FromToRotation(lower.position-origin,knee-origin)*upper.rotation;lower.rotation=Quaternion.FromToRotation(tip.position-lower.position,target-lower.position)*lower.rotation;
 }
 void LateUpdate(){var s=GameSession.I;if(!s||!s.Playing||!animator)return;float blend=s.player.CrouchBlend;string clip=blend>.001f?idle:s.player.speed>.15f?(s.player.running?run:walk):idle;
  if(!string.IsNullOrEmpty(clip)&&clip!=previous){animator.speed=1;animator.CrossFade(clip,.12f);previous=clip;}
  if(s.player.speed<=.15f||blend>.001f){animator.Play(idle,0,0);animator.speed=0;animator.Update(0);attentionActive=blend<=.001f;if(attentionActive){AttentionPose();if(s.petting)s.petting.ApplyHandPose(animator);return;}}else{animator.speed=1;attentionActive=false;}
  if(blend<=0)return;var hips=Bone(HumanBodyBones.Hips);var lf=Bone(HumanBodyBones.LeftFoot);var rf=Bone(HumanBodyBones.RightFoot);if(!hips||!lf||!rf)return;
  if(nativeRig){
   // The source rig animates its shoe bones under the armature root. Keep those native
   // bindings; solve the deforming thigh/shin chains against their original ankles.
   Vector3 left=lf.position,right=rf.position;hips.position-=Vector3.up*(.34f*blend);
   SolveLimb(Bone(HumanBodyBones.LeftUpperLeg),Bone(HumanBodyBones.LeftLowerLeg),lf,left,s.player.transform.forward);
   SolveLimb(Bone(HumanBodyBones.RightUpperLeg),Bone(HumanBodyBones.RightLowerLeg),rf,right,s.player.transform.forward);
   var spine=Bone(HumanBodyBones.Spine);if(spine)spine.Rotate(s.player.transform.right,14*blend,Space.World);
  }else{
   float baseline=Mathf.Min(lf.position.y,rf.position.y);foreach(var side in new[]{new[]{HumanBodyBones.LeftUpperLeg,HumanBodyBones.LeftLowerLeg},new[]{HumanBodyBones.RightUpperLeg,HumanBodyBones.RightLowerLeg}}){var upper=Bone(side[0]);var lower=Bone(side[1]);if(upper)upper.Rotate(s.player.transform.right,-55*blend,Space.World);if(lower)lower.Rotate(s.player.transform.right,100*blend,Space.World);}var spine=Bone(HumanBodyBones.Spine);if(spine)spine.Rotate(s.player.transform.right,20*blend,Space.World);hips.position+=Vector3.up*(baseline-Mathf.Min(lf.position.y,rf.position.y));
  }
  if(s.petting)s.petting.ApplyHandPose(animator);
 }
}
}
