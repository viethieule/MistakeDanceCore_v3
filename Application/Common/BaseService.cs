using System.Text;
using Application.Common.Exceptions;
using FluentValidation.Results;

namespace Application.Common
{
    public abstract class BaseService<TRq, TRs>
        where TRq : BaseRequest
        where TRs : BaseResponse
    {
        public async Task<TRs> RunAsync(TRq rq)
        {
            ValidationResult vr = Validate(rq);
            if (vr.IsValid)
            {
                return await DoRunAsync(rq);
            }

            StringBuilder sb = new StringBuilder();
            vr.Errors.ToList().ForEach(x => sb.AppendLine(x.ErrorMessage));
            throw new ServiceException(sb.ToString());
        }

        protected virtual ValidationResult Validate(TRq rq)
        {
            return new ValidationResult();
        }

        protected abstract Task<TRs> DoRunAsync(TRq rq);
    }
}