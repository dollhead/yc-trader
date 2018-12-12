using System;

namespace YCTrader.Services.Predictor
{
    public class NotEnoughDataException : Exception
    {
        public NotEnoughDataException(string message) : base(message)
        {
        }
    }
}