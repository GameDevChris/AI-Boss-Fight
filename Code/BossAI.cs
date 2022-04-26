using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    public enum Phase
    {
        BossIntroPhase,
        BossSlapPhase
    };
    
    //Public variables
    [Header("Health settings")]
    public float MaxHealth = 100;
    public float CurrentHealth = 100;
    
    //Serialized private variables to be viewed in inspector
    [Space]
    [SerializeField]
    private Transform _rotationPivot;

    [Header("Healthbar settings")]
        [SerializeField]
        private Slider _healthBar;
        [SerializeField]
        private Image _healthFill;
        [SerializeField]
        private Gradient _healthColour;
        
    [Header("Invincibility settings")]
        [SerializeField]
        private Color _invincibleColor;
        [SerializeField]
        private TextMeshProUGUI _invincibilityTimer;
        [SerializeField]
        private Image _invincibilityIcon;
        [SerializeField]
        private int _invincibilityDuration = 10;
        
    [Header("IK settings")]
    

    //Private only variables
    private GameObject _player;
    private bool _canTakeDamage = true;
    private EventGenerator _eventGenerator;
    
    
    //IK
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public TwoBoneIKConstraint rightIK;
    public TwoBoneIKConstraint leftIK;
    public Quaternion rightHandTargetRotation;
    public AnimationCurve handPosCurve;

    public Vector3 introStartPos;
    public Vector3 introEndPos;
    public AnimationCurve introCurve;
    public AnimationCurve introIKCurve;

    //Platforms
    public Vector3 leftStartingPos;
    public Vector3 rightStartingPos;
    public Quaternion leftStartingRot;
    public Quaternion rightStartingRot;
    public PlatformZone leftPlatforms;
    public PlatformZone rightPlatforms;
    public BeatPlatform leftCurrentPlatform;
    public BeatPlatform rightCurrentPlatform;

    //Phase values
    public Phase currentPhase;
    
    public float introStartTime = 0;
    public float introJourneyTime = 0;
    public bool isIntro = false;
    
    public float slapStartTime = 0;
    public float slapJourneyTime = 0;
    public bool isSlapping = false;
    
    public float hoverStartTime = 0;
    public float hoverJourneyTime = 0;
    public bool isHovering = false;
    
    //Boss Aesthetics
    public MeshRenderer mainRend;
    public Material damageTakeMat;
    public Material normalMat;

    
    
    void Start()
    {
        CurrentHealth = MaxHealth;
        _healthBar.maxValue = MaxHealth;
        _player = GameObject.FindGameObjectWithTag("Player");
        _eventGenerator = FindObjectOfType<EventGenerator>();
    }

    void Update()
    {
        if (CurrentHealth <= 0 || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }

        _healthBar.value = CurrentHealth;

        if (_canTakeDamage)
        {
            _invincibilityIcon.enabled = false;
            _healthFill.color = _healthColour.Evaluate(_healthBar.normalizedValue);
        }
        else
        {
            _invincibilityIcon.enabled = true;
            _healthFill.color = _invincibleColor;
        }

        if (currentPhase == Phase.BossIntroPhase)
        {
            if (isIntro)
            {
                rightIK.weight = 0;
                leftIK.weight = 0;
                
                float introEvaluated = introCurve.Evaluate((Time.time - introStartTime) / introJourneyTime);
                float ikEvaluated = introIKCurve.Evaluate((Time.time - introStartTime) / introJourneyTime);

                leftIK.weight = Mathf.Lerp(0, 1, ikEvaluated);
                rightIK.weight = Mathf.Lerp(0,1,ikEvaluated);
                
                _rotationPivot.position = Vector3.Lerp(introStartPos, introEndPos, introEvaluated);
            }
            
        }
        
        if (currentPhase == Phase.BossSlapPhase)
        {
            if (isSlapping)
            {
                float evaluated = handPosCurve.Evaluate((Time.time - slapStartTime) / slapJourneyTime);
                
                leftIK.weight = Mathf.Lerp(0,1, evaluated);
                rightIK.weight = Mathf.Lerp(0,1, evaluated);

                rightHandTargetRotation = rightCurrentPlatform.HandTargetTransform.rotation; 
                rightHandTargetRotation *= Quaternion.Euler(180,0,180);
                rightHandTarget.transform.rotation = Quaternion.Slerp(rightStartingRot, rightHandTargetRotation, evaluated);
                leftHandTarget.transform.rotation = Quaternion.Slerp(leftStartingRot, leftCurrentPlatform.HandTargetTransform.rotation, evaluated);
                
                leftHandTarget.transform.position = Vector3.Lerp(leftStartingPos, leftCurrentPlatform.transform.position, evaluated);
                rightHandTarget.transform.position = Vector3.Lerp(rightStartingPos, rightCurrentPlatform.transform.position, evaluated);
                

                if (_player.GetComponent<ThirdPersonController>().Grounded)
                {
                    Quaternion lookRot = Quaternion.LookRotation(_player.transform.position - _rotationPivot.transform.position);
                    _rotationPivot.transform.rotation = Quaternion.Slerp(_rotationPivot.transform.rotation, lookRot, Time.deltaTime);
                }
                
            }
            
            if (isHovering)
            {
                rightIK.weight = 0;
                leftIK.weight = 0;
                
                leftIK.weight = Mathf.Lerp(1,0, (Time.time - hoverStartTime) / hoverJourneyTime);
                rightIK.weight = Mathf.Lerp(1,0, (Time.time - hoverStartTime) / hoverJourneyTime);
            }
        }
    }

    public void Hover(float hoverTime)
    {
        currentPhase = Phase.BossSlapPhase;
        
        leftCurrentPlatform.IsBeat = false;
        rightCurrentPlatform.IsBeat = false;

        leftCurrentPlatform = leftPlatforms.myPlatforms[Random.Range(0, leftPlatforms.myPlatforms.Count)];
        rightCurrentPlatform = rightPlatforms.myPlatforms[Random.Range(0, rightPlatforms.myPlatforms.Count)];
            
        leftCurrentPlatform.IsBeat = true;
        rightCurrentPlatform.IsBeat = true;

        isSlapping = false;
        
        leftStartingPos = leftHandTarget.transform.position;
        rightStartingPos = rightHandTarget.transform.position;
        
        leftStartingRot = leftHandTarget.transform.rotation;
        rightStartingRot = rightHandTarget.transform.rotation;

        hoverStartTime = Time.time;
        hoverJourneyTime = hoverTime;
        isHovering = true;
        
        
    }

    public void StartIntro(float introTime)
    {
        currentPhase = Phase.BossIntroPhase;
        introStartTime = Time.time;
        introJourneyTime = introTime;
        isIntro = true;
    }
    
    public void Slap(float slapTime)
    {
        currentPhase = Phase.BossSlapPhase;
        
        isHovering = false;
        
        leftStartingPos = leftHandTarget.transform.position;
        rightStartingPos = rightHandTarget.transform.position;
        
        leftStartingRot = leftHandTarget.transform.rotation;
        rightStartingRot = rightHandTarget.transform.rotation;
        
        slapStartTime = Time.time;
        slapJourneyTime = slapTime;
        isSlapping = true;
    }
    
    public void Event()
    {
        rightCurrentPlatform.SlapParticles.Play();
        _eventGenerator.EventDo(rightCurrentPlatform);

        leftCurrentPlatform.SlapParticles.Play();
        _eventGenerator.EventDo(leftCurrentPlatform);
    }

    public void TakeDamage(float damageTaken)
    {
        if (_canTakeDamage)
        {
            CurrentHealth -= damageTaken;
            _canTakeDamage = false;
            StartCoroutine(DamageFlash());
            StartCoroutine(TakeDamageCooldown(_invincibilityDuration));
        }
        
    }
    
    public IEnumerator TakeDamageCooldown(int time)
    {
        if (time > 0)
        {
            _invincibilityTimer.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
            StartCoroutine(TakeDamageCooldown(time));
        }

        else
        {
            _invincibilityTimer.text = "";
            _canTakeDamage = true;
        }
    }
    public IEnumerator DamageFlash()
    {
        mainRend.material = damageTakeMat;
        yield return new WaitForSeconds(0.1f);
        mainRend.material = normalMat;
        yield return new WaitForSeconds(0.1f);
        mainRend.material = damageTakeMat;
        yield return new WaitForSeconds(0.1f);
        mainRend.material = normalMat;
    }
}
