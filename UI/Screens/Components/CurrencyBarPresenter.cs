using UnityEngine;
using System;
using VContainer;

public class CurrencyBarPresenter : MonoBehaviour
{
    [SerializeField] private CurrencyBarView _view;
    private CurrencyState _currencyState;

    public void Initialize(CurrencyState currencyState)
    {
        _currencyState = currencyState;
        _view.SetSoftCurrencyAmount(_currencyState.SoftCurrency);
        _view.SetHardCurrencyAmount(_currencyState.HardCurrency);
        _currencyState.OnSoftCurrencyChanged += OnSoftCurrencyChanged;
        _currencyState.OnHardCurrencyChanged += OnHardCurrencyChanged;
    }

    public void Show(bool show)
    {
        _view.Show(show);
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        if (_currencyState != null)
        {
            _currencyState.OnSoftCurrencyChanged -= OnSoftCurrencyChanged;
            _currencyState.OnHardCurrencyChanged -= OnHardCurrencyChanged;
        }
    }

    private void OnSoftCurrencyChanged(int newAmount)
    {
        _view.SetSoftCurrencyAmount(newAmount);
    }

    private void OnHardCurrencyChanged(int newAmount)
    {
        _view.SetHardCurrencyAmount(newAmount);
    }
}