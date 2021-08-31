using MediatR;

namespace Order.Infrastructure.Query
{
    public interface IQuery<out TResponse>: IRequest<TResponse>
    {
    }
}