using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;

namespace Conduit.Shared
{
    /// <summary>
    /// Used with an EditForm or EditContext to add errors returned from the Conduit API to the ValidationSummary. Based
    /// on https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-3.1#validator-components.
    /// </summary>
    /// <remarks> 
    /// This is an example of a class-only Blazor component. There is no corresponding .razor file. The 
    /// <ServerValidator/> tag will be nested inside an <EditForm>/<EditContext> to obtain the CascadingParameter, but
    /// the <ServerValidator/> itself renders no HTML. We could override BuildrenderTree() to render HTML without 
    /// creating a .razor file, but I HIGHLY recommend against it unless you have a good reason.
    /// </remarks>
    public class ServerValidator : ComponentBase
    {
        private ValidationMessageStore messageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(ServerValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}. " +
                    $"For example, you can use {nameof(ServerValidator)} " +
                    $"inside an {nameof(EditForm)}.");
            }

            messageStore = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) =>
                messageStore.Clear();
            CurrentEditContext.OnFieldChanged += (s, e) =>
                messageStore.Clear(e.FieldIdentifier);
        }

        /// <summary>
        /// See DisplayErrors
        /// </summary>
        public void DisplayError(string key, string error)
        {
            DisplayErrors(new Dictionary<string, List<string>> { { key, new List<string>() { error } } });
        }

        /// <summary>
        /// Adds each entry in the Dictionary to the CurrentEditContext's ValidationMessageStore. If a supplied Key does
        /// not correspond to a field in the CurrentEditContext that entry's key and values will be concatonated and 
        /// added tot he ValidationMessageStore with a string.Empty key. Messages with a string.Empty key are available
        /// via a ValidationSummary.
        /// </summary>
        public void DisplayErrors(Dictionary<string, List<string>> errors)
        {
            foreach (var err in errors)
            {
                var fieldObject = CurrentEditContext.Field(err.Key);

                var property = fieldObject.Model.GetType().GetProperty(err.Key);

                if (property != null)
                {
                    messageStore.Add(CurrentEditContext.Field(err.Key), err.Value);
                }
                else
                {
                    foreach (var message in err.Value)
                    {
                        messageStore.Add(CurrentEditContext.Field(string.Empty), $"{err.Key} {message}");
                    }
                }
            }

            CurrentEditContext.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore.Clear();
            CurrentEditContext.NotifyValidationStateChanged();
        }
    }
}
