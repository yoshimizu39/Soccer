namespace Soccer.Common.Models
{
    public class Response //peticiones a una respuesta
    {
        public bool IsSuccess { get; set; } //si es exitosa devuelve true
        public string Message { get; set; } //mensage de error si es false IsSuccess
        public object Result { get; set; } //devuelve un objeto de resultado
    }
}
