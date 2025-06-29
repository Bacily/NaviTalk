using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DropdownSearch : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField searchInputField;
    [SerializeField]
    private TMP_Dropdown dropdown;

    private List<TMP_Dropdown.OptionData> originalOptions;
    private bool isInitialized = false;

    [SerializeField]
    private TargetHandler targetManager;

    private List<TargetFacade> currentTargetItemsOrigin;


    void Update()
    {
        // Keep checking if we should initialize
        // We initialize if we haven't already AND the dropdown now has options
        if (!isInitialized && dropdown.options.Count > 0)
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        originalOptions = new List<TMP_Dropdown.OptionData>(dropdown.options);
        currentTargetItemsOrigin = new List<TargetFacade>(targetManager.currentTargetItems);
        searchInputField.onValueChanged.AddListener(FilterDropdown);
        isInitialized = true;
        Debug.Log("Dropdown Search Initialized with " + originalOptions.Count + " options.");
    }

    // FilterDropdown function remains the same...
    private void FilterDropdown(string searchQuery)
    {
        if (!isInitialized) return;

        dropdown.ClearOptions();
        targetManager.currentTargetItems.Clear();

        if (string.IsNullOrEmpty(searchQuery))
        {
            targetManager.currentTargetItems.AddRange(currentTargetItemsOrigin);
            dropdown.AddOptions(originalOptions);
            dropdown.RefreshShownValue();
            dropdown.value = 0;
            return;
        }

        List<TMP_Dropdown.OptionData> filteredOptions = originalOptions
            .Where(option => option.text.ToLower().Contains(searchQuery.ToLower()))
            .ToList();

        List<TargetFacade> filteredTargets = currentTargetItemsOrigin
            .Where(target => target.name.ToLower().Contains(searchQuery.ToLower()))
            .ToList();

        if (filteredOptions.Count == 0)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData("No results found"));
        }
        else
        {
            targetManager.currentTargetItems.AddRange(filteredTargets);
            dropdown.options.Add(new TMP_Dropdown.OptionData("Choose room..."));
            dropdown.AddOptions(filteredOptions);
        }

        dropdown.RefreshShownValue();


    }
}