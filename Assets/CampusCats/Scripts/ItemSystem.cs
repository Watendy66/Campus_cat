using UnityEngine;
using UnityEngine.AI;
namespace CampusCats {
public class ItemSystem : MonoBehaviour {
 public CatId foodTarget;
 public int food,selected=1,trash;public bool hasBall,ballOut,pounceAttempted;public GameObject ball,foodObject;
 public Vector3 BallPosition{get{return ball?ball.transform.position:Vector3.zero;}}
 float thrownAt,foodAt,cleanupAt=-100,commissionAt=-100;GameObject[] litter=new GameObject[3];
 public void Init(){food=GameSession.I.config.initialFood;MakeTrash();ball=CampusWorld.Shape(PrimitiveType.Sphere,"长椅下的小球",new Vector3(6,.2f,10),Vector3.one*.3f,CampusWorld.Mat("小球珊瑚色",new Color(1,.45f,.35f)),transform,false);}
 void MakeTrash(){for(int i=0;i<3;i++){if(litter[i])Destroy(litter[i]);litter[i]=CampusWorld.Shape(PrimitiveType.Cube,"待回收纸屑",new Vector3(-2+i*2,.15f,5+i*2),new Vector3(.3f,.15f,.3f),CampusWorld.Mat("纸",Color.white),transform,false);}}
 void Update(){GameSession s=GameSession.I;if(!s||!s.Playing)return;
  if(Input.GetKeyDown(KeyCode.Alpha1))selected=1;if(Input.GetKeyDown(KeyCode.Alpha2))selected=2;
  if(Input.GetKeyDown(KeyCode.Q)&&!s.view.photography)Use();
  if(Input.GetKeyDown(KeyCode.E)&&!s.view.photography)Interact();
  if(ballOut&&ball&&(Time.time-thrownAt>s.config.toyResetSeconds||ball.transform.position.y<-1)){ball.transform.position=new Vector3(9,.25f,24);s.Toast("小球已安全复位至秘密小路，靠近按 E 捡回。");thrownAt=Time.time;}
  if(foodObject&&Time.time-foodAt>90){Destroy(foodObject);food++;s.Toast("无人进食的食物已收回。");}
  if(trash==0&&Time.time-cleanupAt>s.config.cleanupCooldown&&!litter[0]&&!litter[1]&&!litter[2])MakeTrash();
 }
 public void Use(){GameSession s=GameSession.I;
  if(selected==1){if(food<=0){s.Toast("食物用完了：公告板摄影委托或清洁任务可补给。");return;}if(foodObject){s.Toast("已有一份食物，退开等待猫咪。");return;}
   Vector3 point=s.player.transform.position+s.player.transform.forward*1.6f;NavMeshHit hit;if(!NavMesh.SamplePosition(point,out hit,1,NavMesh.AllAreas)){s.Toast("这里无法放下食物。");return;}
   foodObject=CampusWorld.Shape(PrimitiveType.Cylinder,"食物碗",hit.position+Vector3.up*.12f,new Vector3(.65f,.12f,.65f),CampusWorld.Mat("食物",new Color(.65f,.35f,.2f)),transform,false);food--;foodAt=Time.time;foodTarget=CatId.Jiuwei;float closest=float.MaxValue;foreach(CatBrain cat in s.cats){float distance=Vector3.Distance(cat.transform.position,hit.position);if(distance<closest){closest=distance;foodTarget=cat.id;}}if(s.step==StoryStep.FeedOrange)foodTarget=CatId.Dingliu;s.Toast("食物已放下。后退3.4米，让附近的猫安心靠近。");
  }else{
   if(!hasBall||ballOut){s.Toast(ballOut?"先找到并捡回小球。":"先在长椅下找到小球。");return;}
   Vector3 point=s.player.transform.position+Quaternion.Euler(0,s.view.yaw,0)*Vector3.forward*6;NavMeshHit hit;
   if(!NavMesh.SamplePosition(point,out hit,2,NavMesh.AllAreas)||hit.position.x>26||Vector3.Distance(point,hit.position)>2){s.Toast("这个方向不安全，换个方向投球。");return;}
   ball.transform.position=hit.position+Vector3.up*.2f;ball.SetActive(true);ballOut=true;pounceAttempted=false;thrownAt=Time.time;s.Toast("小球已投出：举起相机等待扑玩瞬间。");
  }
 }
 public string Prompt(){GameSession s=GameSession.I;Vector3 p=s.player.transform.position;
  if(s.ending.CanFinish(p))return "E · 阅读照料记录 / 准备合照";
  if(ball&&ball.activeSelf&&Vector3.Distance(p,BallPosition)<2.6f&&(!hasBall?s.step==StoryStep.FindBall:true))return "E · 捡起小球";
  foreach(GameObject l in litter)if(l&&Vector3.Distance(p,l.transform.position)<2)return "E · 拾取纸屑";
  if(Vector3.Distance(p,CampusWorld.Board)<3)return "E · 领取摄影委托 / 查看补给";
  if(Vector3.Distance(p,CampusWorld.Bin)<2.5f)return "E · 回收纸屑领取食物（3份）";
  foreach(CatBrain c in s.cats)if(Vector3.Distance(p,c.transform.position)<2.8f&&(c.IsFriend||c.trust>=s.config.petTrust))return "E · 摸摸"+s.config.cats[(int)c.id].name;
  return "";
 }
 public void Interact(){GameSession s=GameSession.I;Vector3 p=s.player.transform.position;
  if(s.ending.CanFinish(p)){s.ending.Finish();return;}
  if(ball&&ball.activeSelf&&Vector3.Distance(p,BallPosition)<2.6f&&(!hasBall?s.step==StoryStep.FindBall:true)){hasBall=true;ballOut=false;ball.SetActive(false);s.Advance(StoryStep.FindBall);s.Toast("获得小球，可重复使用。2 选择，Q 投出。");return;}
  for(int i=0;i<litter.Length;i++)if(litter[i]&&Vector3.Distance(p,litter[i].transform.position)<2){Destroy(litter[i]);litter[i]=null;trash++;s.Toast("纸屑 "+trash+" / 3");return;}
  if(Vector3.Distance(p,CampusWorld.Board)<3){if(Time.time-commissionAt<s.config.commissionCooldown){s.Toast("下一份委托稍后开放。也可收集纸屑。");return;}s.commission=true;s.Toast("摄影委托：站远一些，拍摄完整公告板（无需食物）。");return;}
  if(Vector3.Distance(p,CampusWorld.Bin)<2.5f){if(trash<3){s.Toast("收集步道上的三份纸屑再来回收。");return;}trash=0;food+=s.config.supplyReward;cleanupAt=Time.time;s.Toast("清洁完成，获得食物 +"+s.config.supplyReward);return;}
  foreach(CatBrain c in s.cats)if(Vector3.Distance(p,c.transform.position)<2.8f&&(c.IsFriend||c.trust>=s.config.petTrust)){c.Pet();return;}
 }
 public void RewardCommission(){if(!GameSession.I.commission)return;GameSession.I.commission=false;commissionAt=Time.time;food+=GameSession.I.config.supplyReward;GameSession.I.Toast("摄影委托完成，食物 +"+GameSession.I.config.supplyReward);}
}
}
