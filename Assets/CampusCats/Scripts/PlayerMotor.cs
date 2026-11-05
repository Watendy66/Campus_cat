using UnityEngine;
namespace CampusCats {
[RequireComponent(typeof(CharacterController))]
public class PlayerMotor:MonoBehaviour {
 public float speed;public bool crouching,running,grounded;public float eyeHeight=1.65f;
 public float CrouchBlend{get{return Mathf.InverseLerp(1.8f,1.2f,body.height);}}
 CharacterController body;float gravity;
 void Awake(){body=GetComponent<CharacterController>();body.stepOffset=.22f;body.skinWidth=.035f;body.minMoveDistance=0;}
 public bool CanStand(){
  Vector3 foot=transform.position;
  Collider[] hits=Physics.OverlapCapsule(foot+Vector3.up*.31f,foot+Vector3.up*1.49f,.285f,~0,QueryTriggerInteraction.Ignore);
  foreach(var hit in hits)if(hit!=body&&!hit.transform.IsChildOf(transform))return false;
  return true;
 }
 public void ApplyCrouch(bool request){ApplyCrouch(request,Time.deltaTime);}
 public void ApplyCrouch(bool request,float dt){
  if(!request&&body.height<1.79f&&!CanStand())request=true;
  crouching=request;float height=Mathf.MoveTowards(body.height,request?1.2f:1.8f,3*dt);
  body.height=height;body.center=Vector3.up*(height*.5f);eyeHeight=height-.15f;
 }
 public void Tick(float dt,bool requestCrouch,bool requestRun,Vector2 axes,float yaw){
  var cfg=GameSession.I.config;ApplyCrouch(requestCrouch,dt);running=requestRun&&!crouching;
  Vector3 move=Quaternion.Euler(0,yaw,0)*Vector3.ClampMagnitude(new Vector3(axes.x,0,axes.y),1);
  float requestedSpeed=crouching?cfg.crouchSpeed:running?cfg.runSpeed:cfg.walkSpeed;
  if(move.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(move),dt*10);
  RaycastHit floor;grounded=body.isGrounded||Physics.SphereCast(transform.position+Vector3.up*.28f,.23f,Vector3.down,out floor,.17f,1<<0,QueryTriggerInteraction.Ignore);
  gravity=grounded&&gravity<0?-2:Mathf.Max(-30,gravity-20*dt);
  Vector3 before=transform.position;body.Move((move*requestedSpeed+Vector3.up*gravity)*dt);Vector3 actual=transform.position-before;actual.y=0;speed=actual.magnitude/Mathf.Max(.0001f,dt);
 }
 void Update(){var s=GameSession.I;if(!s||!s.Playing||(s.ending&&s.ending.PortraitActive)||(s.petting&&s.petting.Active)){speed=0;return;}Tick(Time.deltaTime,Input.GetKey(KeyCode.LeftControl),Input.GetKey(KeyCode.LeftShift),new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")),s.view.yaw);}
}
public class CameraRig : MonoBehaviour {
 public float yaw=0,pitch=12; public bool photography; public Camera lens;
 float zoom=55;
 void LateUpdate(){
  GameSession s=GameSession.I;if(!s||!s.player)return;if(s.ending&&s.ending.PortraitActive){s.ending.FramePortrait(lens);return;}
  if(s.Playing&&!(s.petting&&s.petting.Active)){yaw+=Input.GetAxis("Mouse X")*s.config.mouseSensitivity;pitch=Mathf.Clamp(pitch-Input.GetAxis("Mouse Y")*s.config.mouseSensitivity,-25,65);
   if(Input.GetMouseButtonDown(1)){photography=!photography;s.playerVisual.SetActive(!photography);}
   if(photography)zoom=Mathf.Clamp(zoom-Input.GetAxis("Mouse ScrollWheel")*35,25,70);
  }
  Quaternion rot=Quaternion.Euler(pitch,yaw,0);Vector3 pivot=s.player.transform.position+Vector3.up*s.player.eyeHeight;
  Vector3 target=photography?pivot:pivot-rot*Vector3.forward*4.5f+Vector3.up*.5f;
  RaycastHit hit;Vector3 delta=target-pivot;
  if(delta.sqrMagnitude>.01f&&Physics.SphereCast(pivot,.18f,delta.normalized,out hit,delta.magnitude,1<<0,QueryTriggerInteraction.Ignore))target=pivot+delta.normalized*Mathf.Max(0,hit.distance-.15f);
  transform.position=target;transform.rotation=rot;lens.fieldOfView=photography?zoom:60;
 }
}
}
