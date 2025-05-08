using MediatR;

namespace Application.Common;

public interface ICommandRequest : IRequest;

public interface ICommandRequest<out T> : IRequest<T>, ICommandRequest;
