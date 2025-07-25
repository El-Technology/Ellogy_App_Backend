﻿using TicketsManager.BLL.Dtos.MessageDtos;
using TicketsManager.BLL.Dtos.NotificationDtos;
using TicketsManager.BLL.Dtos.TicketSummaryDtos;
using TicketsManager.DAL.Enums;

namespace TicketsManager.BLL.Dtos.TicketDtos
{
    /// <summary>
    /// Represents the request data for updating a ticket.
    /// </summary>
    public class TicketUpdateRequestDto
    {
        /// <summary>
        /// The updated title of the ticket.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The updated description of the ticket.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The updated list of summaries of the ticket.
        /// </summary>
        public List<TicketSummaryFullDto>? TicketSummaries { get; set; }

        /// <summary>
        /// The context of all conversation with bot.
        /// </summary>
        public string? Context { get; set; }

        /// <summary>
        /// The updated comment associated with the ticket.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// The updated status of the ticket.
        /// </summary>
        public TicketStatusEnum Status { get; set; }

        /// <summary>
        /// Shows at which step the user is at
        /// </summary>
        public TicketCurrentStepEnum CurrentStep { get; set; }

        /// <summary>
        /// The messages list of ticket.
        /// </summary>
        public List<MessageResponseDto> Messages { get; set; }

        /// <summary>
        /// The notifications of the ticket with ids
        /// </summary>
        public List<NotificationFullDto> Notifications { get; set; }
    }
}
