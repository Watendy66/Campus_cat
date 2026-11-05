using UnityEngine;
namespace CampusCats {
public enum CatId { Jiuwei, Heibai, Dingliu }
public enum CatState { Idle, Wander, Observe, Approach, Sniff, Eat, Chase, Pounce, Alert, Flee, Recover, Affection, Guide, Finale }
public enum StoryStep { QuietPhoto, FollowWhite, FindBall, PouncePhoto, FeedOrange, LakePhoto, GatherCats, ReadMemories, Complete }
[CreateAssetMenu(menuName="Campus Cats/Game configuration")]
public class CatConfig : ScriptableObject {
 public string gameTitle="校园猫猫观察日记";
 public float walkSpeed=3.2f, runSpeed=6f, crouchSpeed=1.3f, mouseSensitivity=2f;
 public float quietSeconds=7, trustCooldown=6, pounceSeconds=2, followTrust=20, petTrust=35;
 public float foodSeconds=5, foodApproach=3, safeRetreat=3.4f, toyResetSeconds=35;
 public float photoNear=2, photoFar=18, guideWaitDistance=9, cleanupCooldown=35, commissionCooldown=25;
 public float companionTrust=30, scareCooldown=8, scarePenalty=4, companionDistance=3, photoDistance=4.5f;
 public float jiuweiNear=3.2f, jiuweiFar=6, jiuweiApproachSeconds=2, jiuweiQuietSeconds=4, jiuweiFirstReward=8, jiuweiApproachReward=4, jiuweiQuietReward=6, jiuweiFoodReward=12, jiuweiRewardCooldown=6;
 public float friendshipTrust=20;
 public int albumCapacity=30, initialFood=2, supplyReward=2;
 public CatProfile[] cats = new CatProfile[] {
  new CatProfile("九尾",new Color(.94f,.94f,.9f),4.2f,1.5f,2.2f,1f),
  new CatProfile("黑白咪",new Color(.12f,.14f,.16f),2.8f,1.2f,3.4f,.65f),
  new CatProfile("顶流",new Color(.93f,.53f,.19f),2.4f,1.1f,2f,.8f)};
 public GameObject[] catPrefabs=new GameObject[3]; public GameObject studentPrefab; public GameObject femaleStudentPrefab;
 public string[] diary = {"九尾终于允许我安静地为它拍了一张照片。", "跟着九尾走过教学楼侧面，我记下了第一段路线。", "黑白咪一直盯着长椅。我从下面找回了一颗小球。", "黑白咪扑向小球的瞬间被我留下了，花坛旁的小路是第二段线索。", "放下食物后退开，顶流终于安心地吃了起来。", "顶流在湖边等我拍照，第三段路线接上了。", "沿着三段路线，我找到了它们共同的小窝。", "原来它们每天都回到这里。三只猫，一张合照，新的一页校园日常。"};
}
[System.Serializable] public class CatProfile {
 public string name; public Color color; public float safeDistance, panicDistance, speed, reaction;
 public CatProfile(string n,Color c,float s,float p,float v,float r){name=n;color=c;safeDistance=s;panicDistance=p;speed=v;reaction=r;}
}
public static class StoryRules {
 public static bool Photo(StoryStep step,CatState state,bool visible,bool secret,bool quiet,bool trusted,bool lake){
  if(!visible)return false;
  if(step==StoryStep.QuietPhoto)return quiet&&trusted&&state!=CatState.Flee&&state!=CatState.Alert;
  if(step==StoryStep.PouncePhoto)return secret&&state==CatState.Pounce;
  if(step==StoryStep.LakePhoto)return lake&&trusted&&state==CatState.Observe;
  return false;
 }
 public static bool CanAdvance(StoryStep current,StoryStep expected){return current==expected;}
}
}

