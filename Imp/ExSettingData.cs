using Avalonia.Styling;
using Calc.Data;
using Calc.Imp;
using ExprodesC.Models;
using Func;
using Func.Services;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Text.Json.Serialization;

namespace ExprodesC.Imp;

public class ExSettingData : AppSettingsData
{
    const string _settingsFile = "exp.set";

    /// <summary>
    /// Устанавливает или возвращает файл с настройками приложения
    /// </summary>
    [Reactive] public string SettingFile { get; set; } = _settingsFile;
    public Ex<FileName> GetSettingFile => FileName.OpenOrCreate(_settingsFile);
    
    /// <summary>
    /// Если true - то при попытке удалить генотип, появляется окно подтверждения на удаление генотипа
    /// </summary>
    [Reactive] public bool ConfirmDeleteGenotype { get; set; } = true;

    /// <summary>
    /// Если true - то при попытке удалить популяцию, появляется окно подтверждения на удаление генотипа
    /// </summary>
    [Reactive] public bool ConfirmDeletePopulation { get; set; } = true;

    /// <summary>
    /// Если true - то при попытке удалить локуса, появляется окно подтверждения на удаление локуса
    /// </summary>
    [Reactive] public bool ConfirmDeleteLocus { get; set; } = true;

    /// <summary>
    /// Если true - то при попытке удалить аллель, появляется окно подтверждения на удаление аллеля
    /// </summary>
    [Reactive] public bool ConfirmDeleteAllele { get; set; } = true;

    /// <summary>
    /// Если true - то при попытке удалить синоним, появляется окно подтверждения на удаление синонима
    /// </summary>
    [Reactive] public bool ConfirmDeleteSynonym { get; set; } = true;

    /// <summary>
    /// Текущая тема приложения
    /// </summary>
    [Reactive] public Themes CurrentTheme { get; set; } = Themes[0];
    
    /// <summary>
    /// Высота выпадающего списка содержащего детализацию расчета
    /// </summary>
    [Reactive] public int MaxDetailsHeight { get; set; } = 65;
    [Reactive] public int MinDetailsHeight { get; set; } = 35;

    /// <summary>
    /// Набор возможных тем для приложения
    /// </summary>
    public static ObservableCollection<Themes> Themes { get; set; } = [
        new(){ Id = 0, Name = Lang.Resources.theme_sys},
        new(){ Id = 1, Name = Lang.Resources.theme_light},
        new(){ Id = 2, Name = Lang.Resources.theme_dark},
        ];

}


