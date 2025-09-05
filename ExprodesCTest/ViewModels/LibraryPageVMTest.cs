using ExprodesC.ViewModels;
using ExprodesC.Services;
using Moq;
using Xunit;
using Func.Services;
using Calc.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Calc.Models;
using System.Collections.ObjectModel;
using FluentAvalonia.UI.Controls;
using ExprodesC.Imp;
using Avalonia.Controls;

namespace ExprodesCTest.ViewModels;

// Вспомогательный класс для теста, чтобы можно было изменять коллекции
public class TestExSettingData : ExSettingData
{
    public new List<Population> Populations { get; set; } = new();
    public new ObservableCollection<Population> SourcePopulation { get; set; } = new();
    public new Population CurrentPopulation { get; set; } = new Population { Id = 1, Name = "Current", Ord = 1 };
}

public class LibraryPageVMTest
{
    private LibraryPageVM CreateVM(
        out Mock<ISettingsProvider<ExSettingData>> settingsMock,
        out Mock<IAppDb> appDbMock,
        out Mock<IDialogService> dialogServiceMock,
        out TestExSettingData testSettings)
    {
        testSettings = new TestExSettingData();
        settingsMock = new Mock<ISettingsProvider<ExSettingData>>();
        settingsMock.SetupGet(s => s.Value).Returns(testSettings);
        appDbMock = new Mock<IAppDb>();
        appDbMock.Setup(db => db.GetAllPopLocuses()).Returns(new List<LocusAllele>());
        dialogServiceMock = new Mock<IDialogService>();
        return new LibraryPageVM(settingsMock.Object, appDbMock.Object, dialogServiceMock.Object);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        var vm = CreateVM(out var settingsMock, out var appDbMock, out var dialogServiceMock, out var testSettings);
        Assert.NotNull(vm.Settings);
        Assert.NotNull(vm.Locuses);
        Assert.NotNull(vm.Alleles);
        Assert.NotNull(vm.AddPopulationCommand);
        Assert.NotNull(vm.EditPopulationCommand);
        Assert.NotNull(vm.DelPopulationCommand);
        Assert.NotNull(vm.MovePopulationCommand);
        Assert.NotNull(vm.AddLocusCommand);
        Assert.NotNull(vm.EditLocusCommand);
        Assert.NotNull(vm.DelLocusCommand);
        Assert.NotNull(vm.MoveLocusCommand);
        Assert.NotNull(vm.AddAlleleCommand);
        Assert.NotNull(vm.EditAlleleCommand);
        Assert.NotNull(vm.DelAlleleCommand);
    }

    [Fact]
    public void AddPopulationCommand_AddsPopulation()
    {
        var vm = CreateVM(out var settingsMock, out var appDbMock, out var dialogServiceMock, out var testSettings);
        var newPop = new Population { Name = "TestPop", BaseOn = null };
        dialogServiceMock.Setup(d => d.AddEditPopulation(It.IsAny<UserControl>(), It.IsAny<string>()))
            .ReturnsAsync(newPop);
        appDbMock.Setup(a => a.InsertPopulation(It.IsAny<Population>(), null)).Returns(2);
        int beforeCount = testSettings.SourcePopulation.Count;
        vm.AddPopulationCommand.Execute().Subscribe();
        Assert.True(testSettings.SourcePopulation.Count == beforeCount + 1);
        Assert.Equal("TestPop", testSettings.SourcePopulation.Last().Name);
        Assert.Equal(testSettings.SourcePopulation.Last(), vm.SelectedPopulation);
    }

    [Fact]
    public void EditPopulationCommand_EditsPopulation()
    {
        var vm = CreateVM(out var settingsMock, out var appDbMock, out var dialogServiceMock, out var testSettings);
        var pop = new Population { Id = 3, Name = "OldName", Ord = 3 };
        testSettings.SourcePopulation.Add(pop);
        testSettings.Populations.Add(pop);
        vm.SelectedPopulation = pop;
        var edited = new Population { Id = 3, Name = "NewName", Ord = 3 };
        dialogServiceMock.Setup(d => d.AddEditPopulation(It.IsAny<UserControl>(), It.IsAny<string>()))
            .ReturnsAsync(edited);
        appDbMock.Setup(a => a.UpdatePopulation(It.IsAny<Population>()));
        vm.EditPopulationCommand.Execute(pop).Subscribe();
        Assert.Equal("NewName", vm.SelectedPopulation.Name);
    }

    [Fact]
    public void DelPopulationCommand_DeletesPopulation()
    {
        var vm = CreateVM(out var settingsMock, out var appDbMock, out var dialogServiceMock, out var testSettings);
        var pop = new Population { Id = 4, Name = "ToDelete", Ord = 4 };
        testSettings.SourcePopulation.Add(pop);
        testSettings.Populations.Add(pop);
        vm.SelectedPopulation = pop;
        dialogServiceMock.Setup(d => d.ConfirmDelete(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<System.Action<bool>>()))
            .ReturnsAsync(TaskDialogStandardResult.Yes);
        appDbMock.Setup(a => a.DeletePopulations(It.IsAny<IEnumerable<Population>>()));
        int beforeCount = testSettings.SourcePopulation.Count;
        vm.DelPopulationCommand.Execute().Subscribe();
        Assert.True(testSettings.SourcePopulation.Count == beforeCount - 1);
    }

    [Fact]
    public void MovePopulationCommand_MovesPopulationUp()
    {
        var vm = CreateVM(out var settingsMock, out var appDbMock, out var dialogServiceMock, out var testSettings);
        var pop1 = new Population { Id = 1, Name = "A", Ord = 1 };
        var pop2 = new Population { Id = 2, Name = "B", Ord = 2 };
        testSettings.Populations.Add(pop1);
        testSettings.Populations.Add(pop2);
        vm.SelectedPopulation = pop2;
        appDbMock.Setup(a => a.MovePopulation(pop2, pop1));
        vm.MovePopulationCommand.Execute("Up");
        Assert.Equal(pop1.Ord, vm.SelectedPopulation.Ord);
    }
}
