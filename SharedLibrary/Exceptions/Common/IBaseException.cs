﻿namespace SharedLibrary.Exceptions.Common
{
    public interface IBaseException
    {
        public int StatusCode { get; }

        public string ErrorMessage { get; }
    }
}