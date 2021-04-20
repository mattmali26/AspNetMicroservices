﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;
        private readonly ILogger<CheckoutOrderCommandHandler> logger;

        public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService, ILogger<CheckoutOrderCommandHandler> logger)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.emailService = emailService;
            this.logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = mapper.Map<Order>(request);
            var newOrder = await orderRepository.AddAsync(orderEntity);

            logger.LogInformation($"Order {newOrder.Id} is created");

            await SendEmailAsync(newOrder);

            return newOrder.Id;
        }

        private async Task SendEmailAsync(Order order)
        {
            var email = new Email()
            {
                To = "mattia@mycros.net",
                Body = "Order was created",
                Subject = "Order was created"
            };

            try
            {
                await emailService.SendEmailAsync(email);
            }
            catch (Exception ex)
            {
                logger.LogError($"Order {order.Id} failed due to an error with the email service: {ex.Message}");
            }
        }
    }
}