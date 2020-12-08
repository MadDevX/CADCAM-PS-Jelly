using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Plotting
{
    public static class DataExporter
    {
        public static PlotModel CreateModel(float[] xValues, float[] yValues, string name)
        {
            var model = new PlotModel() { Title = name };
            var data = new LineSeries();
            for (int i = 0; i < xValues.Length; i++)
            {
                data.Points.Add(new DataPoint(xValues[i], yValues[i]));
            }
            model.Series.Add(data);

            return model;
        }

        public static PlotModel CreateModel(string name, params string[] seriesTitles)
        {
            var model = new PlotModel() { Title = name };
            for (int i = 0; i < seriesTitles.Length; i++)
            {
                var data = new LineSeries() {Title=seriesTitles[i]};
                model.Series.Add(data);
            }
            return model;
        }
    }
}
