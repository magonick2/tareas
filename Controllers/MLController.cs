using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using TareasApi.Models;

namespace TareasApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MLController : ControllerBase
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        public MLController()
        {
            _mlContext = new MLContext();
            _model = EntrenarModelo();
        }

        private ITransformer EntrenarModelo()
        {
            // Dataset básico de entrenamiento
            SentimientoData[] data = [
    // Positivos
    new() { Texto = "Excelente trabajo", Sentimiento = true },
    new() { Texto = "El sistema funciona increíble", Sentimiento = true },
    new() { Texto = "Me gusta mucho la aplicación", Sentimiento = true },
    new() { Texto = "Todo bien", Sentimiento = true },
    new() { Texto = "Buena interfaz", Sentimiento = true },
    
    // Negativos
    new() { Texto = "El sistema es una porquería", Sentimiento = false },
    new() { Texto = "No funciona nada", Sentimiento = false },
    new() { Texto = "Es muy lento y malo", Sentimiento = false },
    new() { Texto = "Horrible experiencia", Sentimiento = false },
    new() { Texto = "Falla todo el tiempo", Sentimiento = false }
];

            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // Pipeline: Convertir texto a números y aplicar algoritmo
            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimientoData.Texto))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimientoData.Sentimiento), maximumNumberOfIterations: 100));

            return pipeline.Fit(trainingData);
        }

        [HttpPost("sentimiento")]
        public ActionResult<SentimientoResponse> AnalizarSentimiento([FromBody] SentimientoRequest request)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimientoData, SentimientoPrediction>(_model);
            
            var sample = new SentimientoData { Texto = request.Comentario };
            var result = predictionEngine.Predict(sample);

            return Ok(new SentimientoResponse
            {
                Comentario = request.Comentario,
                Sentimiento = result.Prediction ? "Positivo" : "Negativo"
            });
        }
    }
}