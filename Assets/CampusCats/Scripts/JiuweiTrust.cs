using UnityEngine;
namespace CampusCats {
// One bounded rate of progress from real observations; E/Q presses never directly grant trust.
public class JiuweiTrust:MonoBehaviour {
 CatBrain cat;float approachSeconds,quietSeconds,rewardAt=-100,mealAt=-100;bool firstApproach;
 public string hint="蹲下慢走，保持3.2—6米，让九尾先观察你。";
 public int Stage {get{return cat.IsFriend?3:cat.trust>=14?2:cat.trust>=8?1:0;}}
 public string StageName{get{return new[]{"警惕","接受靠近","愿意互动","朋友"}[Stage];}}
 void Awake(){cat=GetComponent<CatBrain>();}
 public void Observe(float dt,float distance,bool crouch,float speed,bool running,bool frightened){
  if(cat.IsFriend){hint="已经是朋友了，自在地一起散步吧。";return;}
  CatConfig cfg=GameSession.I.config;
  if(frightened){hint="它受惊了。退到4米以外，停下或蹲下，等它平静。";return;}
  if(distance<cfg.jiuweiNear){hint="有些太近了，后退到3.2—6米。";return;}
  if(distance>cfg.jiuweiFar){hint="蹲下缓慢靠近九尾，到3.2—6米处停下。";return;}
  if(running||speed>cfg.crouchSpeed+.1f){hint="放开奔跑，按Ctrl蹲下慢走。";return;}
  if(crouch&&speed>.1f){approachSeconds+=dt;quietSeconds=Mathf.Max(0,quietSeconds-dt*.2f);hint="这样慢慢靠近很好，保持一点距离。";
   if(!firstApproach&&approachSeconds>=cfg.jiuweiApproachSeconds){if(Reward(cfg.jiuweiFirstReward,"第一次蹲下慢慢接近"))firstApproach=true;}else if(approachSeconds>=cfg.jiuweiApproachSeconds*3){approachSeconds=0;Reward(cfg.jiuweiApproachReward,"温和地接近");}
  }else if(speed<.15f){quietSeconds+=dt;hint="正在安静陪伴 "+Mathf.FloorToInt(quietSeconds)+" / "+cfg.jiuweiQuietSeconds.ToString("F0")+"秒；也可放食物再退开。";
   if(quietSeconds>=cfg.jiuweiQuietSeconds){quietSeconds=0;Reward(cfg.jiuweiQuietReward,"安静陪伴");}
  }
 }
 public bool Reward(float value,string reason){if(Time.time-rewardAt<GameSession.I.config.jiuweiRewardCooldown)return false;if(!cat.TryAddTrust(value))return false;rewardAt=Time.time;GameSession.I.Toast("九尾 · "+StageName+"："+reason+" +"+value);return true;}
 public void Fed(){if(cat.IsFriend)return;if(Time.time-mealAt<20){hint="它刚吃饱了，先安静陪伴一会儿。";return;}mealAt=Time.time;cat.TryAddTrust(GameSession.I.config.jiuweiFoodReward,true);GameSession.I.Toast("九尾 · "+StageName+"：安心吃完食物 +"+GameSession.I.config.jiuweiFoodReward);}
}
}
