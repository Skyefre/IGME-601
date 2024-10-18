using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJSONReader : MonoBehaviour
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
    public class EnemyData
    {
        public string enemy;
        public int runSpeed;
        public int jumpForce;
        public int maxHitboxes;
        public FrameDataContainer frameData;
        public HitboxDataContainer hitboxData;
        public HurtboxDataContainer hurtboxData;
    }

    [System.Serializable]
    public class EnemyDataList
    {
        public List<EnemyData> enemyData;
    }

    public EnemyDataList enemyDataList = new EnemyDataList();

    private void Awake()
    {
        GetEnemyStats();
    }

    public void GetEnemyStats()
    {
        if (jsonFile != null)
        {
            enemyDataList = JsonUtility.FromJson<EnemyDataList>(jsonFile.text);
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
