﻿using Microsoft.AspNetCore.Http;
using SharedLibrary.Exceptions.Common;

namespace SharedLibrary.Exceptions
{
    public class BadRequestException : Exception, IBaseException
    {
        public int StatusCode => StatusCodes.Status400BadRequest;

        public string ErrorMessage { get; }

        public BadRequestException() : base()
        {
            ErrorMessage = "Xəta baş verdi.";
        }

        public BadRequestException(string? message) : base(message)
        {
            ErrorMessage = message;
        }
    }
}
