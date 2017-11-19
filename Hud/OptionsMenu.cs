using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private int totalVol;
    private int musicVol;
    private int soundEffectsVol;

    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown ratioDropdown;
    [SerializeField] private Button backButton;
    [SerializeField] private Button applyButton;
    [SerializeField] private GameObject fatherObject;
    private List<string> fourByThree;
    private List<string> sixteenByNine;

    private Resolution selectedResolution;
    private Resolution currentResolution;


    private void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        applyButton.onClick.AddListener(OnApplyButtonClick);
        ratioDropdown.options =
            new List<Dropdown.OptionData> {new Dropdown.OptionData("4:3"), new Dropdown.OptionData("16:9")};
        ratioDropdown.onValueChanged.AddListener(OnRatioDropdownChange);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChange);
        fourByThree = new List<string>();
        sixteenByNine = new List<string>();

        InitializeResolutionsLists();
        currentResolution = Screen.currentResolution;

        selectedResolution = new Resolution
        {
            width = currentResolution.width,
            height = currentResolution.height,
            refreshRate = 60
        };
        ratioDropdown.value =
            MathHelpers.Approximately(GetResolutionRatio(currentResolution.width, currentResolution.height),
                4.00 / 3.00, float.Epsilon)
                ? (int) Ratio.FourByThree
                : (int) Ratio.SixteenByNine;

        OnRatioDropdownChange(MathHelpers.Approximately(
            GetResolutionRatio(currentResolution.width, currentResolution.height),
            4.00 / 3.00, float.Epsilon)
            ? (int) Ratio.FourByThree
            : (int) Ratio.SixteenByNine);
    }

    private void OnEnable()
    {
        applyButton.gameObject.SetActive(false);
        if (!selectedResolution.Equals(currentResolution))
        {
            ResetDropdownValues();
        }
    }


    private void Update()
    {
        applyButton.gameObject.SetActive(!Equals(selectedResolution, currentResolution));
    }

    private double GetResolutionRatio(double width, double height)
    {
        return width / height;
    }

    private string GetResolutionRatioInText(double width, double height)
    {
        return MathHelpers.Approximately(width / height, (double) 4 / 3, float.Epsilon) ? "4:3" : "16:9";
    }


    private Resolution GetResolutionFromText(string resolutionText, Resolution resolution)
    {
        var resolutionTextHolder = resolutionText.Replace(" ", "");
        var resolutionWidth =
            resolutionTextHolder.Substring(0, resolutionTextHolder.IndexOf("x", StringComparison.Ordinal));
        var resolutionHeigth =
            resolutionTextHolder.Substring(resolutionTextHolder.IndexOf("x", StringComparison.Ordinal) + 1);
        resolution.width = int.Parse(resolutionWidth);
        resolution.height = int.Parse(resolutionHeigth);
        return resolution;
    }

    private string GetTextFromResolution(Resolution resolution)
    {
        return resolution.width + " x " + resolution.height;
    }

    private void InitializeResolutionsLists()
    {
        for (var i = 0; i < Screen.resolutions.Length; i++)
        {
            var resolution = Screen.resolutions[i];
            double width = resolution.width;
            double height = resolution.height;

            if (MathHelpers.Approximately(width / height, 4.00 / 3.00, 0.001) &&
                resolution.height >= 480)
            {
                var resolutionToText = width + " x " + height;
                if (!fourByThree.Exists(lambdaExpression => lambdaExpression == resolutionToText))
                {
                    fourByThree.Add(resolutionToText);
                }
            }
            else if (MathHelpers.Approximately(width / height, 16.00 / 9.00, 0.001))
            {
                var resolutionToText = width + " x " + height;
                if (!sixteenByNine.Exists(lambdaExpression => lambdaExpression == resolutionToText))
                {
                    sixteenByNine.Add(resolutionToText);
                }
            }
        }
    }

    private void OnRatioDropdownChange(int selectedOption)
    {
        resolutionDropdown.options = new List<Dropdown.OptionData>();

        switch (selectedOption)
        {
            case (int) Ratio.FourByThree:
                resolutionDropdown.AddOptions(fourByThree);

                resolutionDropdown.value = MathHelpers.Approximately(
                    GetResolutionRatio(currentResolution.width, currentResolution.height),
                    4.00 / 3.00, 0.001)
                    ? fourByThree.FindIndex(lambdaExpression =>
                        lambdaExpression == GetTextFromResolution(currentResolution))
                    : 0;

                OnResolutionDropdownChange(MathHelpers.Approximately(
                    GetResolutionRatio(currentResolution.width, currentResolution.height),
                    4.00 / 3.00, 0.001)
                    ? fourByThree.FindIndex(lambdaExpression =>
                        lambdaExpression == GetTextFromResolution(currentResolution))
                    : 0);
                break;
            case (int) Ratio.SixteenByNine:
                resolutionDropdown.AddOptions(sixteenByNine);

                resolutionDropdown.value = MathHelpers.Approximately(
                    GetResolutionRatio(currentResolution.width, currentResolution.height),
                    16.00 / 9.00, 0.001)
                    ? sixteenByNine.FindIndex(lambdaExpression =>
                        lambdaExpression == GetTextFromResolution(currentResolution))
                    : 0;
                OnResolutionDropdownChange(MathHelpers.Approximately(
                    GetResolutionRatio(currentResolution.width, currentResolution.height),
                    16.00 / 9.00, 0.001)
                    ? sixteenByNine.FindIndex(lambdaExpression =>
                        lambdaExpression == GetTextFromResolution(currentResolution))
                    : 0);
                break;
            default:
                Debug.Log("ERROR");
                break;
        }
    }

    private void OnResolutionDropdownChange(int selectedOption)
    {
        if (selectedOption < 0) selectedOption = 0;
        if (ratioDropdown.value == (int) Ratio.FourByThree)
        {
            selectedResolution = GetResolutionFromText(fourByThree[selectedOption], selectedResolution);
        }
        else if (ratioDropdown.value == (int) Ratio.SixteenByNine)
        {
            selectedResolution = GetResolutionFromText(sixteenByNine[selectedOption], selectedResolution);
        }
    }

    private void ResetDropdownValues()
    {
        resolutionDropdown.value = -1;
        ratioDropdown.value = -1;
        ratioDropdown.value =
            MathHelpers.Approximately(GetResolutionRatio(currentResolution.width, currentResolution.height),
                (double) 4 / 3, float.Epsilon)
                ? (int) Ratio.FourByThree
                : (int) Ratio.SixteenByNine;
        resolutionDropdown.value =
            resolutionDropdown.options.FindIndex(lambdaExpression =>
                lambdaExpression.text == GetTextFromResolution(currentResolution));
    }

    private void OnBackButtonClick()
    {
        fatherObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnApplyButtonClick()
    {
        CoroutineManager.AddCoroutine(ChangeResolutionCoroutine(), "ChangeResolutionCoroutine");
    }

    private IEnumerator ChangeResolutionCoroutine()
    {
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, true, selectedResolution.refreshRate);
        yield return new WaitForSeconds(0.2f);
        currentResolution = Screen.currentResolution;
        ResetDropdownValues();
        CoroutineManager.DeleteCoroutine("ChangeResolutionCoroutine");
    }
}