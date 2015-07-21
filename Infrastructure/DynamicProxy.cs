using Castle.DynamicProxy;

namespace Main
{
    // Currently no in use for this application.
    // If you need Intercepting objects befor they are injected - see this for implementation:
    // http://docs.castleproject.org/Windsor.Interceptors.ashx
    public class DynamicProxy : IInterceptor
    {
        #region IInterceptor Members

        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }

        #endregion
    }
}
