﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Test.Utilities.MinimalImplementations
{
    public static class ASPNetCoreApis
    {
        public const string CSharp = @"
using System;
using System.Threading.Tasks;

class MyValidateAntiForgeryAttribute : Attribute
{
}

namespace Microsoft.AspNetCore
{
    namespace Antiforgery
    {
        using Microsoft.AspNetCore.Http;

        public interface IAntiforgery
        {
            Task ValidateRequestAsync (HttpContext httpContext);
        }

        namespace Internal
        {
            using Microsoft.AspNetCore.Http;

            public class DefaultAntiforgery : IAntiforgery
            {
                public Task ValidateRequestAsync (HttpContext httpContext)
                {
                    return null;
                }
            }
        }
    }

    namespace Mvc
    {
        public class AcceptedAtActionResult
        {
        }

        public abstract class ControllerBase
        {
            public virtual AcceptedAtActionResult AcceptedAtAction (string actionName)
            {
                return null;
            }
        }

        public abstract class Controller : ControllerBase
        {
        }

        public class HttpPostAttribute : Attribute
        {
        }

        public class HttpPutAttribute : Attribute
        {
        }

        public class HttpDeleteAttribute : Attribute
        {
        }

        public class HttpPatchAttribute : Attribute
        {
        }

        public class HttpGetAttribute : Attribute
        {
        }

        public sealed class NonActionAttribute : Attribute
        {
        }

        namespace Filters
        {
            public class FilterCollection : System.Collections.ObjectModel.Collection<IFilterMetadata>
            {
                public FilterCollection ()
                {
                }

                public IFilterMetadata Add<TFilterType> () where TFilterType : IFilterMetadata
                {
                    return null;
                }

                public IFilterMetadata Add (Type filterType)
                {
                    return null;
                }
            }

            public interface IFilterMetadata
            {
            }

            public class AuthorizationFilterContext
            {
            }

            public interface IAsyncAuthorizationFilter : IFilterMetadata
            {
                Task OnAuthorizationAsync (AuthorizationFilterContext context);
            }
        }
    }

    namespace Http
    {
        public abstract class HttpContext
        {
        }
    }
}";
    }
}
