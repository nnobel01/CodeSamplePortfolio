using System.Linq;
using Assets.BulletDecals.Scripts.Decals;
using Assets.BulletDecals.Scripts.Raycast;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Assets.BulletDecals.Scripts.Bullets
{
    /// <summary>
    /// Creates decal on object with collider that in front of the bullet
    /// </summary>
    [RequireComponent(typeof(Gun))]
    public class Bullet : MonoBehaviour
    {
        //bool for target is hit


        [Tooltip("Allow Random Rotation of bullet marks")]
        public bool RandomRotation = true;

        [Tooltip("LayerMask for Raycast")]
        public LayerMask HitLayerMask;

        [Tooltip("Distance for Raycast")]
        public float MaxDistance = Mathf.Infinity;

        [Tooltip("Trigger interaction for Raycast")]
        public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.UseGlobal;

        [Tooltip("Force applied to hit object")]
        public float Force = 0.0f;

        [Tooltip("Scale of the bullet mark")]
        public float Scale = 1.0f;

        /// <summary>
        /// Settings for bullet marks
        /// </summary>
        public BulletMarksSettings BulletMarksSettings { get; set; }

        private bool _isShooting;

        public static Vector3 laserCoordinates;

        public static bool isMissed;

        [SerializeField]
        private Camera cameraToShootFrom;

        public void Shoot()
        {
            _isShooting = true;

        }

        private void Awake()
        {
            isMissed = false;
        }

        private void FixedUpdate()
        {
            //do shooting inside FixedUpdate to make sure that all transforms are up to date
            if (_isShooting)
            {
                _isShooting = false;

#if UNITY_IOS
                PluginManager.resetDetectedLaser();

                laserCoordinates = new Vector3(PluginManager.getCoordinateX(), PluginManager.getCoordinateY(), 0);
#elif UNITY_ANDROID
                Gun.didDetectLaser = false;

#elif UNITY_EDITOR
                //var ray = new Ray(transform.position, transform.forward);
                laserCoordinates = Input.mousePosition;
#endif

                Ray ray = cameraToShootFrom.ScreenPointToRay(laserCoordinates);

                RaycastHit hitInfo;

                //find intersection with any colliders in front of the bullet
                if (Physics.Raycast(ray, out hitInfo, MaxDistance, HitLayerMask.value, TriggerInteraction))
                {
                    if (hitInfo.collider is TerrainCollider)
                    {
                        SearchForIntersectionWithTerrain(hitInfo);
                    }
                    else
                    {
                        SearchForIntersectionWithMesh(hitInfo, ray, true);

                        string scene = SceneManager.GetActiveScene().name;


                        if (hitInfo.collider.CompareTag("NeuroTarget") && UIManager.isSession)
                        {
                            //targethitreaction.isShot = true;
                            GameObject target = hitInfo.collider.gameObject;
                            target.GetComponent<TargetHitReaction>().isShot = true;

                        }
                        else if (hitInfo.collider.CompareTag("CircularTarget") && UIManager.isSession)
                        {
                            //targethitreaction.isShot = true;
                            GameObject target = hitInfo.collider.gameObject;
                            target.GetComponent<CircularTargetManager>().isShot = true;
                        }
                        else if (hitInfo.collider.CompareTag("PauseTarget") && UIManager.isSession)
                        {
                            //targethitreaction.isShot = true;
                            GameObject target = hitInfo.collider.gameObject;
                            target.GetComponent<PauseTargetManager>().isShot = true;
                        }
                        else if (hitInfo.collider.CompareTag("MatchTarget") && UIManager.isSession)
                        {
                            //targethitreaction.isShot = true;
                            GameObject target = hitInfo.collider.gameObject;
                            target.GetComponent<MatchupTargetManager>().isShot = true;


                            MatchupTargetManager.HitList.Add(target.transform.GetChild(0).GetComponent<TextMeshPro>().text);
                        }
                        else if (SceneManager.GetActiveScene().name.Contains("Match"))
                        {
                            Debug.Log("surface");
                            MatchupTargetManager.HitList.Clear();
                            NeuroScore.identifyScore -= 35;
                            isMissed = true;

                        }
                        else if (hitInfo.collider.CompareTag("ActionTarget") && UIManager.isSession)
                        {

                            GameObject target = hitInfo.collider.gameObject;

                            //Dueling Tree Checks
                            if (target.name == "Paddle1")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._1isRight)
                                {
                                    DuelingTreeManager._1isLeft = true;
                                    DuelingTreeManager._1isRight = false;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180);


                                }
                                else if (DuelingTreeManager._1isLeft)
                                {
                                    DuelingTreeManager._1isLeft = false;
                                    DuelingTreeManager._1isRight = true;

                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);


                                }
                            }

                            else if (target.name == "Paddle2")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._2isLeft)
                                {
                                    DuelingTreeManager._2isRight = true;
                                    DuelingTreeManager._2isLeft = false;
                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                                }
                                else if (DuelingTreeManager._2isRight)
                                {
                                    DuelingTreeManager._2isRight = false;
                                    DuelingTreeManager._2isLeft = true;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                                }

                            }
                            else if (target.name == "Paddle3")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._3isLeft)
                                {
                                    DuelingTreeManager._3isRight = true;
                                    DuelingTreeManager._3isLeft = false;
                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                                }
                                else if (DuelingTreeManager._3isRight)
                                {
                                    DuelingTreeManager._3isRight = false;
                                    DuelingTreeManager._3isLeft = true;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                                }
                            }

                            else if (target.name == "Paddle4")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._4isLeft)
                                {
                                    DuelingTreeManager._4isRight = true;
                                    DuelingTreeManager._4isLeft = false;
                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                                }
                                else if (DuelingTreeManager._4isRight)
                                {
                                    DuelingTreeManager._4isRight = false;
                                    DuelingTreeManager._4isLeft = true;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                            }

                            else if (target.name == "Paddle5")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._5isLeft)
                                {
                                    DuelingTreeManager._5isRight = true;
                                    DuelingTreeManager._5isLeft = false;
                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

                                }
                                else if (DuelingTreeManager._5isRight)
                                {
                                    DuelingTreeManager._5isRight = false;
                                    DuelingTreeManager._5isLeft = true;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                                }
                            }

                            else if (target.name == "Paddle6")
                            {
                                Debug.Log(target.name);
                                if (DuelingTreeManager._6isLeft)
                                {
                                    DuelingTreeManager._6isRight = true;
                                    DuelingTreeManager._6isLeft = false;
                                    target.transform.localPosition = new Vector3(.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

                                }
                                else if (DuelingTreeManager._6isRight)
                                {
                                    DuelingTreeManager._6isRight = false;
                                    DuelingTreeManager._6isLeft = true;
                                    target.transform.localPosition = new Vector3(-.15f, target.transform.localPosition.y, target.transform.localPosition.z);
                                    target.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                                }
                            }

                        }
                        else if (hitInfo.collider.CompareTag("SoloPlateTarget") && UIManager.isSession)
                        {
                            Debug.Log("Plate Hit");
                            GameObject target = hitInfo.collider.gameObject;
                            if (target.name == "Plate1Solo")
                            {
                                Debug.Log("Plate 1 Hit");
                                PlateRack._1isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate2Solo")
                            {
                                PlateRack._2isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate3Solo")
                            {
                                PlateRack._3isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate4Solo")
                            {
                                PlateRack._4isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }

                            else if (target.name == "Plate5Solo")
                            {
                                PlateRack._5isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate6Solo")
                            {
                                PlateRack._6isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate7")
                            {
                                PlateRack._7isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate8")
                            {
                                PlateRack._8isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate9")
                            {
                                PlateRack._9isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate10")
                            {
                                PlateRack._10isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate11")
                            {
                                PlateRack._11isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                            else if (target.name == "Plate12")
                            {
                                PlateRack._12isDown = true;
                                target.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
                            }
                        }

                        else if (UIManager.isSession)
                        {
                            if (hitInfo.collider.CompareTag("Target") && scene != "TableIVPhaseOne" && scene != "TableIVPhaseTwo" && scene != "TableIVPhaseThree" && scene != "TableIVPhaseFour" && scene != "5toGo" && scene != "Accelerator" && scene != "OuterLimits" && scene != "Pendulum" && scene != "RoundAbout" && scene != "Showdown" && scene != "SmokeandHope" && scene != "SpeedOption" && !UIManager.isDisplayingTargets)
                            {
                                GameObject target = hitInfo.collider.gameObject;
                                target.transform.localRotation = Quaternion.Euler(180, target.transform.rotation.y, target.transform.rotation.z);
                            }

                            switch (hitInfo.collider.gameObject.transform.parent.tag)
                            {
                                case "Lane1":
                                    ScoreManager.lane1Score++;
                                    break;
                                case "Lane2":
                                    ScoreManager.lane2Score++;
                                    break;
                                case "Lane3":
                                    ScoreManager.lane3Score++;
                                    break;
                                case "Lane4":
                                    ScoreManager.lane4Score++;
                                    break;
                                case "Lane5":
                                    ScoreManager.lane5Score++;
                                    break;
                            }
                        }
                        
                    }
                }
                //StopHitDelayCalculator();
            }
        }

        /// <summary>
        /// Searches for intersection with terrain and creates decal on it
        /// </summary>
        /// <param name="hitInfo"></param>
        private void SearchForIntersectionWithTerrain(RaycastHit hitInfo)
        {
            var terrain = hitInfo.transform.GetComponent<Terrain>();
            if (terrain != null)
            {
                CreateDecalOnTerrain(terrain, hitInfo.point, hitInfo.normal);
            }
        }

        /// <summary>
        /// Searches for intersection with object that has mesh on it
        /// </summary>
        /// <param name="hitInfo"></param>
        /// <param name="ray"></param>
        /// <param name="recursiveIntersection"></param>
        private void SearchForIntersectionWithMesh(RaycastHit hitInfo, Ray ray, bool recursiveIntersection)
        {

            var targetTransform = hitInfo.transform;

            var meshFilter = targetTransform.GetComponentInParent<MeshFilter>() ??
                             targetTransform.GetComponentInChildren<MeshFilter>();

            ApplyForce(targetTransform, ray.direction, hitInfo.point);

            CreateDecal(meshFilter, ray, recursiveIntersection);
        }

        /// <summary>
        /// Applies Force to target if it has rigidbody, so it will be moved by bullet
        /// </summary>
        /// <param name="target"></param>
        /// <param name="forceDirection"></param>
        /// <param name="forcePosition"></param>
        private void ApplyForce(Transform target, Vector3 forceDirection, Vector3 forcePosition)
        {
            if (Force > 0)
            {
                var targetRigidbody = target.GetComponent<Rigidbody>();
                if (targetRigidbody != null)
                {
                    var force = forceDirection * Force;
                    targetRigidbody.AddForceAtPosition(force, forcePosition);
                }
            }
        }

        /// <summary>
        /// Creates bullet hole decal on terrain
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="intersectionPoint"></param>
        /// <param name="intersectionNormal"></param>
        private void CreateDecalOnTerrain(Terrain terrain, Vector3 intersectionPoint, Vector3 intersectionNormal)
        {
            var targetTransform = terrain.transform;

            var bulletMark = PrepareBulletMark(intersectionPoint, intersectionNormal, targetTransform);
            if (bulletMark == null)
            {
                return;
            }

            //create geometry
            var meshDecal = bulletMark.GetComponent<MeshDecal>();
            meshDecal.CreateDecalFromTerrain(terrain, intersectionPoint);

            //attach to target object
            bulletMark.parent = targetTransform;
        }

        /// <summary>
        /// Initialize bullet mark to create decal on target surface
        /// </summary>
        /// <param name="intersectionPoint"></param>
        /// <param name="intersectionNormal"></param>
        /// <param name="targetTransform"></param>
        /// <returns></returns>
        private Transform PrepareBulletMark(Vector3 intersectionPoint, Vector3 intersectionNormal, Transform targetTransform)
        {
            var objectTag = targetTransform.gameObject.tag;

            var bulletMark = BulletMarksSettings.GetBulletMarkByTag(objectTag, intersectionPoint,
                Quaternion.FromToRotation(Vector3.up, intersectionNormal));
            if (bulletMark == null)
            {
                return null;
            }

            //do random rotation on target surface
            if (RandomRotation)
            {
                bulletMark.Rotate(Vector3.up, Random.Range(0f, 360f));
            }

            bulletMark.localScale *= Scale;

            //create impact effect if available
            BulletMarksSettings.GetImpactEffectByTag(objectTag, intersectionPoint, bulletMark.rotation);

            return bulletMark;
        }

        /// <summary>
        /// Creates decal on gameobject with MeshFilter
        /// </summary>
        /// <param name="meshFilter"></param>
        /// <param name="ray"></param>
        /// <param name="recursiveIntersection"></param>
        private void CreateDecal(MeshFilter meshFilter, Ray ray, bool recursiveIntersection)
        {
            if (meshFilter != null)
            {
                var targetTransform = meshFilter.transform;

                Vector3 intersectionPoint;
                Vector3 intersectionNormal;

                //try to find intersection point of the ray on mesh
                if (RaycastMesh.FindIntersectionPoint(ray, meshFilter, out intersectionPoint,
                    out intersectionNormal))
                {
                    switch (targetTransform.position.z)
                    {
                        case 350:
                            intersectionPoint.y += 0.084f;
                            break;
                        case 300:
                            intersectionPoint.y += 0.132f;
                            break;
                        case 250:
                            intersectionPoint.y += 0.144f;
                            break;
                        case 200:
                            intersectionPoint.y += 0.156f;
                            break;
                        case 150:
                            intersectionPoint.y += 0.168f;
                            break;
                        case 100:
                            intersectionPoint.y += 0.0f;
                            break;
                    }

                    var bulletMark = PrepareBulletMark(intersectionPoint, intersectionNormal, targetTransform);
                    if (bulletMark == null)
                    {
                        return;
                    }
                    //create geometry
                    var meshDecal = bulletMark.GetComponent<MeshDecal>();
                    meshDecal.CreateDecal(targetTransform);

                    //attach to target object
                    bulletMark.parent = targetTransform;
                }
                else if (recursiveIntersection)
                {
                    //find other objects behind current collider to find intersection with one of them
                    RaycastHit[] hits = Physics.RaycastAll(ray, MaxDistance, HitLayerMask.value, TriggerInteraction);
                    if (hits.Length > 1)
                    {
                        var hitInfo = hits.OrderBy(p => p.distance).ElementAt(1);

                        if (hitInfo.collider is TerrainCollider)
                        {
                            SearchForIntersectionWithTerrain(hitInfo);
                        }
                        else
                        {
                            SearchForIntersectionWithMesh(hitInfo, ray, false);
                        }
                    }
                }
            }
        }
        private static void StopHitDelayCalculator()
        {
            UIManager.isLaserPointed = false;
        }


    }


}