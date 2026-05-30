using Microsoft.ML.Data;

namespace TareasApi.Models
{
    // Lo que el modelo necesita para entrenar
    public class SentimientoData
    {
        public string Texto { get; set; } = string.Empty;
        public bool Sentimiento { get; set; } // True = Positivo, False = Negativo
    }

    // Lo que el modelo devuelve tras predecir
    public class SentimientoPrediction : SentimientoData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    // El DTO para el Request/Response del API
    public class SentimientoRequest
    {
        public string Comentario { get; set; } = string.Empty;
    }

    public class SentimientoResponse
    {
        public string Comentario { get; set; } = string.Empty;
        public string Sentimiento { get; set; } = string.Empty;
    }
}