using System.Collections;
using UnityEngine;
using UnityEngine.AI;
namespace CampusCats {
public class SharedEnding:MonoBehaviour {
 public static readonly Vector3 RecordPosition=CampusWorld.Nest+new Vector3(2,0,-2);
 public static readonly Vector3 PortraitCenter=CampusWorld.Nest+Vector3.back*3.5f;
 public int waypoint;public bool recordRead,PortraitActive,PhotoReady,Capturing;public int Countdown;public float FadeAlpha;public Texture2D PortraitPhoto;
 public Vector3 CurrentTarget{get{return RecordPosition;}}public bool ShouldWait{get{return false;}}
 void Start(){var root=new GameObject("秘密地点_照料者的日常").transform;root.position=RecordPosition;var wood=CampusWorld.Mat("照料记录桌",new Color(.6f,.44f,.28f));var paper=CampusWorld.Mat("记录纸",new Color(.96f,.91f,.78f));
  CampusWorld.Shape(PrimitiveType.Cube,"照料记录桌",new Vector3(0,.65f,0),new Vector3(1.8f,.12f,1),wood,root);CampusWorld.Shape(PrimitiveType.Cube,"照料记录",new Vector3(.45f,.75f,0),new Vector3(.65f,.05f,.7f),paper,root);CampusWorld.Shape(PrimitiveType.Cube,"旧照片_猫咪剪影插画",new Vector3(-.45f,.75f,0),new Vector3(.6f,.04f,.5f),paper,root);
  for(int i=0;i<3;i++)CampusWorld.Shape(PrimitiveType.Sphere,"旧照片上的猫剪影",new Vector3(-.62f+i*.17f,.79f,0),new Vector3(.12f,.02f,.23f),CampusWorld.Mat("旧影"+i,GameSession.I.config.cats[i].color),root,false);
 }
 Vector3 Spot(CatBrain c){return PortraitCenter+Vector3.right*(((int)c.id-1)*1.45f);}
 public void Drive(CatBrain c){Vector3 destination=PortraitActive?Spot(c):CampusWorld.Nest+new Vector3(((int)c.id-1)*1.6f,0,-3);if(PortraitActive&&PhotoReady){c.SetState(CatState.Observe);c.transform.rotation=Quaternion.Slerp(c.transform.rotation,Quaternion.LookRotation(Vector3.back),Time.deltaTime*5);return;}if(Vector3.Distance(c.transform.position,destination)>.65f)c.MoveStory(destination,false);else c.SetState(CatState.Observe);}
 public bool CanFinish(Vector3 p){var s=GameSession.I;return s.step>=StoryStep.GatherCats&&s.step<StoryStep.Complete&&!PortraitActive&&Vector3.Distance(p,RecordPosition)<6;}
 public void Finish(){var s=GameSession.I;if(!CanFinish(s.player.transform.position))return;recordRead=true;if(s.step==StoryStep.GatherCats)s.Advance(StoryStep.GatherCats);s.SetPanel("照料记录");}
 public void BeginPortrait(){var s=GameSession.I;if(!recordRead||PortraitActive||s.step!=StoryStep.ReadMemories)return;PortraitActive=true;PhotoReady=false;Capturing=false;s.petting.Finish();s.SetPanel("");s.view.photography=true;s.playerVisual.SetActive(false);Cursor.lockState=CursorLockMode.None;Cursor.visible=true;StartCoroutine(Arrange());}
 IEnumerator Arrange(){yield return new WaitForSeconds(2.2f);if(!PortraitActive)yield break;
  // A short visible dissolve places the friends on verified safe spots. The player
  // never has to chase a distant cat or wait for a broken navigation path.
  for(float t=0;t<.22f;t+=Time.unscaledDeltaTime){FadeAlpha=t/.22f;yield return null;}FadeAlpha=1;
  bool safe=true;foreach(var cat in GameSession.I.cats){NavMeshHit hit;if(!NavMesh.SamplePosition(Spot(cat),out hit,1.5f,NavMesh.AllAreas)){safe=false;continue;}var agent=cat.GetComponent<NavMeshAgent>();if(agent.isOnNavMesh)agent.ResetPath();agent.Warp(hit.position);cat.SetState(CatState.Observe);cat.transform.rotation=Quaternion.LookRotation(Vector3.back);}
  PhotoReady=safe;yield return null;for(float t=0;t<.3f;t+=Time.unscaledDeltaTime){FadeAlpha=1-t/.3f;yield return null;}FadeAlpha=0;if(!safe){GameSession.I.Toast("合照位置暂时不可用，请稍后再试。");CancelPortrait();}
 }
 public void FramePortrait(Camera cam){cam.transform.position=PortraitCenter+new Vector3(0,1.65f,-4.7f);cam.transform.LookAt(PortraitCenter+Vector3.up*.55f);cam.fieldOfView=46;}
 public void TakePortrait(){if(!PortraitActive||!PhotoReady||Capturing)return;StartCoroutine(CountAndShoot());}
 IEnumerator CountAndShoot(){Capturing=true;for(int n=3;n>0;n--){Countdown=n;yield return new WaitForSeconds(1);}Countdown=0;FramePortrait(GameSession.I.view.lens);yield return GameSession.I.photos.TakePhoto();if(PortraitActive){Capturing=false;GameSession.I.Toast("再试一次就好，镜头已经为你对准三位朋友。");}}
 public void CompletePortrait(Texture2D photo){if(!PortraitActive||!photo)return;PortraitPhoto=photo;EndPortrait();GameSession.I.Advance(StoryStep.ReadMemories);GameSession.I.Toast("咔嚓。三位朋友，一张合照，今天的日记写完了。");}
 void EndPortrait(){StopAllCoroutines();PortraitActive=false;PhotoReady=false;Capturing=false;Countdown=0;FadeAlpha=0;var s=GameSession.I;s.view.photography=false;s.playerVisual.SetActive(true);}
 public void CancelPortrait(){EndPortrait();GameSession.I.SetPanel("");GameSession.I.Toast("合照随时可以再拍。靠近记录桌按 E 即可继续。");}
 void Update(){if(PortraitActive&&GameSession.I.Playing&&(Input.GetKeyDown(KeyCode.Space)||Input.GetMouseButtonDown(0)&&!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()))TakePortrait();}
}
}
