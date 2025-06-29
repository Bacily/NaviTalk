using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetHandler : MonoBehaviour {

    [SerializeField]
    private NavigationController navigationController;
    [SerializeField]
    private TextAsset targetModelData;
    [SerializeField]
    private TMP_Dropdown targetDataDropdown;

    [SerializeField]
    private GameObject targetObjectPrefab;

    [SerializeField]
    private GameObject floorIndicator;
    ToggleFloor floorIndicatorscript;

    [SerializeField]
    private Transform[] targetObjectsParentTransforms;

    public List<TargetFacade> currentTargetItems = new List<TargetFacade>();

    private void Start() {
        GenerateTargetItems();
        FillDropdownWithTargetItems();
        floorIndicatorscript = floorIndicator.GetComponent<ToggleFloor>();
    }

    private void GenerateTargetItems() {
        IEnumerable<Target> targets = GenerateTargetDataFromSource();
        foreach (Target target in targets) {
            currentTargetItems.Add(CreateTargetFacade(target));
        }
    }

    private IEnumerable<Target> GenerateTargetDataFromSource() {
        return JsonUtility.FromJson<TargetWrapper>(targetModelData.text).TargetList;
    }

    private TargetFacade CreateTargetFacade(Target target) {
        GameObject targetObject = Instantiate(targetObjectPrefab, targetObjectsParentTransforms[target.FloorNumber], false);
        targetObject.SetActive(true);
        target.Name = $"Floor {target.FloorNumber} - {target.Name}";
        targetObject.name = target.Name;
        targetObject.transform.localPosition = target.Position;
        targetObject.transform.localRotation = Quaternion.Euler(target.Rotation);
        TargetFacade targetData = targetObject.GetComponent<TargetFacade>();
        targetData.Name = target.Name;
        targetData.FloorNumber = target.FloorNumber;

        return targetData;
    }

    private void FillDropdownWithTargetItems() {
        List<TMP_Dropdown.OptionData> targetFacadeOptionData =
            currentTargetItems.Select(x => new TMP_Dropdown.OptionData {
                text = x.Name
            }).ToList();

        targetDataDropdown.ClearOptions();
        targetDataDropdown.options.Add(new TMP_Dropdown.OptionData("Choose room..."));
        targetDataDropdown.AddOptions(targetFacadeOptionData);
    }

    public void SetSelectedTargetPositionWithDropdown(int selectedValue) {
        Console.WriteLine("targetFloor.ToString()");
        navigationController.TargetPosition = GetCurrentlySelectedTarget(selectedValue);
        //floorIndicatorscript.textUPstair.text = currentTargetItems[selectedValue].FloorNumber.ToString();
        for(int i=0;i<currentTargetItems.Count;i++) currentTargetItems[i].gameObject.SetActive(false);
        currentTargetItems[selectedValue].gameObject.SetActive(true);
        floorIndicatorscript.writeTragetFloor(currentTargetItems[selectedValue].FloorNumber);
    }

    private Vector3 GetCurrentlySelectedTarget(int selectedValue) {
        if (selectedValue >= currentTargetItems.Count) {
            return Vector3.zero;
        }

        return currentTargetItems[selectedValue].transform.position;
    }

    public TargetFacade GetCurrentTargetByTargetText(string targetText) {
        return currentTargetItems.Find(x =>
            x.Name.ToLower().Equals(targetText.ToLower()));
    }
}
