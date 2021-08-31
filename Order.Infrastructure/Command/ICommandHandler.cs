using MediatR;

namespace Order.Infrastructure.Command
{
    public interface ICommandHandler<in T>: IRequestHandler<T>
        where T : ICommand
    {
    }
}