using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONReader : MonoBehaviour
{
    public TextAsset jsonFile;

    [System.Serializable]
    public class FrameData
    {
        public List<int> startFrames;
        public List<int> endFrames;
    }

    [System.Serializable]
    public class HitboxData
    {
        public int xOffset;
        public int yOffset;
        public int width;
        public int height;
        public int xKnockback;
        public int yKnockback;
        public int hitstun;
    }

    [System.Serializable]
    public class HurtboxData
    {
        public int xOffset;
        public int yOffset;
        public int width;
        public int height;
    }

    [System.Serializable]
    public class FrameDataContainer
    {
        public FrameData sideAttackFrames;
        public FrameData upAttackFrames;
        public FrameData downAttackFrames;
    }

    [System.Serializable]
    public class HitboxDataContainer
    {
        public List<HitboxData> sideAttackHitboxes;
        public List<HitboxData> upAttackHitboxes;
        public List<HitboxData> downAttackHitboxes;
    }

    [System.Serializable]
    public class HurtboxDataContainer
    {
        public HurtboxData idleHurtbox;
        public HurtboxData runHurtbox;
        public HurtboxData jumpsquatHurtbox;
        public HurtboxData jumpHurtbox;
        public HurtboxData landingHurtbox;
        public HurtboxData hitstunHurtbox;
        public HurtboxData shieldHurtbox;
        public HurtboxData sideAttackHurtbox;
        public HurtboxData upAttackHurtbox;
        public HurtboxData downAttackHurtbox;
        public HurtboxData menuingHurtbox;
    }

    [System.Serializable]
    public class WeaponData
    {
        public string weapon;
        public int runSpeed;
        public int jumpForce;
        public int maxHitboxes;
        public FrameDataContainer frameData;
        public HitboxDataContainer hitboxData;
        public HurtboxDataContainer hurtboxData;
    }

    [System.Serializable]
    public class WeaponDataList
    {
        public List<WeaponData> weaponData;
    }

    public WeaponDataList weaponDataList = new WeaponDataList();

    private void Awake()
    {
        GetWeaponStats();
    }

    public void GetWeaponStats()
    {
        if (jsonFile != null)
        {
            //Debug.Log("JSON file found.");
            weaponDataList = JsonUtility.FromJson<WeaponDataList>(jsonFile.text);
            //Debug.Log("JSON data loaded.");
            //Debug.Log("Number of weapons: " + weaponDataList.weaponData.Count);
        }
        else
        {
            Debug.LogError("JSON file not assigned.");
        }
        return;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
