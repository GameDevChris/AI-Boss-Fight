using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BeatPlatform : MonoBehaviour
{
    public enum Effect
    {
        None,
        Grow,
        Hide,
        Spike
    }

    //Public variables
    [HideInInspector]
    public bool IsBeat = false;
    [HideInInspector]
    public Effect CurrentEffect = Effect.None;
    
    [Header("Hand slap target")]
        public Transform HandTargetTransform;
        public ParticleSystem SlapParticles;
        
    //Serialized private variables to be viewed in inspector
    [Header("Effect Icons")]
        [SerializeField]
        private Image _effectIcon;
        [SerializeField]
        private Sprite _emptyIconSprite, _hiddenIconSprite, _grownIconSprite, _spikedIconSprite;
        
    [Header("Countdown UI")]
        [SerializeField]
        private Canvas _countdownCanvas;
        [SerializeField]
        private TextMeshProUGUI _countdownText;
    
    [Header("Effect elements")]
        [SerializeField] 
        private List<BoxCollider> _collidersToHide;
        [SerializeField]
        private List<MeshRenderer> _renderersToHide;
        [SerializeField]
        private List<Transform> _transformsToGrow;
        [SerializeField]
        private GameObject _spikeParticles, _spikeTrigger;
        
    [Header("Beat materials")]
        [SerializeField]
        private Material _isBeatMaterial;
        [SerializeField]
        private Material _defaultMaterial;
        [SerializeField]
        private List<Renderer> _matsToSet;
        
    //Private only variables
    [SerializeField]
    private Vector3 _originalSize;

    void Start()
    {
        _originalSize = transform.localScale;
    }

    void Update()
    {
        //Make effect canvas face camera
        Quaternion cameraRotation = Camera.main.transform.rotation;
        cameraRotation = Quaternion.Euler(0, cameraRotation.eulerAngles.y, 0);
        _countdownCanvas.transform.rotation = cameraRotation;

        //Set materials if beat
        if (IsBeat)
        {
            foreach (var mat in _matsToSet)
            {
                mat.material = _isBeatMaterial;
            }
        }
        
        else
        {
            foreach (var mat in _matsToSet)
            {
                mat.material = _defaultMaterial;
            }
        }

        //Do effects
        switch (CurrentEffect)
        {
            case Effect.None:
                foreach (var tran in _transformsToGrow)
                {
                    tran.localScale = _originalSize;
                }
                
                foreach (var col in _collidersToHide)
                {
                    col.enabled = true;
                }

                foreach (var rend in _renderersToHide)
                {
                    rend.enabled = true;
                }

                _effectIcon.sprite = _emptyIconSprite;
                _effectIcon.enabled = false;
                
                _spikeParticles.SetActive(false);
                _spikeTrigger.SetActive(false);
                break;
            
            case Effect.Grow:
                foreach (var tran in _transformsToGrow)
                {
                    tran.localScale = _originalSize*2;
                }
                
                foreach (var col in _collidersToHide)
                {
                    col.enabled = true;
                }

                foreach (var rend in _renderersToHide)
                {
                    rend.enabled = true;
                }
                
                _effectIcon.sprite = _grownIconSprite;
                _effectIcon.enabled = true;
                
                _spikeParticles.SetActive(false);
                _spikeTrigger.SetActive(false);
                break;
            
            case Effect.Hide:
                foreach (var tran in _transformsToGrow)
                {
                    tran.localScale = _originalSize;
                }
                
                foreach (var col in _collidersToHide)
                {
                    col.enabled = false;
                }

                foreach (var rend in _renderersToHide)
                {
                    rend.enabled = false;
                }
                
                _effectIcon.sprite = _hiddenIconSprite;
                _effectIcon.enabled = true;
                
                _spikeParticles.SetActive(false);
                _spikeTrigger.SetActive(false);
                break;
            
            case Effect.Spike:
                foreach (var tran in _transformsToGrow)
                {
                    tran.localScale = _originalSize;
                }
                
                foreach (var col in _collidersToHide)
                {
                    col.enabled = true;
                }

                foreach (var rend in _renderersToHide)
                {
                    rend.enabled = true;
                }
                
                _effectIcon.sprite = _spikedIconSprite;
                _effectIcon.enabled = true;
                
                _spikeParticles.SetActive(true);
                _spikeTrigger.SetActive(true);
                break;
        }
    }

    //Effect timer
    public IEnumerator Countdown(int time)
    {
        if (time > 0)
        {
            _countdownText.text = time.ToString();
            yield return new WaitForSeconds(1);
            time--;
            StartCoroutine(Countdown(time));
        }

        else
        {
            _countdownText.text = "";
            CurrentEffect = Effect.None;
        }
    }
}
