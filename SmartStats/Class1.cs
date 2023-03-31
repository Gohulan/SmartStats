using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace SmartStats
{
    public class LinearRegressionTrainer
    {
        public void TrainModel(List<InputData> trainingData)
        {
            var context = new MLContext();

            // Convert training data to IDataView
            var dataView = context.Data.LoadFromEnumerable(trainingData);

            // Define training pipeline
            var pipeline = context.Transforms.Concatenate("Features", "X")
                .Append(context.Transforms.NormalizeMinMax("Features"))
                .Append(context.Transforms.Conversion.MapValueToKey("Label"))
                //.Append(context.Transforms.TrainingCache())
                .Append(context.Transforms.Concatenate("Features", "Features", "Label"))
                .Append(context.Transforms.Conversion.MapKeyToValue("Label"))
                .Append(context.Regression.Trainers.Sdca());

            // Train the model
            var model = pipeline.Fit(dataView);

            // Save the model
            context.Model.Save(model, dataView.Schema, "model.zip");
        }
    }

    public class InputData
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

}
