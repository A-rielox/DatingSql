using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IMessageRepository
{
    Task<bool> AddMessage(Message message);
    Task<bool> DeleteMessage(Message message, string userNameDeleting);
    Task<Message> GetMessage(int id);


    //Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessagesForUser(MessageParams messageParams); // el lo hace paginado


    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
}
