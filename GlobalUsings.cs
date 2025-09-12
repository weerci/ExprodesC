global using Microsoft.Extensions.DependencyInjection;
global using ReactiveUI;
global using ReactiveUI.Fody.Helpers;
global using System;
global using System.Collections.Generic;
global using System.Reactive;
global using RxCommandUnit = ReactiveUI.ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>;
global using RxCommandGenotype = ReactiveUI.ReactiveCommand<ExprodesC.Wrappers.GenotypeWR, System.Reactive.Unit>;
global using RxCommandGolumnVM = ReactiveUI.ReactiveCommand<ExprodesC.Views.Controls.GenotypeColumnVM, System.Reactive.Unit>;
global using RxCommandPopulation = ReactiveUI.ReactiveCommand<Calc.Models.Population, System.Reactive.Unit>;
global using RxCommandLocusWR = ReactiveUI.ReactiveCommand<ExprodesC.Wrappers.LocusWR, System.Reactive.Unit>;
global using RxCommandSynonymWR = ReactiveUI.ReactiveCommand<ExprodesC.Wrappers.SynonymWR, System.Reactive.Unit>;
global using RxCommandLocusAllele = ReactiveUI.ReactiveCommand<Calc.Data.LocusAllele, System.Reactive.Unit>;
global using RxCommandText = ReactiveUI.ReactiveCommand<string, System.Reactive.Unit>;


