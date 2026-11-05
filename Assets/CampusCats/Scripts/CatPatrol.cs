using UnityEngine;
using UnityEngine.AI;
namespace CampusCats {
[System.Serializable] public class PatrolArea {
 public Vector3[] points;public float pauseMin=3,pauseMax=6;
 public PatrolArea(Vector3[] values,float min,float max){points=values;pauseMin=min;pauseMax=max;}
}
public class CatPatrol:MonoBehaviour {
 public static Vector3 P(float x,float z){return new Vector3(x,0,z);}
 public static PatrolArea Default(CatId id){
  if(id==CatId.Jiuwei)return new PatrolArea(new[]{P(-3,12),P(-5,23),P(-20,28),P(-34,35),P(-34,55),P(-20,46),P(0,43),P(4,26)},2,4);
  if(id==CatId.Heibai)return new PatrolArea(new[]{P(6,13),P(0,24),P(8,28),P(18,24),P(20,32),P(16,43),P(2,43),P(-5,24)},2,4);
  return new PatrolArea(new[]{P(25,38),P(23,45),P(20,54),P(12,60),P(0,43),P(12,39),P(20,32)},2,5);
 }
 public PatrolArea area;public int visited;public Vector3 destination;public bool travelling;
 NavMeshAgent agent;CatBrain brain;int index;float pause,stuck;Vector3 last;
 public void Init(CatBrain c,NavMeshAgent a){brain=c;agent=a;area=Default(c.id);pause=1;last=transform.position;}
 public void Tick(float dt){
  if(!agent.isOnNavMesh)return;
  if(travelling){
   if(Vector3.Distance(transform.position,last)<.12f)stuck+=dt;else{stuck=0;last=transform.position;}
   if(!agent.pathPending&&(agent.pathStatus!=NavMeshPathStatus.PathComplete||agent.remainingDistance<.65f)||stuck>3){travelling=false;visited++;pause=Random.Range(area.pauseMin,area.pauseMax);stuck=0;brain.SetState(CatState.Observe);}
   return;
  }
  pause-=dt;if(pause>0)return;
  for(int attempt=0;attempt<area.points.Length;attempt++){
   Vector3 point=area.points[index++%area.points.Length];NavMeshHit hit;NavMeshPath path=new NavMeshPath();
   if(!NavMesh.SamplePosition(point,out hit,1.5f,NavMesh.AllAreas)||Mathf.Abs(hit.position.y-transform.position.y)>1.5f||!agent.CalculatePath(hit.position,path)||path.status!=NavMeshPathStatus.PathComplete)continue;
   destination=hit.position;brain.SetState(CatState.Wander);agent.isStopped=false;agent.SetPath(path);travelling=true;last=transform.position;return;
  }
  pause=2;
 }
 public void Suspend(){if(travelling){travelling=false;pause=.5f;}stuck=0;}
}
}
