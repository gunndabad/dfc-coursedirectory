﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Dfc.CourseDirectory.WebV2.MultiPageTransaction
{
    public class MptxInstanceContextModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var instanceContextType = typeof(MptxInstanceContext<>);
            var modelType = context.Metadata.ModelType;

            if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == instanceContextType)
            {
                return new MptxInstanceContextModelBinder();
            }
            else
            {
                return null;
            }
        }
    }

    public class MptxInstanceContextModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var stateType = bindingContext.ModelType.GetGenericArguments()[0];

            var feature = bindingContext.HttpContext.Features.Get<MptxInstanceFeature>();

            if (feature == null)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "No active MPTX instance.");
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else if (feature.Instance.State != null && stateType != feature.Instance.State.GetType())
            {
                bindingContext.ModelState.AddModelError(
                    bindingContext.ModelName,
                    "Persisted state type does not match requested type.");
                bindingContext.Result = ModelBindingResult.Failed();
            }
            else
            {
                var model = ActivatorUtilities.CreateInstance(
                    bindingContext.HttpContext.RequestServices,
                    bindingContext.ModelType,
                    feature.Instance);
                bindingContext.Result = ModelBindingResult.Success(model);
            }

            return Task.CompletedTask;
        }
    }
}