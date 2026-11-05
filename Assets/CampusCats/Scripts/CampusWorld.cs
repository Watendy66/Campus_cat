using UnityEngine;
using UnityEngine.AI;
namespace CampusCats {
public class CampusWorld : MonoBehaviour {
 public static readonly Vector3 Secret=new Vector3(8,0,24), Lake=new Vector3(25,0,38), Nest=new Vector3(-17,0,43), Board=new Vector3(-5,0,3), Bin=new Vector3(1,0,8);
 public static Material Mat(string name,Color c){Material m=new Material(Shader.Find("Standard"));m.name=name;m.color=c;return m;}
 public static GameObject Shape(PrimitiveType type,string name,Vector3 p,Vector3 size,Material m,Transform parent,bool solid=true){
  GameObject o=GameObject.CreatePrimitive(type);o.name=name;o.transform.SetParent(parent,false);o.transform.localPosition=p;o.transform.localScale=size;o.GetComponent<Renderer>().sharedMaterial=m;
  if(!solid)Object.DestroyImmediate(o.GetComponent<Collider>());return o;
 }
 public static void Build(Transform root){
  Material grass=Mat("嫩绿草地",new Color(.59f,.74f,.48f)),stone=Mat("米色步道",new Color(.85f,.8f,.66f)),wall=Mat("浅米墙面",new Color(.95f,.87f,.7f)),wood=Mat("浅木色",new Color(.47f,.32f,.2f)),leaf=Mat("树冠",new Color(.39f,.64f,.43f)),glass=Mat("蓝灰窗",new Color(.4f,.65f,.72f));
  Shape(PrimitiveType.Cube,"校园地面",new Vector3(0,-.3f,25),new Vector3(90,.5f,85),grass,root);
  Shape(PrimitiveType.Cube,"主步道",new Vector3(0,.01f,21),new Vector3(6,.1f,48),stone,root);
  Shape(PrimitiveType.Cube,"庭院步道",new Vector3(10,.03f,24),new Vector3(25,.1f,5),stone,root);
  Shape(PrimitiveType.Cube,"湖边步道",new Vector3(20,.03f,35),new Vector3(5,.1f,25),stone,root);
  Shape(PrimitiveType.Cube,"小窝小路",new Vector3(-9,.04f,43),new Vector3(25,.1f,3),stone,root);
  Shape(PrimitiveType.Cube,"教学楼",new Vector3(-18,4,12),new Vector3(18,8,13),wall,root);
  Shape(PrimitiveType.Cube,"屋檐",new Vector3(-18,8.15f,12),new Vector3(19,.4f,14),wood,root);
  for(int floor=0;floor<2;floor++)for(int i=0;i<6;i++)Shape(PrimitiveType.Cube,"窗框",new Vector3(-25+i*2.8f,2.4f+floor*3.4f,18.55f),new Vector3(1.8f,2,.15f),glass,root);
  Shape(PrimitiveType.Cube,"门",new Vector3(-8.94f,1.6f,12),new Vector3(.15f,3.2f,2),wood,root);
  for(int i=0;i<4;i++)Shape(PrimitiveType.Cylinder,"廊柱",new Vector3(-7.5f,1.7f,7+i*3.5f),new Vector3(.4f,1.7f,.4f),wall,root);
  Shape(PrimitiveType.Cube,"廊顶",new Vector3(-7.3f,3.5f,12),new Vector3(3,.25f,14),wall,root);
  Shape(PrimitiveType.Cylinder,"花坛",new Vector3(10,.3f,16),new Vector3(7,.3f,7),wood,root);
  Shape(PrimitiveType.Cylinder,"花坛绿植",new Vector3(10,.8f,16),new Vector3(6.6f,.4f,6.6f),leaf,root);
  for(int i=0;i<12;i++){float a=i*Mathf.PI/6;Shape(PrimitiveType.Sphere,"花朵",new Vector3(10+Mathf.Cos(a)*2.7f,1.25f,16+Mathf.Sin(a)*2.7f),Vector3.one*.4f,Mat("花色"+i,Color.Lerp(new Color(1,.65f,.64f),Color.white,i/12f)),root,false);}
  Bench(new Vector3(6,0,10),root,wood);Bench(new Vector3(23,0,37),root,wood);
  Shape(PrimitiveType.Cylinder,"湖水（不可进入）",new Vector3(34,.02f,35),new Vector3(15,.08f,23),glass,root,false);
  Shape(PrimitiveType.Cube,"湖岸安全边界",new Vector3(27,.5f,35),new Vector3(.3f,1,24),wood,root);
  for(int i=0;i<9;i++)Shape(PrimitiveType.Cylinder,"湖边栏杆",new Vector3(27,1,24+i*2.8f),new Vector3(.12f,1,.12f),wood,root);
  Shape(PrimitiveType.Cube,"栏杆扶手",new Vector3(27,1.9f,35),new Vector3(.15f,.15f,24),wood,root);
  for(int i=0;i<17;i++){Vector3 pos=new Vector3(i%2==0?-34:36,0,2+i*3);Tree(pos,root,wood,leaf);}
  Tree(new Vector3(-20,0,38),root,wood,leaf);Tree(new Vector3(16,0,43),root,wood,leaf);
  Shape(PrimitiveType.Cube,"小窝背板",Nest+new Vector3(0,.7f,.7f),new Vector3(3,1.4f,.15f),wood,root);
  Shape(PrimitiveType.Cube,"小窝屋顶",Nest+new Vector3(0,1.5f,0),new Vector3(3.5f,.18f,2),wood,root);
  Shape(PrimitiveType.Cube,"小窝软垫",Nest+new Vector3(0,.12f,-.5f),new Vector3(3,.18f,2),wall,root);
  for(int i=-1;i<=1;i+=2)Shape(PrimitiveType.Cube,"小窝支柱",Nest+new Vector3(i*1.45f,.7f,-.8f),new Vector3(.15f,1.4f,.15f),wood,root);
  Shape(PrimitiveType.Cube,"公告板",Board+Vector3.up*1.6f,new Vector3(2.5f,1.5f,.2f),wood,root);
  Shape(PrimitiveType.Cylinder,"公告板支柱",Board+Vector3.up*.65f,new Vector3(.15f,.65f,.15f),wood,root);
  Shape(PrimitiveType.Cylinder,"垃圾桶",Bin+Vector3.up*.55f,new Vector3(.8f,.55f,.8f),glass,root);
  for(int i=0;i<4;i++)Shape(PrimitiveType.Cube,"围界",new Vector3(i<2?(i==0?-44:44):0,1,i<2?25:(i==2?-16:66)),new Vector3(i<2?1:90,2,i<2?84:1),leaf,root);
  GameObject sun=new GameObject("早晨阳光");sun.transform.SetParent(root);sun.transform.rotation=Quaternion.Euler(45,-35,0);Light light=sun.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.05f;light.shadows=LightShadows.Soft;
  RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.8f,.83f,.86f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.81f,.9f,.94f);RenderSettings.fogMode=FogMode.Exponential;RenderSettings.fogDensity=.003f;
  Camera cam=new GameObject("Main Camera").AddComponent<Camera>();cam.tag="MainCamera";cam.transform.SetParent(root);cam.backgroundColor=RenderSettings.fogColor;cam.clearFlags=CameraClearFlags.SolidColor;cam.farClipPlane=180;cam.gameObject.AddComponent<AudioListener>();
 }
 static void Tree(Vector3 p,Transform root,Material wood,Material leaf){Shape(PrimitiveType.Cylinder,"树干",p+Vector3.up*2,new Vector3(.6f,2,.6f),wood,root);Shape(PrimitiveType.Sphere,"圆润树冠",p+Vector3.up*4.5f,new Vector3(4,4,4),leaf,root);}
 static void Bench(Vector3 p,Transform root,Material wood){Shape(PrimitiveType.Cube,"长椅座",p+Vector3.up*.7f,new Vector3(3,.2f,1),wood,root);Shape(PrimitiveType.Cube,"椅背",p+new Vector3(0,1.2f,.45f),new Vector3(3,1,.15f),wood,root);for(int i=-1;i<=1;i+=2)Shape(PrimitiveType.Cube,"椅腿",p+new Vector3(i,.3f,0),new Vector3(.15f,.6f,.8f),wood,root);}
 public static GameObject Placeholder(Transform root,Color color){
  GameObject visual=new GameObject("临时猫模型_待替换");visual.transform.SetParent(root,false);Material fur=Mat("临时猫毛色",color),dark=Mat("猫眼",new Color(.12f,.19f,.14f));
  Shape(PrimitiveType.Sphere,"身体",new Vector3(0,.48f,0),new Vector3(.45f,.48f,.85f),fur,visual.transform,false);
  Shape(PrimitiveType.Sphere,"头",new Vector3(0,.68f,.43f),new Vector3(.44f,.4f,.4f),fur,visual.transform,false);
  for(int i=-1;i<=1;i+=2){Shape(PrimitiveType.Cube,"耳",new Vector3(i*.15f,.91f,.43f),new Vector3(.12f,.21f,.13f),fur,visual.transform,false);Shape(PrimitiveType.Sphere,"眼",new Vector3(i*.12f,.72f,.61f),Vector3.one*.055f,dark,visual.transform,false);for(int j=-1;j<=1;j+=2)Shape(PrimitiveType.Capsule,"腿",new Vector3(i*.16f,.23f,j*.28f),new Vector3(.12f,.23f,.12f),fur,visual.transform,false);}
  GameObject tail=Shape(PrimitiveType.Capsule,"单尾",new Vector3(0,.65f,-.55f),new Vector3(.12f,.4f,.12f),fur,visual.transform,false);tail.transform.localRotation=Quaternion.Euler(-40,0,0);
  visual.AddComponent<CatModelAdapter>();return visual;
 }
}
}
