using UnityEngine;
namespace CampusCats {
public class CampusMusic:MonoBehaviour {
 public AudioSource Source{get;private set;}public bool Ready{get{return Source&&Source.clip&&Source.isPlaying;}}
 void Start(){Source=gameObject.AddComponent<AudioSource>();Source.clip=Resources.Load<AudioClip>("CampusMorning");Source.loop=true;Source.playOnAwake=false;Source.spatialBlend=0;Source.volume=0;Source.ignoreListenerPause=true;if(Source.clip)Source.Play();else Debug.LogError("CAMPUS_MUSIC_MISSING");AudioListener.volume=GameSession.I.volume;}
 void Update(){if(Source)Source.volume=Mathf.MoveTowards(Source.volume,.5f,Time.unscaledDeltaTime*.25f);}
}
}
