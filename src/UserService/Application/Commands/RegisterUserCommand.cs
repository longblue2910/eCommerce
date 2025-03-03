﻿using MediatR;

namespace Application.Commands;

public record RegisterUserCommand(string Username, string Email, string Password) : IRequest<Guid>;
