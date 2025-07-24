using ExprodesC.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Reactive.Linq;

namespace ExprodesCTest.Model;

public class ProjectTest
{

   /* [Fact]
    public void Create_project_test()
    {
        IProject project = Init.Services.GetService<IProject>()!;
        IProject project1 = Init.Services.GetService<IProject>()!;
        Assert.Equal(project.GetHashCode, project1.GetHashCode);

        Assert.True(project.IsSaved);
        var v = project
            .ConnectControl()
            .Subscribe(it => Debug.WriteLine("sdfsdfsdffdsfdfsdfsdfs"));
        Debug.WriteLine("sdfsdfsdffdsfdfsdfsdfs");
        *//*        Assert.False(project.ConnectControl().Select(n=>n).Count() == 0);
                Assert.False(project.Experts.Any());*//*

    }*/
    /*
        [Fact]
        public void Project_save_and_load_test()
        {
            var project = Init.Services.GetService<IProject>()!;

            project.LoadExperts();
            Assert.Empty(project.Experts);
            Assert.Empty(project.Profiles);

            project.LoadProfiles(FileName.Open(@"Calculation\Relative\1-2-3_детей_и_1_предполагаемый_родитель_LR.mgj"));
            Assert.Equal(4, project.Profiles.Count);

            var gs = Init.Services.GetService<IGenotypeStore>()!;
            gs.LoadFromFile(FileName.Open(@"Calculation\Relative\1-2-3_детей_и_1_предполагаемый_родитель_LR.mgj"));
            gs.SaveControls();

            project.LoadExperts();
            Assert.Equal(4, project.Experts.Count);

            project.Experts.RemoveAt(0);
            Assert.Equal(3, project.Experts.Count);

            project.SaveExperts();

            project.Experts.Clear();
            Assert.Empty(project.Experts);

            project.LoadExperts();
            Assert.Equal(3, project.Experts.Count);

        }

        [Fact]
        public void Project_add_del_expert_and_profiles()
        {

        }

        [Fact]
        public void Project_add_del_dynamic_data()
        {
            ReadOnlyObservableCollection<Genotype> _items;

            Project project = Init.Services.GetService<Project>()!;

            project.ConnectExpert()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            Assert.False(_items.Any());
            project.addExpert(new("add1", []));
            project.addExpert(new("add2", []));
            Assert.True(_items.Any());

            project.saveExperts();

        }*/
}
