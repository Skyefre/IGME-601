using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJSONReader : MonoBehaviour
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
        public HurtboxData spellAttackHurtbox;
        public HurtboxData spellUtilHurtbox;
    }

    [System.Serializable]
    public class SpellFrameData
    {
        public List<int> spellAttackFrames;
        public List<int> spellUtilFrames;
    }

    [System.Serializable]
    public class SpellSpawnData
    {
        public int xOffset;
        public int yOffset;
    }

    [System.Serializable]
    public class SpellSpawnDataContainer
    {
        public List<SpellSpawnData> spellAttack;
        public List<SpellSpawnData> spellUtil;
    }

    [System.Serializable]
    public class ImpulseData
    {
        public int xImpulse;
        public int yImpulse;
    }

    [System.Serializable]
    public class ImpulseFrames
    {
        public List<int> sideAttackImpulseFrames;
        public List<int> upAttackImpulseFrames;
        public List<int> downAttackImpulseFrames;
        public List<int> spellAttackImpulseFrames;
        public List<int> spellUtilImpulseFrames;
    }

    [System.Serializable]
    public class ImpulseDataContainer
    {
        public List<ImpulseData> sideAttackImpulseData;
        public List<ImpulseData> upAttackImpulseData;
        public List<ImpulseData> downAttackImpulseData;
        public List<ImpulseData> spellAttackImpulseData;
        public List<ImpulseData> spellUtilImpulseData;
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
        public SpellFrameData spellframeData;
        public SpellSpawnDataContainer spellSpawnData;
        public ImpulseFrames impulseFrames;
        public ImpulseDataContainer impulseData;
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
            weaponDataList = JsonUtility.FromJson<WeaponDataList>(jsonFile.text);
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
