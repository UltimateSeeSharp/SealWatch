using SealWatch.Code.CutterLayer;
using SealWatch.Code.ProjectLayer;
using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace SealWatch.Wpf.Service;

/// <summary>
/// Validates user inputs
/// </summary>
public class UserInputService : IUserInputService
{
    public bool UserConfirmPopUp(string tool)
    {
        return MessageBox.Show("Sind Sie sicher?", tool, MessageBoxButton.YesNoCancel) is MessageBoxResult.Yes;
    }

    public List<ValidationError> GetInvalidCutterInputs(CutterEditDto cutter)
    {
        List<ValidationError> errors = new();
        errors.Add(new ValidationError("Seriennummer", IsSerialNumber(cutter.SerialNumber)));
        errors.Add(new ValidationError("Fräsdauer [Jahre]", IsLargerZero(cutter.MillingDuration_y)));
        errors.Add(new ValidationError("Fräsdauer/Tag", IsLargerZero(cutter.MillingPerDay_h)));
        errors.Add(new ValidationError("Arbeitstage", IsLargerZero(cutter.WorkDays)));
        errors.Add(new ValidationError("Arbeitstage", IsInRange(cutter.WorkDays, 1, 7)));
        errors.Add(new ValidationError("Lebensdauer [Stunden]", IsLargerZero(cutter.LifeSpan_h)));
        errors.Add(new ValidationError("Frässtart", NotMinDateTime(cutter.MillingStart)));
        
        var occuredErrors = errors.Where(x => !string.IsNullOrEmpty(x.ErrorDescription)).ToList();
        return occuredErrors;
    }

    public List<ValidationError> GetInvalidProjectInputs(ProjectEditDto project)
    {
        List<ValidationError> errors = new();
        errors.Add(new ValidationError("Standort", IsTextNoWhite(project.Location)));
        errors.Add(new ValidationError("Lamellen", IsLargerZero(project.Blades)));
        errors.Add(new ValidationError("Tiefe [Meter]", IsLargerZero(project.SlitDepth_m)));
        errors.Add(new ValidationError("Frässtart", NotMinDateTime(project.StartDate)));

        var occuredErrors = errors.Where(x => !string.IsNullOrEmpty(x.ErrorDescription)).ToList();
        return occuredErrors;
    }

    //  Number

    private string IsLargerZero(double num)
    {
        if (num <= 0)
            return "Nummer muss größer als 0 sein!";
        else
            return string.Empty;
    }

    private string IsInRange(int num, int min, int max)
    {
        if (num > max || num < min)
            return "Nummer ist nicht in der validen Spanne!";
        else
            return string.Empty;
    }

    //  Text

    private string IsSerialNumber(string input)
    {
        if (Regex.IsMatch(input, @"181-\d\d\d"))
            return string.Empty;
        else
            return "Seriennummer hat nicht das korrekte Format";
    }

    private string IsNubmer(string input)
    {
        if (Regex.IsMatch(input, @"^\d$"))
            return string.Empty;
        else
            return "Eingabe konnte nicht gelesen werden! Zahl erwartet!";
    }

    private string IsTextNoWhite(string input)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input)) 
            return "Feld ist leer!";

        if (Regex.IsMatch(input, @"^[a-zA-Z]+$"))
            return string.Empty;
        else
            return "Feld darf nur Buchstaben enthalten";
    }

    //  DateTime

    private string NotMinDateTime(DateTime date)
    {
        if (date != DateTime.MinValue)
            return string.Empty;
        else
            return "Bitte wählen Sie ein aktuelles Startdatum!";
    }
}

public record class ValidationError(string PropertyName, string ErrorDescription);