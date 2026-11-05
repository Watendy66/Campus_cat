using UnityEngine;
using UnityEngine.SceneManagement;
namespace CampusCats {
public class GameSession : MonoBehaviour {
 public static GameSession I;public CampusMusic music;public SharedEnding ending;public PetInteraction petting;public CatConfig config;public PlayerMotor player;public GameObject playerVisual;public CameraRig view;public CatBrain[] cats;public ItemSystem items;public Photography photos;public JournalUI ui;
 public StoryStep step;public string panel="主菜单";public string toast="";float toastUntil;public bool commission;public string[] notes=new string[8];public float volume=.7f;
 public StudentGender selectedGender;public bool roleSelected;
 public bool Playing{get{return panel=="";}}
 public static readonly string[] Tasks={"九尾：慢走或蹲下，在安全距离安静相处后拍照", "九尾：跟随到教学楼侧面的观察点", "黑白咪：寻找长椅下的小球", "黑白咪：在秘密小路附近抓拍扑玩", "顶流毛毛：放下食物并后退，等待它吃完", "已移除湖边拍照任务", "三位朋友的故事已收集，去树下阅读照料记录", "读照料记录，和三位朋友拍张合照", "日记完成 · 自由探索和摄影"};
 void Awake(){I=this;}
 void Start(){
  if(!config)config=ScriptableObject.CreateInstance<CatConfig>();
  config=Instantiate(config);config.cats[2].name="顶流毛毛";
  config.diary=new[]{"九尾允许我安静地为它拍照。","九尾带我走过教学楼侧面，我们熟悉了一点。","我帮黑白咪找回了长椅下的小球。","黑白咪扑向小球的瞬间被我留下了。","放下食物后退开，顶流毛毛安心地吃完了。","顶流毛毛在湖边等我拍照。三只猫似乎想带我去一个地方。","我来到树下，看到了旧照片和一本照料记录。","原来它们每天都在这里等校园照料者。九尾喜欢安静，黑白咪喜欢小球，顶流毛毛喜欢吃完后有人陪伴。我们拍下一张合照，把这个温暖的约定写进最后一页日记。"};
  GameObject p=new GameObject("学生");p.transform.position=new Vector3(0,.3f,1);CharacterController cc=p.AddComponent<CharacterController>();cc.height=1.8f;cc.radius=.3f;cc.center=Vector3.up*.9f;player=p.AddComponent<PlayerMotor>();p.layer=2;
  playerVisual=new GameObject("学生外观");playerVisual.transform.SetParent(p.transform,false);Material cloth=CampusWorld.Mat("学生衬衣",new Color(.43f,.63f,.7f)),skin=CampusWorld.Mat("肤色",new Color(.91f,.75f,.61f)),hair=CampusWorld.Mat("头发背包",new Color(.25f,.22f,.2f));
  CampusWorld.Shape(PrimitiveType.Capsule,"衣服",new Vector3(0,.9f,0),new Vector3(.6f,.5f,.4f),cloth,playerVisual.transform,false);CampusWorld.Shape(PrimitiveType.Sphere,"头",new Vector3(0,1.6f,0),Vector3.one*.4f,skin,playerVisual.transform,false);CampusWorld.Shape(PrimitiveType.Sphere,"头发",new Vector3(0,1.72f,-.02f),new Vector3(.42f,.23f,.42f),hair,playerVisual.transform,false);CampusWorld.Shape(PrimitiveType.Cube,"背包",new Vector3(0,1.1f,-.28f),new Vector3(.4f,.5f,.2f),hair,playerVisual.transform,false);
  for(int i=-1;i<=1;i+=2){CampusWorld.Shape(PrimitiveType.Capsule,"裤腿",new Vector3(i*.16f,.3f,0),new Vector3(.2f,.3f,.2f),hair,playerVisual.transform,false);CampusWorld.Shape(PrimitiveType.Capsule,"手臂",new Vector3(i*.38f,.98f,0),new Vector3(.15f,.33f,.15f),cloth,playerVisual.transform,false);}
  view=Camera.main.gameObject.AddComponent<CameraRig>();view.lens=Camera.main;
  cats=new CatBrain[3];Vector3[] spawns={new Vector3(-3,0,12),new Vector3(6,0,13),CampusWorld.Lake};
  for(int i=0;i<3;i++){GameObject cat=new GameObject(config.cats[i].name);cat.transform.position=spawns[i];GameObject model=config.catPrefabs[i]?Instantiate(config.catPrefabs[i],cat.transform):CampusWorld.Placeholder(cat.transform,config.cats[i].color);if(!config.catPrefabs[i]&&i==1)CampusWorld.Shape(PrimitiveType.Sphere,"临时奶牛白斑",new Vector3(.01f,.5f,.15f),new Vector3(.47f,.49f,.3f),CampusWorld.Mat("白斑",Color.white),model.transform,false);if(!model.GetComponent<CatModelAdapter>())model.AddComponent<CatModelAdapter>().placeholder=false;cats[i]=cat.AddComponent<CatBrain>();cats[i].visual=model.transform;cats[i].Init((CatId)i,config.cats[i]);}
  if(config.studentPrefab){Destroy(playerVisual);playerVisual=Instantiate(config.studentPrefab,p.transform);playerVisual.name="学生_已绑定动画";}else playerVisual.AddComponent<StudentPose>();ending=gameObject.AddComponent<SharedEnding>();
  petting=gameObject.AddComponent<PetInteraction>();ConfigureRole(StudentGender.Male);items=gameObject.AddComponent<ItemSystem>();items.Init();photos=gameObject.AddComponent<Photography>();ui=gameObject.AddComponent<JournalUI>();music=gameObject.AddComponent<CampusMusic>();SetPanel("主菜单");
 }
 void Update(){if(!ui)return;if(ending&&ending.PortraitActive){if(Input.GetKeyDown(KeyCode.Escape))ending.CancelPortrait();return;}if(Input.GetKeyDown(KeyCode.Escape))SetPanel(Playing?"暂停":panel=="主菜单"||panel=="选择角色"?"主菜单":"");
  if(Playing&&Input.GetKeyDown(KeyCode.J))SetPanel("手账");else if(Playing&&Input.GetKeyDown(KeyCode.P))SetPanel("相册");

  if(step==StoryStep.LakePhoto)step=StoryStep.GatherCats;
  if(Time.unscaledTime>toastUntil)toast="";
 }
 public void SetVolume(float value){volume=Mathf.Clamp01(value);AudioListener.volume=volume;}
 public void Toast(string message){toast=message;toastUntil=Time.unscaledTime+5;}
 public void SetPanel(string value){if(value==""&&!roleSelected)value="选择角色";panel=value;Time.timeScale=Playing?1:0;Cursor.lockState=Playing?CursorLockMode.Locked:CursorLockMode.None;Cursor.visible=!Playing;}
 public void Advance(StoryStep expected){if(!StoryRules.CanAdvance(step,expected)||step==StoryStep.Complete)return;int n=(int)step;if(n<notes.Length)notes[n]=config.diary[n];step=(StoryStep)(n+1);if(step==StoryStep.LakePhoto)step=StoryStep.GatherCats;if(step==StoryStep.FeedOrange)items.food=Mathf.Max(items.food,1);Toast("日记更新："+notes[n]);if(step==StoryStep.Complete)SetPanel("结局日记");}
 public bool ConfigureRole(StudentGender gender){var prefab=gender==StudentGender.Female?config.femaleStudentPrefab:config.studentPrefab;if(!prefab){Toast("该角色模型尚未接入，请先选择其他角色。");return false;}selectedGender=gender;if(playerVisual){playerVisual.SetActive(false);Destroy(playerVisual);}playerVisual=Instantiate(prefab,player.transform);playerVisual.name=gender==StudentGender.Male?"男性学生":"女性学生_独立模型";playerVisual.AddComponent<StudentAppearance>().Setup(gender);if(view&&view.photography)playerVisual.SetActive(false);return true;}
 public void SelectRole(StudentGender gender){if(!ConfigureRole(gender))return;roleSelected=true;SetPanel("");}
 public bool CanSkipTask {get{return step<StoryStep.GatherCats;}}
 public void SkipCurrentTask(){if(!CanSkipTask)return;int n=(int)step;notes[n]="我暂时跳过了："+Tasks[n]+"（未完成，不记作完成事件）";
  if(step==StoryStep.FindBall||step==StoryStep.PouncePhoto){items.hasBall=true;items.ballOut=false;items.pounceAttempted=false;if(items.ball)items.ball.SetActive(false);}
  if(step==StoryStep.FeedOrange&&items.foodObject){Destroy(items.foodObject);items.foodObject=null;items.food++;}
  foreach(var cat in cats)cat.SetState(CatState.Idle);step=(StoryStep)(n+1);if(step==StoryStep.LakePhoto)step=StoryStep.GatherCats;if(step==StoryStep.FeedOrange)items.food=Mathf.Max(items.food,1);Toast("已跳过当前任务，可继续后续故事。");
 }
 public Vector3 Target(){switch(step){case StoryStep.QuietPhoto:case StoryStep.FollowWhite:return cats[0].transform.position;case StoryStep.FindBall:return items.BallPosition;case StoryStep.PouncePhoto:return CampusWorld.Secret;case StoryStep.FeedOrange:case StoryStep.LakePhoto:return cats[2].transform.position;case StoryStep.GatherCats:return SharedEnding.RecordPosition;default:return SharedEnding.RecordPosition;}}
 public void Restart(){Time.timeScale=1;SceneManager.LoadScene(SceneManager.GetActiveScene().name);}
 void OnDestroy(){Time.timeScale=1;if(I==this)I=null;}
}
}


