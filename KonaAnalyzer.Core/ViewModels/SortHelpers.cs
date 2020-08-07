using System;
using DynamicData.Binding;
using PolyhydraGames.Extensions;

namespace KonaAnalyzer.Core.ViewModels
{
    public static class SortHelpers
    {
        public static Sort GetSort(this string command, Sort existingSort)
        {
            var existingSortString = existingSort.ToString();

            if (existingSortString.Contains(command))
            {
                return (command + (existingSortString.Contains("Ascending") ? "Descending" : "Ascending"))
                    .ToEnum<Sort>();
            }
            else
            {
                command = command + "Descending";
                var sort = command.ToEnum<Sort>();
                return sort;
            }
        }

        public static SortExpressionComparer<StateControlViewModel> GetSorter(this Sort x)
        {
            switch (x)
            {
                case Sort.DeadAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Dead);
                case Sort.DeadDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Dead);
                case Sort.TotalAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Current);
                case Sort.TotalDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Current);
                case Sort.AlphabeticalAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.County);
                case Sort.AlphabeticalDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.County);
                case Sort.MortalityAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.MortalityRate);
                case Sort.MortalityDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.MortalityRate);

                case Sort.RiskAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentRiskRate);
                case Sort.RiskDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentRiskRate);

                case Sort.ChangeAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentChange);
                case Sort.ChangeDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentChange);

                case Sort.PercentAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.CurrentChangeRate);
                case Sort.PercentDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.CurrentChangeRate);

                case Sort.PopulationAscending: return SortExpressionComparer<StateControlViewModel>.Ascending(vm => vm.Population);
                case Sort.PopulationDescending: return SortExpressionComparer<StateControlViewModel>.Descending(vm => vm.Population);


                default: throw new ArgumentOutOfRangeException(nameof(x), x, null);
            }
        }

        //public static string ToFriendlyString(this Sort x)
        //{
        //    switch (x)
        //    {
        //        case Sort.DeadAscending: return "Deaths Asc.";
        //        case Sort.DeadDescending: return "Deaths Desc.";
        //        case Sort.TotalAscending: return "Total Asc.";
        //        case Sort.TotalDescending: return "Total Desc.";
        //        case Sort.AlphabeticalAscending: return "Alpha Asc.";
        //        case Sort.AlphabeticalDescending: return "Alpha Desc.";
        //        case Sort.MortalityAscending: return "Mort Asc.";
        //        case Sort.MortalityDeccending: return "Mort Desc.";
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(x), x, null);
        //    }
        //}
    }
}