using UnityEngine;
using UnityEngine.AI;
namespace CampusCats {
public class CatBrain : MonoBehaviour {
 public CatPatrol patrol;public JiuweiTrust learning;
 public bool isFriend;public float FriendThreshold{get{return GameSession.I?GameSession.I.config.friendshipTrust:20;}}
 public bool IsFriend {get{if(!isFriend&&trust>=FriendThreshold)BecomeFriend();return isFriend;}}
 void BecomeFriend(){isFriend=true;trust=FriendThreshold;if(state==CatState.Alert||state==CatState.Flee||state==CatState.Recover)SetState(CatState.Idle);if(GameSession.I)GameSession.I.Toast(name+" 已成为朋友！以后靠近不再扣亲密值。");}
 float repathAt,stalled;Vector3 lastPosition,lastGoal;
 public CatId id;public CatState state;public float trust;public Vector3 home;public Transform visual;
 public float pounceUntil;public bool lakeWaiting,finishedEating;
 NavMeshAgent agent;CatProfile profile;float timer,quiet,trustAt=-100,scareAt=-100,stateAt;int waypoint;CatModelAdapter adapter;
 static readonly Vector3[] Guide={new Vector3(-5,0,10),new Vector3(-5,0,21),new Vector3(0,0,25)};
 public void Init(CatId value,CatProfile p){id=value;profile=p;home=transform.position;agent=gameObject.AddComponent<NavMeshAgent>();agent.radius=.3f;agent.height=1;agent.speed=p.speed;agent.angularSpeed=240;agent.acceleration=7;agent.stoppingDistance=.55f;agent.autoRepath=true;adapter=GetComponentInChildren<CatModelAdapter>();patrol=gameObject.AddComponent<CatPatrol>();patrol.Init(this,agent);if(id==CatId.Jiuwei)learning=gameObject.AddComponent<JiuweiTrust>();}
 public void AddTrust(float amount){TryAddTrust(amount);}
 public bool TryAddTrust(float amount,bool completedMeal=false){
  GameSession s=GameSession.I;if(IsFriend)return false;
  if(amount<0){if(Time.time-scareAt<s.config.scareCooldown)return false;scareAt=Time.time;}
  else if(!completedMeal){if(Time.time-trustAt<s.config.trustCooldown)return false;trustAt=Time.time;}
  trust=Mathf.Clamp(trust+amount,0,FriendThreshold);if(trust>=FriendThreshold){BecomeFriend();return true;}s.Toast(profile.name+" 信任 "+(amount>=0?"+":"")+amount);return true;
 }

 public void SetState(CatState next){if(state==next)return;state=next;if(patrol&&next!=CatState.Wander&&next!=CatState.Observe)patrol.Suspend();timer=0;stateAt=Time.time;if(agent&&agent.isOnNavMesh)agent.ResetPath();}
 void Go(Vector3 p){if(!agent||!agent.isOnNavMesh)return;if(Time.time<repathAt&&Vector3.Distance(p,lastGoal)<.6f&&agent.hasPath){agent.isStopped=false;return;}repathAt=Time.time+.4f;lastGoal=p;NavMeshHit hit;NavMeshPath path=new NavMeshPath();if(NavMesh.SamplePosition(p,out hit,2,NavMesh.AllAreas)&&agent.CalculatePath(hit.position,path)&&path.status==NavMeshPathStatus.PathComplete){agent.isStopped=false;agent.SetPath(path);}else{agent.ResetPath();SetState(CatState.Idle);}}
 void WatchNavigation(float dt){if(!agent.isOnNavMesh){NavMeshHit safe;if(NavMesh.SamplePosition(transform.position,out safe,2,NavMesh.AllAreas))agent.Warp(safe.position);return;}if(!agent.hasPath||agent.pathPending||agent.remainingDistance<agent.stoppingDistance+.2f){stalled=0;lastPosition=transform.position;return;}if(Vector3.Distance(transform.position,lastPosition)>.15f){stalled=0;lastPosition=transform.position;}else stalled+=dt;if(stalled>3){stalled=0;repathAt=0;agent.ResetPath();patrol.Suspend();SetState(CatState.Idle);}}

 bool Arrived{get{return agent&&agent.isOnNavMesh&&!agent.pathPending&&agent.remainingDistance<.7f;}}
 public void Pet(){if(!IsFriend&&trust<GameSession.I.config.petTrust)return;if(GameSession.I.petting)GameSession.I.petting.Begin(this);}
 void Update(){
  GameSession s=GameSession.I;if(!s||!s.Playing)return;float dt=Time.deltaTime;timer+=dt;WatchNavigation(dt);bool friend=IsFriend;float d=Vector3.Distance(transform.position,s.player.transform.position);
  if(adapter)adapter.Apply(state,agent&&agent.isOnNavMesh?agent.velocity.magnitude:0);
  if(visual&&adapter&&adapter.placeholder)visual.localPosition=Vector3.up*(state==CatState.Pounce?Mathf.Sin(Mathf.Clamp01((Time.time-stateAt)/s.config.pounceSeconds)*Mathf.PI)*.65f:0);
  if(s.ending&&s.ending.PortraitActive){s.ending.Drive(this);return;}
  bool scare=!friend&&(d<profile.panicDistance||(s.player.running&&d<profile.safeDistance));
  if(scare&&state!=CatState.Flee&&state!=CatState.Alert&&state!=CatState.Recover){SetState(CatState.Alert);AddTrust(-s.config.scarePenalty);quiet=0;lakeWaiting=false;}
  if(learning&&!friend)learning.Observe(dt,d,s.player.crouching,s.player.speed,s.player.running,state==CatState.Alert||state==CatState.Flee||state==CatState.Recover);
  if(state==CatState.Alert){if(timer>profile.reaction){SetState(CatState.Flee);Go(transform.position+(transform.position-s.player.transform.position).normalized*5);}return;}
  if(state==CatState.Flee){if(timer>3||Arrived)SetState(CatState.Recover);return;}
  if(state==CatState.Recover){if(timer>2&&!scare)SetState(CatState.Idle);return;}
  if(state==CatState.Affection){if(s.petting&&s.petting.Active&&s.petting.Target==this)return;if(timer>2)SetState(CatState.Idle);return;}
  bool safe=d>=profile.safeDistance&&d<profile.safeDistance+3&&s.player.speed<1.5f;
  if(!friend&&id!=CatId.Jiuwei&&safe){quiet+=dt;if(quiet>=s.config.quietSeconds){AddTrust(10);quiet=0;}}else quiet=0;
  if(s.step>=StoryStep.GatherCats){s.ending.Drive(this);return;}
  if(id==CatId.Jiuwei&&s.step==StoryStep.FollowWhite){
   if(waypoint>=Guide.Length){if(d<7)s.Advance(StoryStep.FollowWhite);return;}
   SetState(CatState.Guide);if(d>s.config.guideWaitDistance){if(agent.isOnNavMesh)agent.isStopped=true;}else{Go(Guide[waypoint]);if(Vector3.Distance(transform.position,Guide[waypoint])<1)waypoint++;}return;
  }
  if(id==CatId.Heibai&&s.step==StoryStep.PouncePhoto&&s.items.ballOut){
   if(state==CatState.Pounce){if(Time.time>=pounceUntil){SetState(CatState.Idle);s.items.pounceAttempted=true;}return;}
   if(!s.items.pounceAttempted){SetState(CatState.Chase);Go(s.items.BallPosition);if(Vector3.Distance(transform.position,s.items.BallPosition)<1){SetState(CatState.Pounce);pounceUntil=Time.time+s.config.pounceSeconds;AddTrust(8);}return;}
  }
  if(s.items.foodObject&&s.items.foodTarget==id){
   float foodDist=Vector3.Distance(transform.position,s.items.foodObject.transform.position);
   if(Vector3.Distance(s.player.transform.position,s.items.foodObject.transform.position)<s.config.safeRetreat){SetState(CatState.Observe);return;}
   if(foodDist>1){SetState(CatState.Approach);Go(s.items.foodObject.transform.position);return;}
   if(state!=CatState.Sniff&&state!=CatState.Eat)SetState(CatState.Sniff);
   if(state==CatState.Sniff&&timer>1.5f)SetState(CatState.Eat);
   if(state==CatState.Eat&&timer>s.config.foodSeconds){Destroy(s.items.foodObject);finishedEating=true;if(learning)learning.Fed();else AddTrust(25);if(id==CatId.Dingliu)s.Advance(StoryStep.FeedOrange);}return;
  }
  if(!friend&&learning&&s.step!=StoryStep.FollowWhite&&d<s.config.jiuweiFar&&s.player.crouching&&s.player.speed<=s.config.crouchSpeed+.1f){patrol.Suspend();SetState(CatState.Observe);return;}
  if(friend||trust>=(id==CatId.Jiuwei?50:s.config.companionTrust)){
   float distance=s.view.photography?s.config.photoDistance:Mathf.Max(profile.panicDistance+1,s.config.companionDistance);
   if(s.view.photography&&d<distance-.4f){SetState(CatState.Approach);Go(transform.position+(transform.position-s.player.transform.position).normalized*(distance-d));}
   else if(d>distance+.8f){SetState(CatState.Approach);Go(s.player.transform.position+(transform.position-s.player.transform.position).normalized*distance);}
   else if(!agent.hasPath||agent.remainingDistance<.8f)SetState(CatState.Observe);
   return;
  }
  if(state==CatState.Chase||state==CatState.Pounce||state==CatState.Guide||state==CatState.Approach||state==CatState.Sniff||state==CatState.Eat)SetState(CatState.Idle);
  patrol.Tick(dt);
 }
 public void MoveStory(Vector3 destination,bool wait){SetState(CatState.Guide);if(wait){if(agent.isOnNavMesh)agent.isStopped=true;}else Go(destination);}
 void SetFinale(){Vector3 dest=CampusWorld.Nest+new Vector3(((int)id-1)*1.5f,0,-3);if(Vector3.Distance(transform.position,dest)>.65f){SetState(CatState.Finale);Go(dest);}else{SetState(CatState.Observe);transform.rotation=Quaternion.LookRotation(Vector3.back);}}
}
}
