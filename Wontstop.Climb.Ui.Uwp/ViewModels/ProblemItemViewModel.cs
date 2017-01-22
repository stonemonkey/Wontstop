// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Mvvm.WinRT.AttachedProperties;
using Mvvm.WinRT.Commands;
using PropertyChanged;
using Wontstop.Climb.Ui.Uwp.Dtos;

namespace Wontstop.Climb.Ui.Uwp.ViewModels
{
    [ImplementPropertyChanged]
    public class ProblemItemViewModel
    {
        public bool Busy { get; private set; }

        private const int MinNoTries = 1;
        private const int MaxNoTries = 100;

        public int Tries { get; set; } = MinNoTries;

        [Model]
        public Problem Problem { get; set; }

        private RelayCommand _incrementTriesComand;
        public RelayCommand IncrementTriesCommand => _incrementTriesComand ??
            (_incrementTriesComand = new RelayCommand(
                () => Tries++, () => (Tries < MaxNoTries) && !Busy));

        private RelayCommand _decrementTriesComand;
        public RelayCommand DecrementTriesCommand => _decrementTriesComand ??
            (_decrementTriesComand = new RelayCommand(
                () => Tries--, () => (Tries > MinNoTries) && !Busy));

        private RelayCommand _tickComand;
        public RelayCommand TickCommand => _tickComand ??
            (_tickComand = new RelayCommand(async () => await TickAsync(), () => !Busy));

        private async Task TickAsync()
        {
            Busy = true;

            // TODO: send it to Problemator ...
            await Task.Delay(2000);

            Busy = false;
        }

        private readonly ProblematorRequestsFactory _requestsFactory;

        public ProblemItemViewModel(ProblematorRequestsFactory requestsFactory)
        {
            _requestsFactory = requestsFactory;
        }
    }
}