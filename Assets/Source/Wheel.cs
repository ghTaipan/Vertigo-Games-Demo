using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wheel : MonoBehaviour
{
    [SerializeField] private Button _buttonSpin;
    [SerializeField] private Image _wheelImage;
    [SerializeField] private bool _bSpinRight = false;
    [SerializeField] private float _spinMaxSpeed = 500.0f;
    [SerializeField] private float _spinDecayAmount = 0.75f;
    [SerializeField] private float _spinMinTimeToStartDecay = 2.0f;
    [SerializeField] private float _spinMaxTimeToStartDecay = 3.5f;
    private float _spinDecayStartTime = 0.0f;
    private float _spinSpeed = 0.0f;
    private float _spinDirectionMult = 1.0f;

    void Start()
    {
        _buttonSpin.onClick.AddListener(OnSpinButtonPressed);
        _spinDirectionMult = _bSpinRight ? -1.0f : 1.0f;
    }

    void OnDestroy()
    {
        _buttonSpin.onClick.RemoveListener(OnSpinButtonPressed);
    }

    void OnSpinButtonPressed()
    {
        _buttonSpin.interactable = false;
        StartCoroutine(Spin());
    }

    IEnumerator Spin()
    {
        _spinDecayStartTime = Random.Range(_spinMinTimeToStartDecay, _spinMaxTimeToStartDecay);
        _spinSpeed = _spinMaxSpeed;
        float spinnedTime = 0.0f;

        while (Mathf.Abs(_spinSpeed) > Mathf.Abs(_spinDecayAmount))
        {
            
            if (spinnedTime < _spinDecayStartTime)
            {
                spinnedTime += Time.deltaTime;
            }
            else
            {
                _spinSpeed = Mathf.Abs(_spinSpeed) - Mathf.Abs(_spinDecayAmount);  
            }

            _wheelImage.rectTransform.Rotate(0, 0, _spinDirectionMult * Mathf.Abs(_spinSpeed) * Time.deltaTime);

            yield return null;
        }
    }
}
