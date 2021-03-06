﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate void TreatmentSlotEvent(TreatmentHeroSlot slot);

public class TreatmentHeroSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerClickHandler
{
    public string activityName;

    public Text heroLabel;

    public Image lockerIcon;
    public Image activeIcon;
    public Image buttonIcon;
    public Image slotIcon;
    public Image freeIcon;

    public TreatmentSlot TreatmentSlot { get; set; }
    public RectTransform RectTransform { get; set; }

    public event TreatmentSlotEvent onHeroDropped;
    public event TreatmentSlotEvent onHeroRemoved;
    public event TreatmentSlotEvent onTreatmentButtonClick;

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void Initialize(TreatmentSlot slot)
    {
        TreatmentSlot = slot;
        UpdateSlot();
    }
    public void SetStatus(ActivitySlotStatus status)
    {
        if (TreatmentSlot == null)
            return;

        TreatmentSlot.Status = status;
        switch (TreatmentSlot.Status)
        {
            case ActivitySlotStatus.Available:
                TreatmentSlot.Hero = null;
                break;
            case ActivitySlotStatus.Blocked:
            case ActivitySlotStatus.Caretaken:
            case ActivitySlotStatus.Crierd:
            case ActivitySlotStatus.Checkout:
            case ActivitySlotStatus.Paid:
            default:
                break;
        }
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        if (!TreatmentSlot.IsUnlocked)
            SetActivitySlotClosed();
        else
        {
            switch (TreatmentSlot.Status)
            {
                case ActivitySlotStatus.Available:
                    SetActivitySlotOpened();
                    break;
                case ActivitySlotStatus.Checkout:
                    SetActivitySlotOccupiedCheckout();
                    break;
                case ActivitySlotStatus.Paid:
                    SetActivitySlotOccupiedPaid();
                    break;
                case ActivitySlotStatus.Blocked:
                case ActivitySlotStatus.Caretaken:
                case ActivitySlotStatus.Crierd:
                default:
                    SetActivitySlotClosed();
                    break;
            }
        }
    }
    public void PayoutSlot()
    {
        if (onHeroRemoved != null)
            onHeroRemoved(this);

        TreatmentSlot.Status = ActivitySlotStatus.Paid;

        UpdateSlot();
    }
    public void ClearSlot()
    {
        if (onHeroRemoved != null)
            onHeroRemoved(this);

        TreatmentSlot.Hero = null;
        TreatmentSlot.Status = ActivitySlotStatus.Available;
        
        UpdateSlot();
    }
    public void ResetTreatment()
    {
        if(TreatmentSlot != null)
        {
            TreatmentSlot.TargetDiseaseQuirk = null;
            TreatmentSlot.TargetNegativeQuirk = null;
            TreatmentSlot.TargetPositiveQuirk = null;
        }
    }

    public void UpdateAvailable()
    {
        if (TreatmentSlot.IsUnlocked)
            if (TreatmentSlot.Status == ActivitySlotStatus.Available)
                SetActivitySlotOpened();
    }
    public void UpdateHeroAvailable(Hero hero)
    {
        if (TreatmentSlot.IsUnlocked)
        {
            if (TreatmentSlot.Status == ActivitySlotStatus.Available)
                SetActivitySlotHighlighted();
            else if (TreatmentSlot.Status == ActivitySlotStatus.Checkout)
            {
                TreatmentSlot.Hero = null;
                TreatmentSlot.Status = ActivitySlotStatus.Available;
                SetActivitySlotHighlighted();
            }
        }
    }

    public void TreatmentButtonClicked()
    {
        if (onTreatmentButtonClick != null)
            onTreatmentButtonClick(this);
    }

    void SetActivitySlotOccupiedPaid()
    {
        heroLabel.text = TreatmentSlot.Hero.Name;
        buttonIcon.gameObject.SetActive(true);
        lockerIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(true);
        freeIcon.gameObject.SetActive(false);

        if (TreatmentSlot.Hero != null)
            slotIcon.sprite = DarkestDungeonManager.HeroSprites[TreatmentSlot.Hero.ClassStringId]["A"].Portrait;
    }
    void SetActivitySlotOccupiedCheckout()
    {
        heroLabel.text = TreatmentSlot.Hero.Name;

        buttonIcon.gameObject.SetActive(false);
        lockerIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(false);
        freeIcon.gameObject.SetActive(false);

        if (TreatmentSlot.Hero != null)
            slotIcon.sprite = DarkestDungeonManager.HeroSprites[TreatmentSlot.Hero.ClassStringId]["A"].Portrait;
    }
    void SetActivitySlotClosed()
    {
        heroLabel.text = "";

        buttonIcon.gameObject.SetActive(false);
        lockerIcon.gameObject.SetActive(true);
        activeIcon.gameObject.SetActive(false);
        freeIcon.gameObject.SetActive(false);
    }
    void SetActivitySlotOpened()
    {
        heroLabel.text = "";

        buttonIcon.gameObject.SetActive(false);
        lockerIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(false);
        if (DarkestDungeonManager.Campaign.EventModifiers.FreeActivities.ContainsKey(activityName) &&
            DarkestDungeonManager.Campaign.EventModifiers.FreeActivities[activityName] == true)
            freeIcon.gameObject.SetActive(true);
        else
            freeIcon.gameObject.SetActive(false);
        slotIcon.sprite = DarkestDungeonManager.Data.Sprites["hero_slot.background"];
    }
    void SetActivitySlotHighlighted()
    {
        heroLabel.text = "";

        buttonIcon.gameObject.SetActive(false);
        lockerIcon.gameObject.SetActive(false);
        activeIcon.gameObject.SetActive(false);
        if (DarkestDungeonManager.Campaign.EventModifiers.FreeActivities.ContainsKey(activityName) &&
            DarkestDungeonManager.Campaign.EventModifiers.FreeActivities[activityName] == true)
            freeIcon.gameObject.SetActive(true);
        else
            freeIcon.gameObject.SetActive(false);
        slotIcon.sprite = DarkestDungeonManager.Data.Sprites["hero_slot.backgroundhightlight"];
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (TreatmentSlot.Status == ActivitySlotStatus.Caretaken)
            ToolTipManager.Instanse.Show(LocalizationManager.GetString("str_cant_place_hero_here"),
                eventData, RectTransform, ToolTipStyle.FromBottom, ToolTipSize.Small);

        if (TreatmentSlot.IsUnlocked)
            DarkestSoundManager.PlayOneShot("event:/ui/town/button_mouse_over_3");
        else
            DarkestSoundManager.PlayOneShot("event:/ui/town/button_mouse_over_2");
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManager.Instanse.Hide();
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.tag == "Roster Hero Slot")
        {
            if (TreatmentSlot == null || !TreatmentSlot.IsUnlocked)
            {
                DarkestSoundManager.PlayOneShot("event:/ui/town/button_invalid");
                return;
            }

            if (TreatmentSlot.Status == ActivitySlotStatus.Available)
            {
                var heroSlot = eventData.pointerDrag.GetComponent<HeroSlot>();
                TreatmentSlot.Status = ActivitySlotStatus.Checkout;
                TreatmentSlot.Hero = heroSlot.Hero;
                if (onHeroDropped != null)
                    onHeroDropped(this);
                UpdateSlot();
                DarkestSoundManager.PlayOneShot("event:/ui/town/character_add");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.dragging)
            return;

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (TreatmentSlot != null && TreatmentSlot.Status == ActivitySlotStatus.Checkout)
            ClearSlot();
    }
}
