using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ModbusTester_WPF.Command;
using ModbusTester_WPF.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace ModbusTester_WPF.ViewModels
{
    public class GraphViewModel:AbstractViewModel
    {
        private int _index;
        private Dictionary<int, List<DataPoint>> _seriesDictionary;
        private MainViewModel _viewModel;
        private bool _running;
        private List<string> _seriesCollection;

        public PlotModel Model { get; private set; }
        public ICommand AddSeriesCommand { get; private set; }
        public ICommand RemoveSeriesCommand { get; private set; }
        public ICommand StartUpdateCommand { get; private set; }
        public ICommand StopUpdateCommand { get; private set; }

        public List<string> SeriesCollection
        {
            get { return _seriesCollection; }
            set
            {
                _seriesCollection = value; 
                RaisePropertyChanged();
            }
        }

        public int Address { get; set; }
        public GraphViewModel(MainViewModel model)
        {
            CreateModel();
            CreateAxes();
            SeriesCollection = new List<string>();
            InvalidatePlotCommand = new RelayCommand(p => Model.InvalidatePlot(true));
            _viewModel = model;
            _seriesDictionary = new Dictionary<int, List<DataPoint>>();
            AddSeriesCommand = new RelayCommand(a=>AddSeries(Address));
            RemoveSeriesCommand = new RelayCommand(a=>RemoveSeries(Address));
            StartUpdateCommand = new RelayCommand(a=>StartUpdate(),p=>!_running);
            StopUpdateCommand = new RelayCommand(a=>StopUpdate(),p=>_running);
        }

        private void StartUpdate()
        {
            _running = true;
            _viewModel.UpdateEvent += ViewModelOnUpdateEvent;
        }

        private void StopUpdate()
        {
            _running = false;
            _viewModel.UpdateEvent -= ViewModelOnUpdateEvent;
        }
        private void ViewModelOnUpdateEvent(object sender, UpdateEventArgs updateEventArgs)
        {
            if (_seriesDictionary.Count == 0) return;
            UpdateData(updateEventArgs.Address, updateEventArgs.Array);
        }

        public ICommand InvalidatePlotCommand { get; set; }

        public void Clear()
        {
            _seriesDictionary.Clear();
            SeriesCollection.Clear();
            foreach (var ser in Model.Series)
                ((LineSeries)ser).Points.Clear();
            _index = 0;
            Model.InvalidatePlot(true);
        }

        public void UpdateData(int start, ushort[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (_seriesDictionary.ContainsKey(i+start))
                {
                    _seriesDictionary[i+start].Add(new DataPoint(_index,data[i]));
                }
            }
            SeriesCollection = Model.Series.Select(e=>e.Title).ToList();
            Model.InvalidatePlot(true);
            _index++;
        }

        private void AddSeries(int address)
        {
            var newBuff = new List<DataPoint>();
            if (!_seriesDictionary.ContainsKey(address))
            {
                _seriesDictionary.Add(address,newBuff);
            }
            CreateSeries("Регистр "+address, OxyColors.Automatic, newBuff);
            SeriesCollection = Model.Series.Select(e => e.Title).ToList();
        }

        private void RemoveSeries(int address)
        {
            if (_seriesDictionary.ContainsKey(address))
            {
                _seriesDictionary.Remove(address);
                var rem = Model.Series.Where(s => s.Title == ("Регистр "+Address.ToString())).ToArray();
                Model.Series.Remove(rem[0]);
                SeriesCollection = Model.Series.Select(e => e.Title).ToList();
            }
        }

        private void CreateSeries(string Name, OxyColor Color, List<DataPoint> Buffer)
        {
            var series = new LineSeries();
            series.Title = Name;
            series.Color = Color;
            series.ItemsSource = Buffer;
            Model.Series.Add(series);
        }
        private void CreateAxes()
        {
            var force = new LinearAxis();
            force.Title = "Индекс";
            force.Position = AxisPosition.Bottom;
            force.MajorGridlineColor = OxyColor.FromArgb(128, 0, 0, 0);
            force.MajorGridlineStyle = LineStyle.Solid;
            force.MinorGridlineColor = OxyColor.FromArgb(64, 64, 64, 64);
            force.MinorGridlineStyle = LineStyle.Solid;
            force.AxisTitleDistance = 10;
            Model.Axes.Add(force);

            var tenz = new LinearAxis();
            tenz.Title = "Величина, попугаи";
            tenz.Key = "Def";
            tenz.AxisTitleDistance = 10;
            tenz.Position = AxisPosition.Left;
            tenz.MajorGridlineColor = OxyColor.FromArgb(196, 0, 0, 0);
            tenz.MajorGridlineStyle = LineStyle.Solid;
            tenz.MinorGridlineColor = OxyColor.FromArgb(128, 65, 64, 64);
            tenz.MinorGridlineStyle = LineStyle.Solid;
            tenz.AxisTitleDistance = 10;
            Model.Axes.Add(tenz);;
        }

        private void CreateModel()
        {
            Model = new PlotModel();
            //Model.Title = Name;
            Model.IsLegendVisible = false;
        }
    }
}
