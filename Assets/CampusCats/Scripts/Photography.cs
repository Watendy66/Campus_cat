using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CampusCats {
public class PhotoRecord {public Texture2D image;public StoryStep step;public string caption;public bool mission;public float time;}
public class Photography : MonoBehaviour {
 public List<PhotoRecord> album=new List<PhotoRecord>();public Dictionary<StoryStep,Texture2D> diaryPhotos=new Dictionary<StoryStep,Texture2D>();bool capturing;
 public Coroutine TakePhoto(){return StartCoroutine(Shutter());}
 void Update(){GameSession s=GameSession.I;if(s&&s.Playing&&!s.ending.PortraitActive&&s.view.photography&&Input.GetMouseButtonDown(0)&&!capturing)StartCoroutine(Shutter());}
 public bool Visible(CatBrain cat){int seen=0;Vector3 p=cat.transform.position;Vector3[] samples={p+Vector3.up*.65f,p+Vector3.up*.4f+cat.transform.forward*.3f,p+Vector3.up*.4f-cat.transform.forward*.3f};foreach(Vector3 point in samples)if(PointVisible(point,cat.transform))seen++;return seen>=2;}
 public bool PointVisible(Vector3 point,Transform subject){Camera cam=GameSession.I.view.lens;Vector3 v=cam.WorldToViewportPoint(point);if(v.z<.3f||v.x<.02f||v.x>.98f||v.y<.02f||v.y>.98f)return false;
  Vector3 d=point-cam.transform.position;RaycastHit[] hits=Physics.RaycastAll(cam.transform.position,d.normalized,d.magnitude-.03f,~0,QueryTriggerInteraction.Ignore);
  foreach(RaycastHit hit in hits){if(subject&&(hit.transform==subject||hit.transform.IsChildOf(subject)))continue;if(hit.transform.IsChildOf(GameSession.I.player.transform))continue;return false;}return true;
 }
 IEnumerator Shutter(){capturing=true;GameSession s=GameSession.I;
  bool portrait=s.ending.PortraitActive;if(album.Count>=s.config.albumCapacity&&!portrait){s.Toast("相册已满，按 P 删除一张照片后再拍。");capturing=false;yield break;}
  StoryStep step=s.step;bool success=false;string feedback="照片已收录。";
  if(portrait){success=s.ending.PhotoReady;foreach(var cat in s.cats)success&=Visible(cat);feedback=success?"我们的校园合照":"三位朋友还没有完全入画，请重试。";}
  else if(step>=StoryStep.GatherCats){feedback="纪念照片已收录。去树下阅读照料记录，准备三猫合照。";}
  else if(step==StoryStep.QuietPhoto||step==StoryStep.PouncePhoto){CatBrain c=s.cats[step==StoryStep.QuietPhoto?0:step==StoryStep.PouncePhoto?1:2];bool visible=Visible(c);bool secret=Vector3.Distance(c.transform.position,CampusWorld.Secret)<5;
   success=StoryRules.Photo(step,c.state,visible,secret,s.player.speed<.2f,c.trust>=s.config.followTrust,Vector3.Distance(c.transform.position,CampusWorld.Lake)<8);
   feedback=!visible?"目标未入画或主体被遮挡。":step==StoryStep.PouncePhoto?(!secret?"引它到秘密小路附近扑玩。":"错过扑玩动作，捡回小球再试。"):step==StoryStep.QuietPhoto?"停下并保持安全距离，先建立信任。":"让顶流在湖边停下，再拉开距离取景。";
  }
  Transform board=GameObject.Find("公告板").transform;
  bool commission=s.commission&&PointVisible(CampusWorld.Board+Vector3.up*1.6f,board)&&PointVisible(CampusWorld.Board+new Vector3(1,2,0),board)&&Vector3.Distance(s.view.transform.position,CampusWorld.Board)>3;
  // Capture at the shutter, before end-of-frame: UI is a screen overlay and never enters this render target.
  Camera cam=s.view.lens;RenderTexture rt=RenderTexture.GetTemporary(960,540,24);RenderTexture old=RenderTexture.active;RenderTexture previous=cam.targetTexture;
  Texture2D texture=new Texture2D(960,540,TextureFormat.RGB24,false);
  try{cam.targetTexture=rt;cam.Render();RenderTexture.active=rt;texture.ReadPixels(new Rect(0,0,960,540),0,0);texture.Apply();}
  finally{cam.targetTexture=previous;RenderTexture.active=old;RenderTexture.ReleaseTemporary(rt);}
  PhotoRecord record=new PhotoRecord{image=texture,step=step,mission=success,caption=portrait?feedback:success?(step==StoryStep.QuietPhoto?"九尾 · 初次的信任":"黑白咪 · 扑球的瞬间"):feedback,time=Time.time};album.Add(record);
  if(success){diaryPhotos[step]=texture;if(portrait)s.ending.CompletePortrait(texture);else s.Advance(step);}else s.Toast(feedback);
  if(commission)s.items.RewardCommission();capturing=false;yield break;
 }
 public void Delete(int index){if(index<0||index>=album.Count)return;Texture2D t=album[index].image;album.RemoveAt(index);if(!diaryPhotos.ContainsValue(t))Destroy(t);}
 void OnDestroy(){HashSet<Texture2D> all=new HashSet<Texture2D>();foreach(PhotoRecord p in album)all.Add(p.image);foreach(Texture2D t in diaryPhotos.Values)all.Add(t);foreach(Texture2D t in all)if(t)Destroy(t);}
}
}
