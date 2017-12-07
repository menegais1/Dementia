using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private struct StandardResolution
    {
        public double width;
        public double height;

        public StandardResolution(double width, double heigth)
        {
            this.width = width;
            this.height = heigth;
        }
    }

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
    private Resolution nativeResolution;

    private StandardResolution[] resolutions =
    {
        new StandardResolution(640, 480), new StandardResolution(800, 600), new StandardResolution(1024, 768),
        new StandardResolution(1152, 864), new StandardResolution(1280, 960), new StandardResolution(1400, 1050),
        new StandardResolution(1600, 1200), new StandardResolution(1280, 720), new StandardResolution(1366, 768),
        new StandardResolution(1600, 900), new StandardResolution(1920, 1080)
    };

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

        nativeResolution = GameManager.instance.NativeResolution;
        currentResolution = Screen.currentResolution;

        InitializeResolutionsLists();

        selectedResolution = new Resolution
        {
            width = currentResolution.width,
            height = currentResolution.height,
            refreshRate = currentResolution.refreshRate,
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
        if (currentResolution.height == 0 || selectedResolution.height == 0)
        {
            currentResolution = Screen.currentResolution;
            selectedResolution = new Resolution
            {
                width = currentResolution.width,
                height = currentResolution.height,
                refreshRate = currentResolution.refreshRate,
            };
        }

        applyButton.gameObject.SetActive(false);
        if (!selectedResolution.Equals(currentResolution))
        {
            ResetDropdownValues();
        }
    }


    private void Update()
    {
        if (!selectedResolution.Equals(currentResolution) && !applyButton.gameObject.activeSelf)
        {
            applyButton.gameObject.SetActive(true);
        }
        else if (applyButton.gameObject.activeSelf && selectedResolution.Equals(currentResolution))
        {
            applyButton.gameObject.SetActive(false);
        }
    }

    private double GetResolutionRatio(double width, double height)
    {
        return width / height;
    }

    private Resolution GetResolutionFromText(string resolutionText, Resolution resolution)
    {
        var resolutionTextHolder = resolutionText.Replace(" ", "");
        var resolutionWidth =
            resolutionTextHolder.Substring(0, resolutionTextHolder.IndexOf("x", StringComparison.Ordinal));
        var resolutionHeight =
            resolutionTextHolder.Substring(resolutionTextHolder.IndexOf("x", StringComparison.Ordinal) + 1);
        resolution.width = int.Parse(resolutionWidth);
        resolution.height = int.Parse(resolutionHeight);
        return resolution;
    }

    private string GetTextFromResolution(Resolution resolution)
    {
        return resolution.width + " x " + resolution.height;
    }

    private void InitializeResolutionsLists()
    {
        for (var i = 0; i < resolutions.Length; i++)
        {
            var resolution = resolutions[i];
            AddResolutionToList(resolution.width, resolution.height);
        }

        AddResolutionToList(nativeResolution.width, nativeResolution.height);
    }

    private void AddResolutionToList(double width, double height)
    {
        if (MathHelpers.Approximately(width / height, 4.00 / 3.00, 0.001) &&
            (height >= 480 && height <= nativeResolution.height))
        {
            var resolutionToText = width + " x " + height;
            if (!fourByThree.Exists(lambdaExpression => lambdaExpression == resolutionToText))
            {
                fourByThree.Add(resolutionToText);
            }
        }
        else if (MathHelpers.Approximately(width / height, 16.00 / 9.00, 0.001) &&
                 height <= nativeResolution.height)
        {
            var resolutionToText = width + " x " + height;
            if (!sixteenByNine.Exists(lambdaExpression => lambdaExpression == resolutionToText))
            {
                sixteenByNine.Add(resolutionToText);
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
        applyButton.gameObject.SetActive(false);
        CoroutineManager.DeleteCoroutine("ChangeResolutionCoroutine");
    }
}