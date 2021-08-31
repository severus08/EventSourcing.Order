using System.Threading.Tasks;
using MediatR;

namespace Order.Infrastructure.Command
{
    public class CommandBus: ICommandBus
    {
        private readonly IMediator mediator;

        public CommandBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            return mediator.Send(command);
        }
    }
}