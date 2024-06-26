﻿using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record EventDto(int Id, string Url, string Title, DateTime DateStart, DateTime DateEnd, string ImageLink, string Price, string TicketLink, string Location, string Category);
    public record CreateEventDto(string Url, string Title, DateTime DateStart, DateTime DateEnd, string ImageLink, string Price, string TicketLink, string Location, string Category);
    public record UpdateEventDto(string Url, string Title, DateTime DateStart, DateTime DateEnd, string ImageLink, string Price, string TicketLink, string Location, string Category);
}
