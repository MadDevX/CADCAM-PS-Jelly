using MadEngine.Plotting;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class Plotter : UpdatableMComponent
    {
        public event Action AmplitudeUpdated;

        private SpringMovement _spring;

        private PlotView _forcesPlot;
        private PlotView _xxtxttPlot;
        private PlotView _statePlot;

        private PlotModel _forcesModel;
        private PlotModel _xxtxttModel;
        private PlotModel _stateModel;

        private PlotView[] _plots;
        private int _plotToInvalidate = 0;

        public Plotter(PlotView forcesPlot, PlotView xxtxttPlot, PlotView statePlot)
        {
            _forcesPlot = forcesPlot;
            _xxtxttPlot = xxtxttPlot;
            _statePlot = statePlot;
            _plots = new PlotView[] { _forcesPlot, _xxtxttPlot, _statePlot };
            _forcesModel = DataExporter.CreateModel("Forces Graph", "f(t)", "g(t)", "h(t)", "w(t)");
            _xxtxttModel = DataExporter.CreateModel("Motion Graph", "x(t)", "x_t(t)", "x_tt(t)");
            _stateModel = DataExporter.CreateModel("State Graph", "State Vector");

            _forcesPlot.Model = _forcesModel;
            _xxtxttPlot.Model = _xxtxttModel;
            _statePlot.Model = _stateModel;

            SetGridlines(_forcesModel);
            SetGridlines(_xxtxttModel);
            SetGridlines(_stateModel);

            SetAxesLabels(_forcesModel, "Time", "Force");
            SetAxesLabels(_xxtxttModel, "Time", "Value");
            SetAxesLabels(_stateModel, "Position", "Velocity");
        }

        private void SetAxesLabels(PlotModel model, string xLabel, string yLabel)
        {
            model.Axes[0].Title = xLabel;
            model.Axes[1].Title = yLabel;
        }

        private void SetGridlines(PlotModel model)
        {
            model.Axes[0].MajorGridlineStyle = LineStyle.Solid;
            model.Axes[1].MajorGridlineStyle = LineStyle.Solid;
            model.Axes[0].MinorGridlineStyle = LineStyle.Dash;
            model.Axes[1].MinorGridlineStyle = LineStyle.Dash;
            model.Axes[0].MajorGridlineColor = OxyColor.FromArgb(100, 100, 100, 100);
            model.Axes[1].MajorGridlineColor = OxyColor.FromArgb(100, 100, 100, 100);
            model.Axes[0].MinorGridlineColor = OxyColor.FromArgb(30, 100, 100, 100);
            model.Axes[1].MinorGridlineColor = OxyColor.FromArgb(30, 100, 100, 100);
        }

        public override void Initialize()
        {
            _spring = OwnerNode.GetComponent<SpringMovement>();
            _spring.OnSimulationUpdated += UpdateData;
            _spring.OnSimulationReset += ResetData;
            base.Initialize();
        }

        private void ResetData()
        {
            _forcesYMax = 0.0;
            _xxtxttYMax = 0.0;
            _stateXYMax = 0.0;

            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[0])).Points.Clear();
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[1])).Points.Clear();
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[2])).Points.Clear();
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[3])).Points.Clear();

            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[0])).Points.Clear();
            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[1])).Points.Clear();
            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[2])).Points.Clear();

            ((OxyPlot.Series.LineSeries)(_stateModel.Series[0])).Points.Clear();
        }

        public override void Dispose()
        {
            _spring.OnSimulationUpdated -= UpdateData;
            base.Dispose();
        }

        double _forcesYMax = 0.0;
        double _xxtxttYMax = 0.0;
        double _stateXYMax = 0.0;

        private void UpdateData(double t)
        {
            var state = _spring.CurrentState;

            var f = _spring.f(t);
            var g = _spring.g(t);
            var h = _spring.h(t);
            var w = _spring.w(t);

            var x = state.pos;
            var xt = state.vel;
            var xtt = state.acc;

            var forcesAbsNew = MathExtensions.MaxAbs(f, g, h, w);
            var xxtxttAbsNew = MathExtensions.MaxAbs(x, xt, xtt);
            var stateAbsNew = MathExtensions.MaxAbs(x, xt);
            _forcesYMax = forcesAbsNew > _forcesYMax ? forcesAbsNew : _forcesYMax;
            _xxtxttYMax = xxtxttAbsNew > _xxtxttYMax ? xxtxttAbsNew : _xxtxttYMax;
            _stateXYMax = stateAbsNew > _stateXYMax ? stateAbsNew : _stateXYMax;

            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[0])).Points.Add(new DataPoint(t, f));
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[1])).Points.Add(new DataPoint(t, g));
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[2])).Points.Add(new DataPoint(t, h));
            ((OxyPlot.Series.LineSeries)(_forcesModel.Series[3])).Points.Add(new DataPoint(t, w));

            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[0])).Points.Add(new DataPoint(t, x));
            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[1])).Points.Add(new DataPoint(t, xt));
            ((OxyPlot.Series.LineSeries)(_xxtxttModel.Series[2])).Points.Add(new DataPoint(t, xtt));

            ((OxyPlot.Series.LineSeries)(_stateModel.Series[0])).Points.Add(new DataPoint(x, xt));

            _forcesModel.Axes[0].Minimum = Math.Max(0, t - 10.0f);
            _forcesModel.Axes[0].Maximum = Math.Max(10.0f, t);
            _forcesModel.Axes[1].Minimum = -_forcesYMax;
            _forcesModel.Axes[1].Maximum = _forcesYMax;

            _xxtxttModel.Axes[0].Minimum = Math.Max(0, t - 10.0f);
            _xxtxttModel.Axes[0].Maximum = Math.Max(10.0f, t);
            _xxtxttModel.Axes[1].Minimum = -_xxtxttYMax;
            _xxtxttModel.Axes[1].Maximum = _xxtxttYMax;


            _stateModel.Axes[0].Minimum = _stateModel.Axes[1].Minimum = -_stateXYMax;
            _stateModel.Axes[0].Maximum = _stateModel.Axes[1].Maximum = _stateXYMax;


            UpdateAmplitude(t, x, w);
        }

        double _lastMax = 0.0;
        double _lastMin = 0.0;

        double _xVal1 = 0.0;
        double _xVal2 = 0.0;
        double _xVal3 = 0.0;

        double _wVal1 = 0.0;
        double _wVal2 = 0.0;
        double _wVal3 = 0.0;

        double _txPeak = 0.0;
        double _twPeak = 0.0;
        double _pendingTwPeak = 0.0;

        double _prevTime = 0.0;

        private void UpdateAmplitude(double t, double x, double w)
        {
            _xVal1 = _xVal2;
            _xVal2 = _xVal3;
            _xVal3 = x;

            _wVal1 = _wVal2;
            _wVal2 = _wVal3;
            _wVal3 = w;

            if (_wVal2 >= _wVal1 && _wVal2 >= _wVal3) //found W Max
            {
                _pendingTwPeak = _prevTime;
            }
            if (_xVal2 >= _xVal1 && _xVal2 >= _xVal3) //found x Max
            {
                _txPeak = _prevTime;
                _twPeak = _pendingTwPeak;
                _lastMax = _xVal2;
                AmplitudeUpdated?.Invoke();
            }
            else if (_xVal2 <= _xVal1 && _xVal2 <= _xVal3) //found x Min
            {
                _lastMin = _xVal2;
                AmplitudeUpdated?.Invoke();
            }

            _prevTime = t;
        }

        public double Amplitude => (_lastMax - _lastMin) * 0.5;
        public double PhaseDifference => (_txPeak - _twPeak) * _spring.omega;

        protected override void OnUpdate(float deltaTime)
        {
            if (_plotToInvalidate < _plots.Length) _plots[_plotToInvalidate].InvalidatePlot(true);
            _plotToInvalidate = (_plotToInvalidate + 1) % 3;
        }
    }
}
