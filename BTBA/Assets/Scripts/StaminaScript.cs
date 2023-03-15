using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaScript : MonoBehaviour
{
    Slider _staminaSlider;
    float _currentStamina;
    float _currentMaxStamina;
    float _staminaRegenSpeed;
    bool _pauseStaminaRegen = false;

    private void Start()
    {
        _staminaSlider = GetComponent<Slider>();
    }

    public void SetAllValues(float stam, float maxStam, float stamRegen, bool pause)
    {
        _currentStamina = stam;
        _currentMaxStamina = maxStam;
        _staminaRegenSpeed = stamRegen;
        _pauseStaminaRegen = pause;
    }

    public float GetMaximumStamina()
    {
        return _currentMaxStamina;
    }

    public void SetMaxStamina(float maxStamina)
    {
        _staminaSlider.maxValue = maxStamina;
        _staminaSlider.value = maxStamina;
    }
    
    public float GetCurrentStamina()
    {
        return _currentStamina;
    }

    public void SetCurrentStamina(float value)
    {
        _currentStamina = value;
    }

    public bool GetStaminaPause()
    {
        return _pauseStaminaRegen;
    }
    
    public void SetStaminaPause(bool state)
    {
        _pauseStaminaRegen = state;
    }

    public void SetStaminaBar(float value)
    {
        _currentStamina = value;
        _staminaSlider.value = _currentStamina;
    }

    public void UseStamina(float drainAmount)
    {
        if (_currentStamina > 0)
        {
            _currentStamina -= drainAmount * Time.deltaTime;
        }
        SetStaminaBar(_currentStamina);
    }

    public void RegenerateStamina()
    {
        if (_currentStamina < _currentMaxStamina && _pauseStaminaRegen == false)
        {
            _currentStamina += _staminaRegenSpeed * Time.deltaTime;
        }
        SetStaminaBar(_currentStamina);
    }
}
