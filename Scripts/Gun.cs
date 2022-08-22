using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform attachTo;

    public OVRSkeleton skeleton;
    public OVRSkeleton.BoneId bone;

    public bool autoFire = false;
    public GameObject projectilePrefab;

    public float initialSpeed = 10;

    [TooltipAttribute("Number of projectiles to send / second.")]
    public float burstRate = 5;

    [TooltipAttribute("Max number of projectile to send in a burst")]
    public int burstCount = 10;


    int burstRemaining = 0;
    float burstNext = 0;

    private void Awake()
    {
        //Debug.Log("awake");
        transform.SetParent(attachTo, false);
        transform.localPosition = Vector3.zero;

    }

    private void OnEnable()
    {
        //Debug.Log("enable");
        if (autoFire)
        {
            Fire();
        }
    }

    private void OnDisable()
    {
        FireStop();
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    Debug.Log("start");
    //}


    IEnumerator Start()
    {
        //OVRSkeleton skeleton = attachTo.GetComponent<OVRSkeleton>();
        if (skeleton)
        {
            Debug.Log("Waiting bones");
            while (skeleton.Bones.Count == 0)
            {
                yield return null;
            }

            foreach (var bone in skeleton.Bones)
            {
                //if (bone.Id == OVRSkeleton.BoneId.Hand_IndexTip)
                if (bone.Id == this.bone)
                {
                    Transform indexTipTransform = bone.Transform;
                    Debug.Log(indexTipTransform);
                    transform.SetParent(indexTipTransform, false);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                    if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft)
                    {
                        transform.Rotate(0, 0, 180, Space.Self);
                    }

                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (burstRemaining > 0 && Time.time >= burstNext)
        {
            FireOne();
        }
    }

    public void Fire()
    {
        if (burstRemaining <= 0)
        {
            burstRemaining = burstCount;
        }

        FireOne();
    }

    public void FireOne()
    {
        if (burstRemaining > 0)
        {
            burstRemaining--;
            burstNext = Time.time + (1 / burstRate);
        }

        Transform spawnPoint = transform.FindChildRecursive("spawn point");
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        //Debug.Log(transform.right);
        rb.AddForce(transform.right * initialSpeed, ForceMode.VelocityChange);
    }

    public void FireStop()
    {
        burstRemaining = 0;
    }

}
